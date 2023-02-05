<template>
    <div>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th scope="col">Zdroj</th>
                    <th scope="col">Titulek</th>
                    <th scope="col" class="published-col">Publikováno</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="article in articles" v-bind:key="article.id">
                    <th scope="row" class="source-col single-line-text">{{ getSourceName(article.sourceId) }}</th>
                    <td>{{ article.currentTitle }}</td>
                    <td class="published-col">{{ getLocalTimeString(article.published) }}</td>
                    <td>
                        <a :href="article.link"><fai :icon="['fas', 'link']"></fai></a>
                        <router-link :to="{ name: 'Article', params: { id: article.id } }">
                            <fai :icon="['fas', 'circle-info']"></fai>
                        </router-link>
                    </td>
                </tr>
            </tbody>            
        </table>

        <nav aria-label="pagination" class="mt-5">
            <ul class="pagination justify-content-center">
                <li :class="['page-item', 'cursor-pointer', currentPage <= 0 ? 'disabled' : '']" v-on:click="selectPage(currentPage - 1)">
                    <a class="page-link" aria-label="Předchozí">
                        <span aria-hidden="true">&laquo;</span>
                    </a>
                </li>
                <li :class="['page-item', 'cursor-pointer', page == currentPage ? 'active' : '']" v-for="page in pagination" v-bind:key="page" v-on:click="selectPage(page)"><a class="page-link">{{ page + 1 }}</a></li>
                <li :class="['page-item', 'cursor-pointer', currentPage >= Math.floor(articlesCount / recordsPerPage) ? 'disabled' : '']" v-on:click="selectPage(currentPage + 1)">
                    <a class="page-link" aria-label="Další">
                        <span aria-hidden="true">&raquo;</span>
                    </a>
                </li>
            </ul>
        </nav>
    </div>
</template>

<script>
import moment from 'moment'

export default {
    name: 'ArticlesTable',
    components: {
    },
    data() {
        return {
            currentPage: 0,
            pagination: [0],
        }
    },
    props: {
        articles: {
            default() { return [] },
            type: Array
        },
        articlesCount: {
            default: 0,
            type: Number
        },
        articleSources: {
            default() { return [] },
            type: Array
        },
        recordsPerPage: {
            default: 10,
            type: Number
        }
    },
    emits: ['fetcharticles'],
    methods: {
        getSourceName(sourceId) {
            return this.articleSources.find(x => x.source.id == sourceId).source.name
        },
        selectPage(page) {
            var lastPage = this.getLastPage()

            page = page <= 0 ? 0 : page >= lastPage ? lastPage : page

            if (page == this.currentPage)
                return

            this.currentPage = page
            this.pagination = this.calculatePagination(page, lastPage)

            this.$emit('fetcharticles', page)
        },
        calculatePagination(currentPage, lastPage) {
            var pages = []

            if (lastPage < 5) {
                for (let i = 0; i <= lastPage; i++) {
                    pages.push(i)
                }
            }
            else if (currentPage < 2) {
                pages = [0, 1, 2, 3, lastPage]
            }
            else if (lastPage - currentPage < 2) {
                pages = [0, lastPage - 3, lastPage - 2, lastPage - 1, lastPage]
            }
            else {
                pages.push(0)
                for (let i = currentPage - 1; i < currentPage + 2; i++) {
                    pages.push(i)
                }
                pages.push(lastPage)
            }

            return pages
        },
        getLastPage() {
            return Math.floor(this.articlesCount / this.recordsPerPage)
        },
        getLocalTimeString(dateTimeUTC) {
            const date = new Date(dateTimeUTC + '+00:00')

            return moment(date).format("HH:mm DD.MM.YYYY")
        },
    },
    beforeUpdate() {
        this.pagination = this.calculatePagination(this.currentPage, this.getLastPage())
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .source-col {
        max-width: 140px;
    }

    @media screen and (max-width: 960px) {
        .source-col {
            max-width: 80px;
        }
    }    

    @media (max-width: 576px) {
        .published-col {
            display: none;
        }
    }

</style>