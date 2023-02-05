<template>
    <div>

    </div>
</template>

<script>
import axios from 'axios'
import endpoints from './../api/endpoints.js'

export default {
    name: 'ArticleDetailPage',
    data() {
        return {
            article: null,
            headlineChangesPerPage: 10,
            headlineChangesPage: [],
            headlineChangesCount: 0
        }
    },
    components: {

    },
    computed: {

    },
    methods: {
        fetchHeadlineChangePageByArticleId(articleId, page) {
            axios
                .get(endpoints.HeadlineChanges.GetByArticleIdSkipTake(articleId, page * this.headlineChangesPerPage, this.headlineChangesPerPage))
                .then(response => {
                    this.headlineChangesPage = response.data.headlineChanges
                    this.headlineChangesCount = response.data.totalCount
                    console.log(response.data)
                })
        },
    },
    created() {

    },
    mounted() {
        this.fetchHeadlineChangePageByArticleId(this.$route.params.id, 0)
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    
</style>