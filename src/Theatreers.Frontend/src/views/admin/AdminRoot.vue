<template name="AdminRoot">
  <div class="about">Admin Root</div>
</template>

<script>

export default {
  mounted: function () {
    this.$store.commit('breadcrumbs/setBreadcrumbs', 
    [
      {
        text: 'Theatreers',
        href: this.$router.resolve({ name: 'root' }).href
      },
      {
        text: 'Admin',
        active: true
      }
    ])
  }
}
</script>
