<template>
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <h1 v-if="show">Edit {{ show.showName }}</h1>
    <div>
    <b-form @submit="onSubmit" v-if="visible">
      <b-form-group
        id="label-showName"
        label="Show Name:"
        label-for="showName"
      >
        <b-form-input
          id="input-showName"
          v-model="form.showName"
          type="text"
          required
          placeholder="Enter the name of the show"
          v-if="show"
        ></b-form-input>
      </b-form-group>

      <b-form-group id="label-description" label="Description" label-for="input-description">
        <b-form-textarea
          id="input-description"
          v-model="form.description"
          placeholder="Enter a synposis of the show and any other important details."
          rows="3"
          max-rows="6"
        ></b-form-textarea>
      </b-form-group>
      
      <b-form-group
        id="label-author"
        label="Author:"
        label-for="input-author"
      >
        <b-form-input
          id="input-author"
          v-model="form.author"
          type="text"
          required
          placeholder="Name of the Author"
        ></b-form-input>
      </b-form-group>

      <b-form-group
        id="label-composer"
        label="Composer:"
        label-for="input-composer"
      >
        <b-form-input
          id="input-composer"
          v-model="form.composer"
          type="text"
          required
          placeholder="Name of the Composer"
        ></b-form-input>
      </b-form-group>

      <b-button type="submit" variant="primary">Submit</b-button>
    </b-form>
    <b-card class="mt-3" header="Form Data Result">
      <pre class="m-0">{{ form }}</pre>
    </b-card>
  </div>
  </div>
</template>

<script>
export default {
      data() {
      return {
        form: {
          id: this.$route.params.id,
          showId: this.$route.params.id
        },
        visible: true,
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
    methods: {
      onSubmit(evt) {
        fetch("https://th-show-weu-dev-func.azurewebsites.net/api/updateshow", {
          headers: {
          'Accept': 'application/json'
          },
          method: 'POST',
          body: JSON.stringify(this.form)
        })
        .then(function (response) {      
          alert("Form submitted")
        })
        .catch(function (error) {
          console.log(error)
          console.log("HELLO WORLD")
        })
      }
    },
    mounted: function () {
      this.isLoading = true
      fetch(`https://th-show-weu-dev-func.azurewebsites.net/api/show/${this.$route.params.id}/show`, {
        method: 'GET'
      })
        .then(function (response) {
          return response.json()
        })
        .then((jsonData) => {
          this.isLoading = false
          this.show = jsonData[0]
          this.form.showName = this.show.showName
          this.form.description = this.show.description
          this.form.author = this.show.author
          this.form.composer = this.show.composer
        })
    }
}
</script>
