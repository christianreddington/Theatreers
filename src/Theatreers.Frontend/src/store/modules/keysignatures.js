import keySignaturesData from '../../api/keysignatures'

// initial state
const state = {
  all: []
}

// getters
const getters = {}

// actions
const actions = {
  getAllKeySignatures ({ commit }) {
    keySignaturesData.getKeySignatures(keySignatures => {
      commit('setKeySignatures', keySignatures)
    })
  }
}

// mutations
const mutations = {
  setKeySignatures (state, keySignatures) {
    state.all = keySignatures
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}