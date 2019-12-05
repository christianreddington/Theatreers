<template>
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs" v-if="show"></b-breadcrumb>
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
      fetch(`https://api.theatreers.com/show/show/${this.$route.params.id}`, {
        headers: {
          'Accept': 'application/json'
        },
        method: 'PUT',
        body: JSON.stringify(this.show)
      })
        .then(function (response) {
          alert('Form submitted')
        })
        .catch(function (error) {
          // // console.log(error)
          // // console.log('blah')
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
            href: this.$router.resolve({ name: 'getshow', params: { id: this.$route.params.id } }).href
          },
          {
            text: 'Edit',
            active: true
          }
        ]
      })
  }
}
</script>
