import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";

import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.VITE_APP_BASE_URL ?? import.meta.env.BASE_URL),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
      meta: { isPublic: true },
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      // route level code-splitting
      // this generates a separate chunk (ProfileView.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/account/ProfileView.vue"),
    },
    {
      name: "SignIn",
      path: "/sign-in",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/sign-out",
      component: () => import("./views/account/SignOutView.vue"),
    },
    // API Keys
    {
      name: "ApiKeyList",
      path: "/api-keys",
      component: () => import("./views/apiKeys/ApiKeyList.vue"),
    },
    {
      name: "ApiKeyEdit",
      path: "/api-keys/:id",
      component: () => import("./views/apiKeys/ApiKeyEdit.vue"),
    },
    // Configuration
    {
      name: "Configuration",
      path: "/configuration",
      component: () => import("./views/configuration/ConfigurationEdit.vue"),
    },
    // Languages
    {
      name: "LanguageList",
      path: "/languages",
      component: () => import("./views/languages/LanguageList.vue"),
    },
    {
      name: "LanguageEdit",
      path: "/languages/:id",
      component: () => import("./views/languages/LanguageEdit.vue"),
    },
    // Realms
    {
      name: "RealmList",
      path: "/realms",
      component: () => import("./views/realms/RealmList.vue"),
    },
    {
      name: "RealmEdit",
      path: "/realms/:id",
      component: () => import("./views/realms/RealmEdit.vue"),
    },
    // Roles
    {
      name: "RoleList",
      path: "/roles",
      component: () => import("./views/roles/RoleList.vue"),
    },
    {
      name: "RoleEdit",
      path: "/roles/:id",
      component: () => import("./views/roles/RoleEdit.vue"),
    },
    // Tokens
    {
      name: "Tokens",
      path: "/tokens",
      component: () => import("./views/TokenView.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      component: () => import("./views/NotFound.vue"),
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
