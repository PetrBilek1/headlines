<template>
    <section class="section-first" @keyup.enter="fetchArticlePage(selectedPage)">
        <h1 class="color-yellow text-wrapper mb-3 mb-lg-5">
            <b>Články</b>
        </h1>
        <div class="d-flex filters-width">
            <div>
                <div class="dropdown">
                    <button class="btn btn-lg btn-light dropdown-toggle" type="button" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                        <fai :icon="['fas', 'list']"></fai>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end p-2">
                        <li>
                            &nbsp;
                            <input class="form-check-input" type="checkbox" value="" name="all-sources" v-model="allSourcesSelected" @click="toggleAllSources">
                            &nbsp;
                            <label class="form-check-label"><b>Vše</b></label>
                        </li>
                        <li><hr class="dropdown-divider"></li>
                        <li class="single-line-text pb-1" v-for="sourcePair in articleSources" :key="sourcePair.source.name">
                            &nbsp;
                            <input class="form-check-input" type="checkbox" value="" :name="sourcePair.source.id" v-model="sourcePair.isSelected" @click="toggleSelectArticleSource(sourcePair.source.id)">
                            &nbsp;
                            <label class="form-check-label">
                                {{ sourcePair.source.name }}
                            </label>
                        </li>
                    </ul>
                </div>
            </div>
            <div style="width: 100%; margin-left: 10px;">
                <input class="form-control form-control-lg" type="text" placeholder="Titulek" v-model="searchPrompt">
            </div>
            <button class="btn btn-lg btn-light" style="width: 100px; margin-left: 10px;" type="button" @click="fetchArticlePage(selectedPage)">
                <fai :icon="['fas', 'magnifying-glass']"></fai>
            </button>
        </div>       
    </section>
    <section class="spacer layer1"></section>
    <section class="section-second">
        <ArticlesTable v-on:fetcharticles="fetchArticlePage"
                       :articles="articlePage"
                       :articlesCount="articlesCount"
                       :articleSources="articleSources"
                       :recordsPerPage="articlesPerPage"
                       :startPage="startPage">
        </ArticlesTable>
    </section>
</template>

<script>
import axios from 'axios'
import endpoints from './../api/endpoints.js'
import ArticlesTable from './ArticlesTable.vue'


export default {
    name: 'ArticlesPage',
    data() {
        return {
            allSourcesSelected: false,
            articleSources: [],
            searchPrompt: "",
            articlesPerPage: 10,
            selectedPage: 0,
            startPage: 0,
            articlePage: [],
            articlesCount: 0
        }
    },
    components: {
        ArticlesTable
    },
    computed: {
        
    },
    methods: {
        async fetchArticleSources() {
            let response = await axios.get(endpoints.ArticleSources.GetAll())

            this.articleSources = []
            response.data.articleSources.forEach(x => this.articleSources.push({ source: x, isSelected: true }))

            this.setAllSourcesSelected()
        },
        async fetchArticlePage(page) {
            let response = await axios.post(endpoints.Articles.Search(), {
                    skip: page * this.articlesPerPage,
                    take: this.articlesPerPage,
                    searchPrompt: this.searchPrompt,
                    articleSources: this.getSelectedSourcesArray()
            })

            this.articlePage = response.data.articles
            this.articlesCount = response.data.matchesFiltersCount
            this.selectedPage = page

            this.setArticlesPageState()
        },
        toggleSelectArticleSource(id) {
            let sourcePair = this.articleSources.find(x => x.source.id === id)

            sourcePair.isSelected = !sourcePair.isSelected

            this.setAllSourcesSelected()
        },
        toggleAllSources(){
            let wantedState = !this.allSourcesSelected

            this.articleSources.forEach(x => x.isSelected = wantedState)

            this.setAllSourcesSelected()
        },
        setAllSourcesSelected() {
            this.allSourcesSelected = this.articleSources.every(x => x.isSelected)
        },
        getSelectedSourcesArray() {
            let selected = []

            this.articleSources.forEach(x => {
                if (x.isSelected) {
                    selected.push(x.source.id)
                }
            })

            return this.articleSources.length <= 0
                ? null
                : selected
        },
        setArticlesPageState() {
            let articlesPageState = this.$store.state.articlesPage

            let selectedSources = []
            this.articleSources.forEach(x => {
                if (!x.isSelected)
                    return

                selectedSources.push(x.source.id)
            })

            articlesPageState.selectedPage = this.selectedPage
            articlesPageState.searchPrompt = this.searchPrompt
            articlesPageState.selectedSources = selectedSources

            this.$store.commit("setArticlesPage", articlesPageState)
        }
    },
    created() {
        
    },
    async beforeMount() {
        let articlesPageState = this.$store.state.articlesPage

        this.selectedPage = articlesPageState.selectedPage
        this.startPage = this.selectedPage
        this.searchPrompt = articlesPageState.searchPrompt

        await this.fetchArticleSources()       

        if (articlesPageState.selectedSources != null) {
            this.articleSources.forEach(x => {
                x.isSelected = articlesPageState.selectedSources.includes(x.source.id)
            })

            this.setAllSourcesSelected()
        }       

        await this.fetchArticlePage(this.selectedPage)
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
        padding: 0 5vw 0 5vw;
    }

    @media screen and (max-width: 960px) {
        .section-second {
            padding: 0;
        }
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

    .filters-width {
        width: 60%;
    }

    @media screen and (max-width: 960px) {
        .filters-width {
            width: 100%;
        }
    }
</style>