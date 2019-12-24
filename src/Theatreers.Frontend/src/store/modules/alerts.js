import alertsMock from '../../api/alert'

// initial state
const state = {
  alerts: []
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
    if (state.alerts == null){
      state.alerts = [];
    }
    alerts.forEach(function (item) {
      item.id = state.alerts.length + 1;
      state.alerts.push(item);
    });
  },
  removeAlert (state, alert) {
    var index = state.alerts.map(x => {
      return x.id;
    }).indexOf(alert);
    state.alerts.splice(index, 1)
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}