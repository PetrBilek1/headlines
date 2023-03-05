import { createStore } from 'vuex'
import axios from 'axios';
import endpoints from './api/endpoints.js'

const store = createStore({
    state() {
        return {
            userData: null,
            userUpvotes: [],
            webSocket: null,
            homePage: {
                stopAnimating: false
            },
            articlesPage: {
                selectedPage: 0,
                searchPrompt: "",
                selectedSources: null
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
        },
        articlesPage(state) {
            return state.articlesPage
        },
        webSocket(state) {
            return state.webSocket
        }
    },
    mutations: {
        setUserUpvotes(state, userUpvotes) {
            state.userUpvotes = userUpvotes
        },
        setHomePage(state, homePage) {
            state.homePage = homePage
        },
        setArticlesPage(state, articlesPage) {
            state.articlesPage = articlesPage
        },
        setWebSocket(state, webSocket) {
            state.webSocket = webSocket
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
        connectWebSocket(context) {
            const webSocket = new WebSocket(endpoints.WebSocketServer.GetAddress())
            webSocket.onopen = () => {
                context.commit('setWebSocket', webSocket)
            }
        }
    }
})

export default store