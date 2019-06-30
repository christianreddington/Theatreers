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
    data() {
      return {
        show: {
          showName: '',
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
      onSubmit(evt) {
        self = this  
        this.$AuthService.getAccessToken(['https://theatreers.onmicrosoft.com/show-api/user_impersonation'])
        .then(bearerToken => {              
          self.$AuthService.postApi('https://th-show-dev-neu-func.azurewebsites.net/api/show', 
          JSON.stringify(this.show), 
          bearerToken)           
          .catch(function () {                
            self.alert = {
              visible: true,
              content: `${self.show.showName} has not been successfully created`,
              type: 'danger',
              display: true
            }
          })
          .then(function() {         
            self.alert = {
              visible: true,
              content: `${self.show.showName} has been successfully created`,
              type: 'success',
              display: true
            }
          })
        })  
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
