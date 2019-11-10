<template>

      <div>
  <b-form @submit="onSubmit">
    <b-form-group
      id="label-showName"
      label="Show Name:"
      label-for="showName"
    >
      <b-form-input
        id="input-showName"
        v-model="value.showName"
        type="text"
        required
        placeholder="Enter the name of the show"
      ></b-form-input>
    </b-form-group>

    <b-form-group id="label-description" label="Description" label-for="input-description">
      <b-form-textarea
        id="input-description"
        v-model="value.description"
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
        v-model="value.author"
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
        v-model="value.composer"
        type="text"
        required
        placeholder="Name of the Composer"
      ></b-form-input>
    </b-form-group>

    <b-button type="submit" variant="primary">Submit</b-button>
  </b-form>
</div>
</template>

<script>
export default {
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  watch: {
    value () {
      this.$emit('input', this.value)
    }
  },
  methods: {
    onSubmit () {
      this.$parent.onSubmit()
    }
  }
}
</script>
