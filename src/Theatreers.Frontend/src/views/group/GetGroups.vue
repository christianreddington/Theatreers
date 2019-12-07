<template>
  <div class="overflow-auto">
    <h1>Groups</h1>
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
          active: true
        }
      ])
  }
}
</script>
