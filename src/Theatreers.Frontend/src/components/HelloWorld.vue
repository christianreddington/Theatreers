<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <p>
      For a guide and recipes on how to configure / customize this project,<br />
      check out the
      <a href="https://cli.vuejs.org" target="_blank" rel="noopener"
        >vue-cli documentation</a
      >.
    </p>
    
    <div>
      <ul v-for="item in notes" v-bind:key="item.value">
        <li>{{item.text}}</li> 
      </ul>
    </div>
  </div>
</template>

<script>



import { mapState, mapActions } from 'vuex'
export default {
  computed: mapState({
    notes: state => state.notes.all
  }),
  methods: mapActions('notes', [
    'addNotesToCart'
  ]),
  created () {
    this.$store.dispatch('notes/getAllNotes')
  },  
  name: "HelloWorld",
  props: {
    msg: String
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
