<template>
  <div class="group">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <h1>Specific Group</h1>
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
          href: this.$router.resolve({ name: 'getgroups' }).href
        },
        {
          text: 'Group',
          active: true
        }
      ]
    }
  }
}
</script>
