import notesData from '../../api/notes'

// initial state
const state = {
  all: []
}

// getters
const getters = {}

// actions
const actions = {
  getAllNotes ({ commit }) {
    notesData.getNotes(notes => {
      commit('setNotes', notes)
    })
  }
}

// mutations
const mutations = {
  setNotes (state, notes) {
    state.all = notes
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}