<template>
  <div class="alert-component">
    <div v-for="item in alerts" v-bind:key="item.id">
      <b-alert show v-bind:variant="item.variant" dismissible @dismissed="removeAlert(item)">{{item.message}}</b-alert> 
    </div>
  </div>
</template>

<script>
import { mapState } from 'vuex'
export default {
  computed: mapState({
    alerts: state => state.alerts.alerts
  }),
  methods: {
    removeAlert: function (alert) {
      this.$store.commit('alerts/removeAlert', alert.id);
    }
  }
}
</script>