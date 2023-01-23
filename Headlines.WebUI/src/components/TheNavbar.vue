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
                            <router-link to="/" class="nav-link text-dark" asp-area="">Titulky</router-link>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark">Články - brzy</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
</template>

<script>
    export default {
        name: 'TheNavbar'
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .navbar {
        background-color: #1B818C !important;
    }

    .navbar-brand, .nav-link {
        color: #FFFFFF !important;
    }

    .navbar-toggler {
        color: #FFFFFF55 !important;
        border-color: #FFFFFF55 !important;
    }

    .navbar-toggler-icon {
        filter: brightness(0) invert(1) opacity(1);
    }
</style>