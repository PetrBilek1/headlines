import { createStore } from 'vuex'
import axios from 'axios';
import endpoints from './api/endpoints.js'

const store = createStore({
    state() {
        return {
            userData: null,
            userUpvotes: [],
            homePage: {
                stopAnimating: false
            },
            articlesPage: {

            }
        }
    },
    getters: {
        userData(state) {
            return state.userData
        },
        userUpvotes(state) {
            return state.userUpvotes
        },
        homePage(state) {
            return state.homePage
        }
    },
    mutations: {
        setUserUpvotes(state, userUpvotes) {
            state.userUpvotes = userUpvotes
        },
        setHomePage(state, homePage) {
            state.homePage = homePage
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