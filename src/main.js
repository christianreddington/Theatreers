import Vue from 'vue'
import './plugins/bootstrap-vue'
import App from './App.vue'
import router from './router'
import store from './store'
import AuthService from './msal'

Vue.config.productionTip = false
Vue.prototype.$AuthService = new AuthService()

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
