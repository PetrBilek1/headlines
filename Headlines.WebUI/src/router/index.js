import { createWebHistory, createRouter } from "vue-router";
import HomePage from './../components/HomePage.vue'
import ArticlesPage from './../components/ArticlesPage.vue'
import ArticleDetailPage from './../components/ArticleDetailPage.vue'

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
    {
        path: "/articles/:id",
        name: "Article",
        component: ArticleDetailPage,
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;