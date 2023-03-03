<template>
    <TheNavbar />
    <router-view />
</template>

<script>
    import TheNavbar from './components/TheNavbar.vue'
    import { mapActions } from 'vuex'

    export default {
        name: 'App',
        components: {
            TheNavbar
        },
        methods: {
            ...mapActions(['connectWebSocket']),
            getUserDataCookie() {
                let userData = this.$cookies.get("user_data")

                if (userData == null) {
                    userData = {
                        lastSeen: new Date(-8640000000000),
                        nowSeen: new Date(-8640000000000),
                        userToken: this.createGuid()
                    }
                }

                return userData
            },
            setUserDataCookie(userData) {
                this.$cookies.set("user_data", userData, '6m')
            },
            createGuid() {
                function _p8(s) {
                    var p = (Math.random().toString(16) + "000000000").substr(2, 8);
                    return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
                }

                return _p8() + _p8(true) + _p8(true) + _p8();
            },
        },
        created() {
            var userData = this.getUserDataCookie()
            this.$store.state.userData = userData
            this.$store.dispatch('fetchUserUpvotes', userData.userToken) 
            this.connectWebSocket()
        },
        mounted() {
            this.$store.getters.userData.lastSeen = this.$store.getters.userData.nowSeen
            this.$store.getters.userData.nowSeen = new Date()
            this.setUserDataCookie(this.$store.getters.userData)
        }
    }
</script>

<style>
    #app {
        font-family: Avenir, Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        color: #2c3e50;
    }

    .single-line-text {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .color-white {
        color: white;
    }

    .color-yellow {
        color: #E8BE6D;
    }

    .cursor-pointer {
        cursor: pointer;
    }

    .cursor-default {
        cursor: default;
    }  
</style>
