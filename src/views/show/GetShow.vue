<template>
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <div align="right">
      <b-button variant="primary" v-if="show" :to="{ name: 'editshow', params: { id: $route.params.id } }">Edit</b-button>
    </div>
    <h1 v-if="show">{{ show.showName }}</h1>
    <p>{{ show.showName }}</p>
    <p>Composer: {{ show.composer }}</p>
    <p>Author: {{ show.author }}</p>
  </div>
</template>

<script>
export default {
  data () {
    return {
      show: null
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
      this.breadcrumbs = [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Shows',
          href: this.$router.resolve({ name: 'getshows' }).href
        },
        {
          text: this.show.showName,
          active: true
        }
      ]
    })
  }
}
</script>
