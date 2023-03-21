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
            <h1 id="top-title" class="ml7">
                <span class="color-yellow text-wrapper">
                    <span class="letters">Nejlepší změny titulků</span>
                    &nbsp;
                    <fai v-if="stopAnimating && topHeadlineChanges.length == 0" :icon="['fas', 'spinner']" :style="{ color: 'white' }" spin></fai>
                </span>
            </h1>
            <div id="top-table" class="masked hide-scrollbar" style="height: 85%;">                
                <TopHeadlinesTable v-on:upvoted="upvoted"
                                   :headlineChanges="topHeadlineChanges"
                                   :startAnimDelay="topHeadlineChangesStartAnimDelay"
                                   :userToken="userData.userToken"
                                   :userUpvotes="userUpvotes"
                                   :animateonmount="!stopAnimating" />
            </div>            
        </div>
    </section>
    <section class="spacer layer1">
        <div class="down-button">
            <span @click="scrollDown()"><b>DOLŮ</b></span>
        </div>
    </section>

    <section id="section-second" class="section-second">
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
            stopAnimating: false,
            topHeadlineChangesStartAnimDelay: 0,
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
                    this.currentPage = page
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
            if (showGreetings) {
                this.topHeadlineChangesStartAnimDelay = 3300
                this.animateGreetingTitle()
            }
        },
        animateGreetingTitle() {
            let textWrapper = document.querySelector('.ml10 .letters')
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
                }).add({
                    targets: ['#top-title', '#top-table', ".down-button"],
                    opacity: [0, 1],
                    duration: 750,
                    easing: "easeOutExpo"
                })
        },
        upvoted(data) {
            this.$store.commit('setUserUpvotes', data)

            this.fetchTopHeadlines()
            this.fetchHeadlineChangePage(this.currentPage)
        },
        scrollDown() {
            document.getElementById("section-second").scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" })
        }
    },
    created() {
        this.fetchHeadlineChangeCount()
        this.fetchTopHeadlines()
        this.fetchHeadlineChangePage(0)
    },
    mounted() {
        let homePageStore = this.$store.state.homePage
        this.stopAnimating = homePageStore.stopAnimating
        if (homePageStore.stopAnimating)
            return

        //hours
        let seenAgo = (new Date().getTime() - new Date(this.$store.state.userData.lastSeen).getTime()) / 1000 / 60 / 60

        this.showGreetingOnMount = seenAgo > 3
        this.resolveAnimations(this.showGreetingOnMount)
        homePageStore.stopAnimating = true

        this.$store.commit("setHomePage", homePageStore)
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

    .down-button {    
        position: static;
        width: 100%;
        height: 70%;
        align-content: center;
        display: flex;
        justify-content: center;
        align-items: center;
    }

        .down-button span {
            background-color: white;
            border-radius: 25px;
            padding: 10px 50px;
            cursor: pointer;
            transition: 0.4s;
            z-index: 100;
        }

            .down-button span:hover {
                background-color: #E8BE6D;                
            }
</style>