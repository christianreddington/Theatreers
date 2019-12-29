import "@babel/polyfill";
import "mutationobserver-shim";
import Vue from "vue";
import "./plugins/bootstrap-vue";
import "./plugins/bootstrap-vue";
import App from "./App.vue";
import store from "./store";
import AuthService from "./msal";
import router from "./router";
import InputTag from 'vue-input-tag'

Vue.use(router)
Vue.prototype.$AuthService = new AuthService();
Vue.config.productionTip = false;
Vue.component('input-tag', InputTag)

new Vue({
  router,
  store,
  render: h => h(App),
}).$mount("#app");
