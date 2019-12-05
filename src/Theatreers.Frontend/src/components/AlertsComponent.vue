<template>
  <div class="alert-component">
    <div v-for="item in alerts" v-bind:key="item.id">
      <b-alert show v-bind:variant="item.variant" dismissible>{{item.message}}</b-alert> 
    </div>
  </div>
</template>

<script>
import { mapState, mapActions } from 'vuex'
export default {
  computed: mapState({
    alerts: state => state.alerts.all
  }),
  methods: mapActions('cart', [
    'addProductToCart'
  ]),
  created () {
    this.$store.dispatch('alerts/getAllAlerts')
  }
}
</script>