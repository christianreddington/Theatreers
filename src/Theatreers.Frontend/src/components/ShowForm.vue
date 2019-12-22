<template>
  <div>
    <b-form @submit="onSubmit">
      <b-form-group id="label-showName" label="Show Name:" label-for="showName" >
        <b-form-input id="input-showName" v-model="value.showName" type="text" required placeholder="Enter the name of the show" />
      </b-form-group>

      <b-form-group id="label-description" label="Description" label-for="input-description">
        <b-form-textarea id="input-description" v-model="value.description" placeholder="Enter a synposis of the show and any other important details." rows="3" max-rows="6" />
      </b-form-group>

      <b-form-group id="label-author" label="Author:" label-for="input-author" >
        <b-form-input id="input-author" v-model="value.author" type="text" required placeholder="Name of the Author" />
      </b-form-group>

      <b-form-group id="label-composer" label="Composer:" label-for="input-composer">
        <b-form-input id="input-composer" v-model="value.composer" type="text" required placeholder="Name of the Composer" />
      </b-form-group>

      <h2>Songs <b-button @click="addSong" variant="success" >+</b-button></h2>
      <div v-for="(song, index) in value.songs" :key="`song-${index}`">
        <div class="row">
          <div class="col-3">
            <b-form-group id="label-songname" label="Name:" label-for="input-songname-name">
              <b-form-input id="input-songname" v-model="song.name" float-label="Country Code" />
            </b-form-group>
          </div>
          <div class="col-5">
            <b-form-group id="label-songdescription" label="Description:" label-for="input-description">
              <b-form-textarea id="input-description" v-model="song.description" placeholder="Enter a synposis of the show and any other important details." rows="3" max-rows="6" />
            </b-form-group>
          </div>
          <div class="col-1">
            <b-form-group id="label-song-low" label="Low:" label-for="input-song-low">
              <b-form-select id="input-song-low" v-model="song.low" float-label="Lowest note of song" :options="notes" />
            </b-form-group>
          </div>
          <div class="col-1">
            <b-form-group id="label-song-high" label="High:" label-for="input-song-high">
              <b-form-select id="input-song-high" v-model="song.high" float-label="Highest note of song" :options="notes" />
            </b-form-group>
          </div>
          <div class="col-1">
            <b-button @click="removeSong(index)" variant="danger">-</b-button>
          </div>
        </div>
      </div>

      <h2>Characters <b-button @click="addCharacter" variant="success" >+</b-button></h2>
      <div v-for="(character, index) in value.characters" :key="`character-${index}`">
        <div class="row">
          <div class="col-3">
            <b-form-group id="label-charactername" label="Name:" label-for="input-character-name">
              <b-form-input id="input-charactername" v-model="character.name" />
            </b-form-group>
          </div>
          <div class="col-5">
            <b-form-group id="label-characterdescription" label="Description:" label-for="input-description">
              <b-form-textarea id="input-description" v-model="character.description" placeholder="Enter a synposis of the show and any other important details." rows="3" max-rows="6" />
            </b-form-group>
          </div>
          <div class="col-1">
            <b-form-group id="label-character-low" label="Low:" label-for="input-character-low">
              <b-form-select id="input-character-low" v-model="character.low" float-label="Lowest note:" :options="notes" />
            </b-form-group>
          </div>
          <div class="col-1">
            <b-form-group id="label-character-high" label="High:" label-for="input-cgaracter-high">
              <b-form-select id="input-character-high" v-model="character.high" float-label="Highest note" :options="notes" />
            </b-form-group>
          </div>
          <div class="col-1">
            <b-button @click="removeCharacter(character.name)" variant="danger">-</b-button>
          </div>
        </div>
      </div>
      <b-button type="submit" variant="primary">Submit</b-button>
    </b-form>
  </div>
</template>

<script>
import { mapState } from 'vuex'
export default {
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  computed: mapState({
    notes: state => state.notes.all
  }),
  data () {
    return {
      blockRemoval: true
    }
  },
  created () {
    this.$store.dispatch('notes/getAllNotes')
  },
  mounted: function () {
  },
  watch: {
    value () {
      this.$emit('input', this.value)
    }
  },
  methods: {
    onSubmit () {
      this.$parent.onSubmit()
    },
    addSong () {
      this.$props.value.songs.push({
        name: null,
        description: null,
        low: null,
        high: null
      })
    },
    removeSong (songId) {
      this.$props.value.songs.splice(songId, 1)
    },
    addCharacter () {
      this.$props.value.characters.push({
        name: null,
        description: null,
        low: null,
        high: null
      })
    },
    removeCharacter (character) {
      this.$props.value.characters.splice(character, 1)
    }
  }
}
</script>
