<template>
    <section class="section-first">
        <h1 class="color-yellow mb-3 mb-lg-5">
            <b>ČLÁNKY</b>
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
                <input class="form-control form-control-lg" type="text" placeholder="Titulek" v-model="sourcesTitle">
            </div>
            <button class="btn btn-lg btn-light" style="width: 100px; margin-left: 10px;" type="button">
                <fai :icon="['fas', 'magnifying-glass']"></fai>
            </button>
        </div>       
    </section>
    <section class="spacer layer1"></section>
</template>

<script>
import axios from 'axios'
import endpoints from './../api/endpoints.js'

export default {
    name: 'ArticlesPage',
    data() {
        return {
            allSourcesSelected: false,
            articleSources: [],
            sourcesTitle: "",
            selectedPage: 0,
            articlePage: []
        }
    },
    components: {
        
    },
    computed: {
        
    },
    methods: {
        fetchArticleSources() {
            axios
                .get(endpoints.ArticleSources.GetAll())
                .then(response => {
                    this.articleSources = []
                    response.data.articleSources.forEach(x => this.articleSources.push({source: x, isSelected: true}))

                    this.setAllSourcesSelected()
                })
        },
        toggleSelectArticleSource(id) {
            var sourcePair = this.articleSources.find(x => x.source.id === id)

            sourcePair.isSelected = !sourcePair.isSelected

            this.setAllSourcesSelected()
        },
        toggleAllSources(){
            var wantedState = !this.allSourcesSelected

            this.articleSources.forEach(x => x.isSelected = wantedState)

            this.setAllSourcesSelected()
        },
        setAllSourcesSelected() {
            this.allSourcesSelected = this.articleSources.every(x => x.isSelected)
        },
        fetchArticlePage(page) {

        }
    },
    created() {
        
    },
    mounted() {
        this.fetchArticleSources()
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