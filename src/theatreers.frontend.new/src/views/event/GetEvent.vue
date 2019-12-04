<template>
  <div class="event">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <h1>Specific Event</h1>
  </div>
</template>

<script>
export default {
  data () {
    return {
      breadcrumbs: [
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
      ]
    }
  }
}
</script>
