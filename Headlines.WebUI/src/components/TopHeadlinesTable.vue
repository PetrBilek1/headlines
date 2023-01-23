<template>
    <div>
        <HeadlineChangeRow v-for="(headlineChange, i) in headlineChanges"
                           v-bind:key="headlineChange.id"
                           v-bind:change="headlineChange"
                           v-bind:order="i + 1"
                           v-on:upvoted="upvoted"
                           :mountedanimdelay="startAnimDelay + i * 45"
                           :animateonmount="true"
                           :usertoken="userToken"
                           :userupvotes="userUpvotes">
        </HeadlineChangeRow>
    </div>   
</template>

<script>
import HeadlineChangeRow from './HeadlineChangeRow.vue'

export default {
    name: 'TopHeadlinesTable',
    components: {
        HeadlineChangeRow
    },    
    props: {
        headlineChanges: {
            default() { return [] },
            type: Array
        },
        startAnimDelay: {
            default: 0,
            type: Number
        },
        userToken: {
            default: "",
            type: String
        },
        userUpvotes: {
            default() { return [] },
            type: Array
        }
    },
    emits: ['upvoted'],
    methods: {
        upvoted(data) {
            this.$emit('upvoted', data)
        }
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>