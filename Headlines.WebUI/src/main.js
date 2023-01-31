import { createApp } from 'vue'
import VueCookies from 'vue-cookies'
import App from './App.vue'
import router from './router'
import store from './store'

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/js/bootstrap.js'

//fontawesome
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { faLink, faLinkSlash, faThumbsUp, faList, faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons'

library.add(faLink, faLinkSlash, faThumbsUp, faList, faMagnifyingGlass)

createApp(App)
    .use(router)
    .use(VueCookies, { expires: '7d' })
    .use(store)
    .component('fai', FontAwesomeIcon)
    .mount('#app')