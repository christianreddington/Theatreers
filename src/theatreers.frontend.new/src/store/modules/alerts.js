import alertsMock from '../../api/alert'

// initial state
const state = {
  all: []
}

// getters
const getters = {}

// actions
const actions = {
  getAllAlerts ({ commit }) {
    alertsMock.getAlerts(alerts => {
      commit('setAlerts', alerts)
    })
  }
}

// mutations
const mutations = {
  setAlerts (state, alerts) {
    state.all = alerts
  },

  decrementAlertInventory (state, { id }) {
    const alert = state.all.find(alert => alert.id === id)
    alert.inventory--
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}