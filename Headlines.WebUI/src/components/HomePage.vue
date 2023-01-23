<template>
    <section class="section-first">
        <div class="greet-title">
            <h1 class="ml10">
                <span class="color-yellow text-wrapper">
                    <span :style="[showGreetingOnMount == true ? 'opacity: 1' : 'opacity: 0']" class="letters">Vítej na Titulkovači</span>
                </span>
            </h1>
        </div>
        <div class="section-first-content h-100">
            <h1 class="ml7">
                <span class="color-yellow text-wrapper">
                    <span class="letters">Nejlepší změny titulků</span>
                </span>
            </h1>
            <div class="masked hide-scrollbar" style="height: 85%;">
                <TopHeadlinesTable v-on:upvoted="upvoted"
                                   :headlineChanges="topHeadlineChanges"
                                   :startAnimDelay="topHeadlineChangesStartAnimDelay"
                                   :userToken="userData.userToken"
                                   :userUpvotes="userUpvotes" />
            </div>
        </div>
    </section>
    <section class="spacer layer1"></section>

    <section class="section-second">
        <div class="section-second-content">
            <h1 class="section-second-title">Nedávné změny titulků</h1>
            <HeadlineChangesTable v-on:upvoted="upvoted"
                               v-on:fetchheadlinechanges="fetchHeadlineChangePage"
                               :headlineChanges="shownHeadlineChanges"
                               :headlineChangeCount="headlineChangeCount"
                               :recordsPerPage="recordsPerPage"
                               :userToken="userData.userToken"
                               :userUpvotes="userUpvotes" />
        </div>
    </section>
</template>

<script>
import TopHeadlinesTable from './TopHeadlinesTable.vue'
import HeadlineChangesTable from './HeadlineChangesTable.vue'
import anime from 'animejs'
import axios from 'axios'
import endpoints from './../api/endpoints.js'

