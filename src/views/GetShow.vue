<template>
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <h1 v-if="show">{{ show.showName }}</h1>
    <p>Description about Show</p>
  </div>
</template>

<script>
export default {
  data () {
    return {
      show: null,
      breadcrumbs: [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Shows',
          href: this.$router.resolve({ name: 'getshows' }).href
        },
        {
          text: 'Show',
          active: true
        }
      ]
    }
  },
  mounted: function () {
    this.isLoading = true
    fetch(`https://th-show-weu-dev-func.azurewebsites.net/api/show/${this.$route.params.id}/show`, {
      method: 'get'
    })
      .then(function (response) {
        return response.json()
      })
      .then((jsonData) => {
        this.isLoading = false
        this.show = jsonData[0]
      })
  }
}
</script>
