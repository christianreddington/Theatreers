<template>
  <div class="about">
    <h1 v-if="show">Edit {{ show.showName }}</h1>
    <ShowForm v-model="show" />
  </div>
</template>

<script>
import ShowForm from '../../components/ShowForm'

export default {
  components: {
    ShowForm
  },
  data () {
    return {
      show: {
        id: this.$route.params.id,
        showId: this.$route.params.id
      },
      visible: true,
      breadcrumbs: null
    }
  },
  methods: {
    onSubmit (evt) {
      console.log(evt);
      var tokenRequest = {
        scopes: ['https://theatreers.onmicrosoft.com/show-api/user_impersonation']
      }

      var jsonBody = JSON.stringify(this.show)

      this.$AuthService.acquireToken(tokenRequest)
        .then(bearerToken => {
          let self = this
          this.$AuthService.putApi(`https://api.theatreers.com/show/show/${this.$route.params.id}`, jsonBody, bearerToken.accessToken)
            .catch(function (error) {
              console.log('Error: ' + error)
            })
            .then(function (response) {
              console.log(response);
              //console.log(self);
              self.$router.push("/show/"+ self.$route.params.id);
              self.$store.commit('alerts/setAlerts', [
                {"id": 1, "variant": "success", "message": `${self.show.showName} was successfully updated`},
              ]);
            })
        })
    }
  },
  mounted: function () {
    this.isLoading = true
    fetch(`https://api.theatreers.com/show/show/${this.$route.params.id}/show`, {
      method: 'GET'
    })
      .then(function (response) {
        return response.json()
      })
      .then((jsonData) => {
        this.isLoading = false
        this.show = jsonData
        this.$store.commit('breadcrumbs/setBreadcrumbs', 
        [
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
            href: this.$router.resolve({ name: 'getshow', params: { id: this.$route.params.id } }).href
          },
          {
            text: 'Edit',
            active: true
          }
        ])  
      })
  }
}
</script>
