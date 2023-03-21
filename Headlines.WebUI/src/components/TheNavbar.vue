<template>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white box-shadow">
            <div class="container-fluid">
                <router-link to="/" class="navbar-brand">Titulkovač</router-link>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <router-link to="/" class="nav-link text-dark" :id="[currentRouteName === 'Home' ? 'nav-current' : '']">Titulky</router-link>
                        </li>
                        <li class="nav-item">
                            <router-link to="/articles" class="nav-link text-dark" :id="[currentRouteName === 'Articles' ? 'nav-current' : '']">Články</router-link>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
</template>

<script>
    import { useRoute } from 'vue-router'

    export default {
        name: 'TheNavbar',
        computed: {
            currentRouteName() {
                return useRoute().name
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .navbar {
        background-color: #1B818C !important;
        font-size: 22px;
    }

    .navbar-brand {
        color: #FFFFFF !important;
        font-size: 26px;
    }

        .navbar-brand:hover {
            color: #E8BE6D !important;
        }

    .nav-link {
        color: #FFFFFF !important;
    }

        .nav-link:hover {
            color: #E8BE6D !important;
        }

    .navbar-toggler {
        color: #FFFFFF55 !important;
        border-color: #FFFFFF55 !important;
    }

    .navbar-toggler-icon {
        filter: brightness(0) invert(1) opacity(1);
    }

    #nav-current {
        color: #E8BE6D !important;
    }
</style>