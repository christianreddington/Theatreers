<template>
  <div class="group">
    <h1>Specific Group</h1>
  </div>
</template>

<script>
export default {
  mounted: async function(){
    this.$store.commit('breadcrumbs/setBreadcrumbs', 
    [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Groups',
          href: this.$router.resolve({ name: 'getgroups' }).href
        },
        {
          text: 'Group',
          active: true
        }
      ])
  }
}
</script>
