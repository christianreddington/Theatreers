import Vue from 'vue'
import Vuex from 'vuex'
import alerts from './modules/alerts'
import breadcrumbs from './modules/breadcrumbs'
import notes from './modules/notes'

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    alerts,
    breadcrumbs,
    notes
  }
})