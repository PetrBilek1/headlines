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
                    <span class="page-link" aria-label="Předchozí">
                        <span aria-hidden="true">&lsaquo;</span>
                    </span>
                </li>
                <li :class="['page-item', 'cursor-pointer', page == currentPage ? 'active' : '']" v-for="page in pagination" v-bind:key="page" v-on:click="selectPage(page)"><span class="page-link">{{ page + 1 }}</span></li>
                <li :class="['page-item', 'cursor-pointer', currentPage >= Math.floor((headlineChangeCount - 1) / recordsPerPage) ? 'disabled' : '']" v-on:click="selectPage(currentPage + 1)">
                    <span class="page-link" aria-label="Další">
                        <span aria-hidden="true">&rsaquo;</span>
                    </span>
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
            let checkedPage = ensurePageIsInLimits(page, this.getLastPage())

            if (checkedPage == this.currentPage)
                return

            this.currentPage = checkedPage
            this.pagination = this.calculatePagination(checkedPage, this.getLastPage())

            this.$emit('fetchheadlinechanges', checkedPage)

            function ensurePageIsInLimits(pageToCheck, maxPage) {
                let checked = pageToCheck

                checked = checked >= maxPage ? maxPage : checked
                checked = checked <= 0 ? 0 : checked

                return checked
            }
        },
        calculatePagination(currentPage, lastPage) {
            let pages = []

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
            return Math.floor((this.headlineChangeCount - 1) / this.recordsPerPage)
        },
    },
    beforeUpdate() {
        this.pagination = this.calculatePagination(this.currentPage, this.getLastPage())
    }
}
</script>