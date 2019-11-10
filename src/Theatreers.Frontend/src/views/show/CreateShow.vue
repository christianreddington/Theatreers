<template name="CreateShow">
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs" v-if="show"></b-breadcrumb>
    <h1 v-if="show">Create {{ show.showName }}</h1>
    <b-alert v-model="alert.visible" :variant="alert.type" dismissible>
      {{ alert.content }}
    </b-alert>
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
        showName: '',
        description: '',
        author: '',
        composer: ''
      },
      visible: true,
      breadcrumbs: [],
      alert: {
        visible: false,
        type: 'info',
        content: 'No message to display'
      }
    }
  },
  methods: {
    onSubmit (evt) {
      var tokenRequest = {
        scopes: ['https://theatreers.onmicrosoft.com/show-api/user_impersonation']
      }

      var jsonBody = JSON.stringify(this.show)

      getAccessToken(tokenRequest)
        .then(bearerToken => {
          postApiWithToken('https://api.theatreers.com/show/show', jsonBody, bearerToken.accessToken)
            .catch(function (error) {
              // console.log('Error: ' + error)
            })
            .then(function (response) {
              // console.log('Succeeded')
            })
        })
      this.cleanPermission = null
      this.dirtyPermission = null
    }
  },
  mounted: function () {
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
        text: 'Create new Show',
        active: true
      }
    ]
  }
}
</script>
