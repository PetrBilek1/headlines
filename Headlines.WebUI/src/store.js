import { createStore } from 'vuex'
import axios from 'axios';
import endpoints from './api/endpoints.js'

const store = createStore({
    state() {
        return {
            userData: null,
            userUpvotes: [],
        }
    },
    getters: {
        userData(state) {
            return state.userData
        },
        userUpvotes(state) {
            return state.userUpvotes
        }
    },
    mutations: {
        setUserUpvotes(state, userUpvotes) {
            state.userUpvotes = userUpvotes
        }
    },
    actions: {
        fetchUserUpvotes(context, userToken) {            
            axios
                .get(endpoints.UserUpvotes.Get(userToken))
                .then(response => {
                    context.commit('setUserUpvotes', response.data.upvotes)
                })
        },
    }
})

export default store