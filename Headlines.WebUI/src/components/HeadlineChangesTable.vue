<template>
    <div>
        <HeadlineChangeRow v-for="headlineChange in headlineChanges"
                           v-bind:key="headlineChange.id"
                           v-bind:change="headlineChange"
                           v-on:upvoted="upvoted"
                           backgroundcolor="#79D9CF" 
                           :borderwidth="1" 
                           :usertoken="userToken"
                           :userupvotes="userUpvotes"
                           :showarticledetaillink="showarticledetaillink">
        </HeadlineChangeRow>

        <nav aria-label="pagination" class="mt-5">
            <ul class="pagination justify-content-center">
                <li :class="['page-item', 'cursor-pointer', currentPage <= 0 ? 'disabled' : '']" v-on:click="selectPage(currentPage - 1)">
                    <a class="page-link" aria-label="Předchozí">
                        <span aria-hidden="true">&laquo;</span>
                    </a>
                </li>
                <li :class="['page-item', 'cursor-pointer', page == currentPage ? 'active' : '']" v-for="page in pagination" v-bind:key="page" v-on:click="selectPage(page)"><a class="page-link">{{ page + 1 }}</a></li>
                <li :class="['page-item', 'cursor-pointer', currentPage >= Math.floor(headlineChangeCount / recordsPerPage) ? 'disabled' : '']" v-on:click="selectPage(currentPage + 1)">
                    <a class="page-link" aria-label="Další">
                        <span aria-hidden="true">&raquo;</span>
                    </a>
                </li>
            </ul>
        </nav>
    </div>   
</template>

<script>
import HeadlineChangeRow from './HeadlineChangeRow.vue'

export default {
    name: 'HeadlineChangesTable',
    components: {
        HeadlineChangeRow
    },
    data() {
        return {
            currentPage: 0,
            pagination: [0],
        }
    },
    props: {
        headlineChanges: {
            default() { return [] },
            type: Array
        },
        headlineChangeCount: {
            default: 0,
            type: Number
        },
        recordsPerPage: {
            default: 10,
            type: Number
        },
        userToken: {
            default: "",
            type: String
        },
        userUpvotes: {
            default() { return [] },
            type: Array
        },
        showarticledetaillink: {
            default: true,
            type: Boolean
        }
    },
    emits: ['upvoted', 'fetchheadlinechanges'],
    methods: {
        upvoted(data) {
            this.$emit('upvoted', data)
        },        
        selectPage(page) {
            var lastPage = this.getLastPage()

            page = page <= 0 ? 0 : page >= lastPage ? lastPage : page

            if (page == this.currentPage)
                return

            this.currentPage = page
            this.pagination = this.calculatePagination(page, lastPage)

            this.$emit('fetchheadlinechanges', page)
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
            return Math.floor(this.headlineChangeCount / this.recordsPerPage)
        },
    },
    beforeUpdate() {
        this.pagination = this.calculatePagination(this.currentPage, this.getLastPage())
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>