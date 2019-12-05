import breadcrumbMock from '../../api/breadcrumb'

// initial state
const state = {
  all: []
}

// getters
const getters = {}

// actions
const actions = {
  getAllBreadcrumbs ({ commit }) {
    breadcrumbMock.getBreadcrumbs(breadcrumbs => {
      commit('setBreadcrumbs', breadcrumbs)
    })
  }
}

// mutations
const mutations = {
  setBreadcrumbs (state, breadcrumbs) {
    state.all = breadcrumbs
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}