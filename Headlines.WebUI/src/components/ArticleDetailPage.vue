<template>
    <section class="section-first">        
        <header class="color-yellow" v-if="article">
            <span class="color-white w-100 mb-3" style="text-align: left; font-size: 24px;" v-if="article">
                <b>{{ article.source.name }}</b>
            </span>
            <br>
            <span class="cursor-pointer" style="font-size: 32px;" @click="redirect(article.link)"><b><i>{{ article.currentTitle }}</i></b></span>
            <br>
            <div class="color-white w-100 mt-3" style="text-align: right; font-size: 18px;" v-if="article">
                <b>Publikováno {{ getLocalTimeString(article.published) }}</b>
            </div>
        </header>
        <fai v-if="!article" :icon="['fas', 'spinner']" size="3x" :style="{ color: 'white' }" spin></fai>
    </section>
    <section class="spacer layer1"></section>
    <section class="section-navigation">
        <div class="btn-group" role="group">
            <input type="radio" class="btn-check" name="btnradio" :checked="shownSection == 3">
            <label class="btn btn-outline-primary" @click="shownSection = 3">Článek</label>

            <input type="radio" class="btn-check" name="btnradio" :checked="shownSection == 2">
            <label class="btn btn-outline-primary" @click="shownSection = 2">Titulky</label>
        </div>
    </section>
    <section v-if="shownSection == 2" class="section-second">
        <div v-if="headlineChangesPage.length == 0">
            <h3>Titulek nebyl měněn</h3>
        </div>
        <div v-else class="section-second-content">
            <h3 class="section-second-title">Změny titulků</h3>
            <HeadlineChangesTable v-on:upvoted="upvoted"
                                  v-on:fetchheadlinechanges="fetchHeadlineChangePage"
                                  :headlineChanges="headlineChangesPage"
                                  :headlineChangeCount="headlineChangesCount"
                                  :recordsPerPage="headlineChangesPerPage"
                                  :userToken="userData.userToken"
                                  :userUpvotes="userUpvotes"
                                  :showarticledetaillink="false"/>
        </div>
    </section>
    <section v-if="shownSection == 3" class="section-third">
        <div v-if="articleDetailScrapeUnsuccessful" class="align-text-center mb-3">
            <h3>
                Získání článku se nezdařilo :(&nbsp;
            </h3>
            <button class="btn btn-primary btn-sm" @click="requestDetailScrape($route.params.id)" :disabled="articleDetailScrapeRequested">
                Zkusit znovu
            </button>
        </div>
        <div v-if="articleDetail">
            <div v-if="articleDetail.isPaywalled" style="font-size: 22px;">
                <fai :icon="['fas', 'sack-dollar']"></fai>
                Placený článek
            </div>
            <h1>
                <b>{{ articleDetail.title }}</b>
            </h1>
            <p v-for="(paragraph, i) in articleDetail.paragraphs" v-bind:key="i">
                {{ paragraph }}
            </p>
            <div v-if="articleDetail.tags.length > 0" class="mt-3">
                <h5>Tagy:</h5>
                <button v-for="(tag, i) in articleDetail.tags" v-bind:key="i" type="button" class="btn btn-primary btn-sm tag">
                    {{ tag }}
                </button>
            </div>
        </div>
        <div v-if="article" class="align-text-center">
            <fai v-if="!articleDetail && article.source.scrapingSupported" :icon="['fas', 'spinner']" size="2x" :style="{ color: 'black' }" spin></fai>
            <div v-if="!article.source.scrapingSupported">
                <h3>U tohoto serveru obsah článků zatím nepodporujeme :(</h3>
            </div>
        </div>
    </section>
</template>

<script>
import { mapActions } from 'vuex'
import axios from 'axios'
import moment from 'moment'
import endpoints from './../api/endpoints.js'
import HeadlineChangesTable from './HeadlineChangesTable.vue'

export default {
    name: 'ArticleDetailPage',
    data() {
        return {
            article: null,
            articleDetailScrapeRequested: false,
            articleDetailScrapeUnsuccessful: false,
            articleDetail: null,
            shownSection: 3,
            currentPage: 0,
            headlineChangesPerPage: 10,
            headlineChangesPage: [],
            headlineChangesCount: 0
        }
    },
    components: {
        HeadlineChangesTable
    },
    computed: {
        userData() {
            return this.$store.getters.userData
        },
        userUpvotes() {
            return this.$store.getters.userUpvotes
        },
        webSocket() {
            return this.$store.getters.webSocket
        }
    },
    methods: {
        ...mapActions(['connectWebSocket']),
        async fetchArticleById(articleId) {
            const response = await axios.get(endpoints.Articles.GetById(articleId))

            this.article = response.data.article
        },
        async fetchArticleDetailById(articleId) {
            const response = await axios.get(endpoints.Articles.GetDetailById(articleId))

            this.articleDetail = response.data.detail
        },
        async fetchHeadlineChangePage(page) {
            const response = await axios.get(endpoints.Articles.GetHeadlineChangesByArticleId(this.$route.params.id, page * this.headlineChangesPerPage, this.headlineChangesPerPage))

            response.data.headlineChanges.forEach(x => x.article = this.article)

            this.headlineChangesPage = response.data.headlineChanges
            this.headlineChangesCount = response.data.totalCount
            this.currentPage = page
        },
        async upvoted(data) {
            this.$store.commit('setUserUpvotes', data)

            await this.fetchHeadlineChangePage(this.currentPage)
        },
        async requestDetailScrape(articleId) {
            if (this.articleDetailScrapeRequested)
                return

            this.articleDetailScrapeRequested = true
            await axios.post(endpoints.Articles.RequestDetailScrape(), {
                articleId: articleId
            })

        },
        getLocalTimeString(dateTimeUTC) {
            const date = new Date(dateTimeUTC + '+00:00')

            return moment(date).format("HH:mm DD.MM.YYYY")
        },
        redirect(url) {
            if (url.length <= 0)
                return

            window.open(url)
        },
        connectWebSocketIfNotAlready() {
            if (this.webSocket == null || this.webSocket.readyState == WebSocket.CLOSED) {
                console.log("WebSocket connection re-established.")
                this.connectWebSocket()
            }
        }
    },
    created() {

    },
    async mounted() {
        await this.fetchArticleById(this.$route.params.id)
        await this.fetchHeadlineChangePage(this.currentPage)
        await this.fetchArticleDetailById(this.$route.params.id)

        if (!this.article.source.scrapingSupported)
            return
        
        this.connectWebSocketIfNotAlready()

        this.webSocket.onmessage = (event) => { 
            const data = JSON.parse(event.data)
            if (data.messageType === "article-detail-scraped" && data.articleId == this.$route.params.id) {
                if (data.wasSuccessful) {
                    this.articleDetail = data.detail
                }
                setTimeout(() => { this.articleDetailScrapeRequested = false }, 1000)
                this.articleDetailScrapeUnsuccessful = !data.wasSuccessful
            }
        }
        this.webSocket.send(endpoints.WebSocketServer.Messages.ListenToArticleDetailScrape(this.article.id))

        if (this.articleDetail == null) {
            await this.requestDetailScrape(this.$route.params.id)
        }
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
        padding: 30px 5vw 0 5vw;
        background-color: #26A6A6;
    }

    .section-navigation {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        height: 100%;
    }

    .section-second {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        height: 100%;
        padding: 35px 0% 100px 0%;
    }

    .section-third {
        position: relative;
        display: flex;
        flex-direction: column;
        height: 100%;
        padding: 35px 15vw 100px 15vw;
    }

    .align-text-center {
        text-align: center;
        width: 100%;
    }

    @media screen and (max-width: 960px) {
        .section-third {
            padding: 35px 5vw 100px 5vw;
        }
    }

    .section-third h1 {
        margin-bottom: 20px;
    }

    .section-second-content {
        position: relative;
        width: 85%;
    }

    .spacer {
        aspect-ratio: 900/100;
        width: 100%;
        background-repeat: no-repeat;
        background-position: center;
        background-size: cover;
    }

    .layer1 {
        background-image: url('../assets/layered-peaks.svg')
    }

    .tag {
        margin: 3px;
    }
</style>