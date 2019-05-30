<template>
  <div class="overflow-auto">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <h1>Groups</h1>
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
          text: 'Groups',
          active: true
        }
      ]
    }
  }
}
</script>
