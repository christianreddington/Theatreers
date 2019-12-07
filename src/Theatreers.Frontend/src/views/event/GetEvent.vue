<template>
  <div class="event">
    <h1>Specific Event</h1>
  </div>
</template>

<script>
export default {
  mounted: async function () {
      
    this.$store.commit('breadcrumbs/setBreadcrumbs', 
    [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Events',
          href: this.$router.resolve({ name: 'getevents' }).href
        },
        {
          text: 'Event',
          active: true
        }
      ])
  }
}
</script>
