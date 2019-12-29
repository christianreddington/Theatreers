import Vue from 'vue'
import Vuex from 'vuex'
import alerts from './modules/alerts'
import breadcrumbs from './modules/breadcrumbs'
import keysignatures from './modules/keysignatures'
import notes from './modules/notes'
import vocaltypes from './modules/vocaltype'
import createPersistedState from 'vuex-persistedstate';

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    alerts,
    breadcrumbs,
    keysignatures,
    notes,
    vocaltypes
  },
  plugins: [createPersistedState()]
})