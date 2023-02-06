<template>
    <section class="section-first">
        <h3 class="color-yellow" v-if="article">
            <b><i>{{ article.currentTitle }}</i></b>
        </h3>
        <fai v-if="!article" :icon="['fas', 'spinner']" size="3x" :style="{ color: 'white' }" spin></fai>
    </section>
    <section class="spacer layer1"></section>
    <section class="section-second">
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
                                  :userUpvotes="userUpvotes" />
        </div>
    </section>
</template>

<script>
import axios from 'axios'
import endpoints from './../api/endpoints.js'
import HeadlineChangesTable from './HeadlineChangesTable.vue'

export default {
    name: 'ArticleDetailPage',
    data() {
        return {
            article: null,
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
        }
    },
    methods: {
        async fetchArticleById(articleId) {
            var response = await axios.get(endpoints.Articles.GetById(articleId))

            this.article = response.data.article
        },
        async fetchHeadlineChangePage(page) {
            var response = await axios.get(endpoints.HeadlineChanges.GetByArticleIdSkipTake(this.$route.params.id, page * this.headlineChangesPerPage, this.headlineChangesPerPage))

            response.data.headlineChanges.forEach(x => x.article = this.article)

            this.headlineChangesPage = response.data.headlineChanges
            this.headlineChangesCount = response.data.totalCount
            this.currentPage = page
        },
        async upvoted(data) {
            this.$store.commit('setUserUpvotes', data)

            await this.fetchHeadlineChangePage(this.currentPage)
        }
    },
    created() {

    },
    async mounted() {
        await this.fetchArticleById(this.$route.params.id)
        await this.fetchHeadlineChangePage(this.currentPage)
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

    .section-second {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        height: 100%;
        padding: 35px 0% 100px 0%;
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
</style>