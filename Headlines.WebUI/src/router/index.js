import { createWebHistory, createRouter } from "vue-router";
import HomePage from './../components/HomePage.vue'
import ArticlesPage from './../components/ArticlesPage.vue'

const routes = [
    {
        path: "/",
        name: "Home",
        component: HomePage,
    },
    {
        path: "/articles",
        name: "Articles",
        component: ArticlesPage,
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;