export default {
    name: 'HomePage',
    data() {
        return {
            showGreetingOnMount: false,
            topHeadlineChangesStartAnimDelay: 500,
            topHeadlineChanges: [],
            shownHeadlineChanges: [],
            headlineChangeCount: 0,
            recordsPerPage: 10,
            currentPage: 0,
        }
    },
    components: {
        TopHeadlinesTable,
        HeadlineChangesTable
    },
    computed: {
        userData() {
            return this.$store.getters.userData
        },
        userUpvotes() {
            return this.$store.getters.userUpvotes
        }
    },
    methods: {
        fetchTopHeadlines() {
            axios
                .get(endpoints.HeadlineChanges.GetTopUpvoted(10))
                .then(response => {
                    this.topHeadlineChanges = response.data.headlineChanges
                })
        },
        fetchHeadlineChangePage(page) {
            axios
                .get(endpoints.HeadlineChanges.GetSkipTake(page * this.recordsPerPage, this.recordsPerPage))
                .then(response => {
                    this.shownHeadlineChanges = response.data.headlineChanges
                })
            this.fetchHeadlineChangeCount()
        },
        fetchHeadlineChangeCount() {
            axios
                .get(endpoints.HeadlineChanges.GetCount())
                .then(response => {
                    this.headlineChangeCount = response.data
                })
        },
        resolveAnimations(showGreetings) {
            var bestChangesDelay = 0

            if (showGreetings == true) {
                bestChangesDelay = 2500
                this.topHeadlineChangesStartAnimDelay = 3300
                this.animateGreetingTitle()
            }

            this.animateBestChangesTitle(bestChangesDelay)
        },
        animateGreetingTitle() {
            var textWrapper = document.querySelector('.ml10 .letters')
            textWrapper.innerHTML = textWrapper.textContent.replace(/\S/g, "<span class='letter' style='display: inline-block; line-height: 1em; transform-origin: 0 0;'>$&</span>")

            anime.timeline({ loop: false })
                .add({
                    targets: '.ml10 .letter',
                    rotateY: [-90, 0],
                    duration: 1300,
                    delay: (el, i) => 45 * i
                }).add({
                    targets: '.ml10',
                    opacity: 0,
                    duration: 1000,
                    easing: "easeOutExpo",
                    delay: 250
                })
        },
        animateBestChangesTitle(baseDelay) {
            var textWrapper2 = document.querySelector('.ml7 .letters')
            textWrapper2.innerHTML = textWrapper2.textContent.replace(/\S/g, "<span class='letter' style='transform-origin: 0 100%; display: inline-block; line-height: 1em;'>$&</span>")

            anime.timeline({ loop: false })
                .add({
                    targets: '.ml7 .letter',
                    translateY: ["1.1em", 0],
                    translateX: ["0.55em", 0],
                    translateZ: 0,
                    rotateZ: [180, 0],
                    duration: 750,
                    easing: "easeOutExpo",
                    delay: (el, i) => baseDelay + 50 * i,
                    complete: () => this.topHeadlineChangesStartAnimDelay = 500
                })
        },
        upvoted(data) {
            this.$store.commit('setUserUpvotes', data)

            this.fetchTopHeadlines()
            this.fetchHeadlineChangePage(this.currentPage)
        }
    },
    created() {
        this.fetchHeadlineChangeCount()
        this.fetchTopHeadlines()
        this.fetchHeadlineChangePage(0)
    },
    mounted() {
        //hours
        var seenAgo = (new Date().getTime() - new Date(this.$store.state.userData.lastSeen).getTime()) / 1000 / 60 / 60

        this.showGreetingOnMount = seenAgo > 3
        this.resolveAnimations(this.showGreetingOnMount)
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .section-first {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        height: 100%;
        padding: 35px 20vw 100px 20vw;
        background-color: #26A6A6;
    }

    .section-second {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        height: 100%;
        padding: 35px 0% 100px 0%;
    }

    .greet-title {
        padding: 150px 0;
    }

    .section-first-content {
        position: absolute;
        width: 85%;
    }

    .section-second-content {
        position: relative;
        width: 85%;
    }

    .section-second-title {
        color: #0B5B8C;
    }

    @media screen and (max-width: 960px) {
        .section-first {
            padding: 50px 7.5vw 100px 7.5vw;
        }

        .greet-title {
            padding: 100px 0;
        }
    }

    @media (max-width: 576px) {
        .section-first {
            padding: 25px 4vw 100px 4vw;
        }

        .greet-title {
            padding: 100px 0;
        }
    }

    .spacer {
        aspect-ratio: 900/200;
        width: 100%;
        background-repeat: no-repeat;
        background-position: center;
        background-size: cover;
    }

    .layer1 {
        background-image: url('../assets/layered-waves.svg')
    }

    .ml10 {
        position: relative;
        font-weight: 900;
        font-size: 4em;
    }

        .ml10 .text-wrapper {
            position: relative;
            display: inline-block;
            padding-top: 0.2em;
            padding-right: 0.05em;
            padding-bottom: 0.1em;
            overflow: hidden;
        }

    .ml7 {
        position: relative;
        font-weight: 900;
        font-size: 2.3em;
    }

        .ml7 .text-wrapper {
            position: relative;
            display: inline-block;
            padding-top: 0.2em;
            padding-right: 0.05em;
            padding-bottom: 0.1em;
            overflow: hidden;
        }

    @media screen and (max-width: 960px) {
        .ml7 {
            font-size: 2.2em;
        }
    }

    @media (max-width: 576px) {
        .ml7 {
            font-size: 2em;
        }
    }

    .masked {
        -webkit-mask-image: linear-gradient(to bottom, black 90%, transparent 100%);
        mask-image: linear-gradient(to bottom, black 90%, transparent 100%);
        height: 100px;
        overflow-x: hidden;
        overflow-y: auto;
    }

    .hide-scrollbar::-webkit-scrollbar {
        display: none;
    }

    .hide-scrollbar {
        -ms-overflow-style: none; /* IE and Edge */
        scrollbar-width: none; /* Firefox */
    }
</style>