import vocalTypeData from '../../api/vocaltype'

// initial state
const state = {
  all: []
}

// getters
const getters = {}

// actions
const actions = {
  getAllVocalTypes ({ commit }) {
    vocalTypeData.getVocalTypes(vocalType => {
      commit('setvocalTypes', vocalType)
    })
  }
}

// mutations
const mutations = {
  setvocalTypes (state, vocalType) {
    state.all = vocalType
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}