<template>
    <b-breadcrumb :items="breadcrumbs"></b-breadcrumb>
</template>

<script>
import { mapState, mapActions } from 'vuex'
export default {
  computed: mapState({
    breadcrumbs: state => state.breadcrumbs.all
  }),
  methods: mapActions('breadcrumbs', [
    'addBreacrumbsToCart'
  ]),
  created () {
    this.$store.dispatch('breadcrumbs/getAllBreadcrumbs')
  }
}
</script>