<template>
  <div class="overflow-auto">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs"></b-breadcrumb>
    <div align="right">
      <b-button variant="primary" :to="{ name: 'createshow' }">Create</b-button>
    </div>
    <h1 v-if="!selectedPartition">Shows</h1>
    <h1 v-if="selectedPartition">Shows beginning with {{ selectedPartition }}</h1>

    <b-form-select v-model="selectedPartition" :options="partitions"></b-form-select>
    <br /><br />
    <b-pagination
      v-model="currentPage"
      :total-rows="rows"
      :per-page="perPage"
      aria-controls="my-table"
      v-if="items && items.length > 0"
    ></b-pagination>

    <p class="mt-3" v-if="items && items.length > 0">Current Page: {{ currentPage }}</p>

    <b-spinner variant="primary" type="grow" label="Spinning" v-if="isLoading"></b-spinner>

    <div>
      <b-media  v-for="item in items" v-bind:key="item.showId">
        <svg slot="aside" class="bd-placeholder-img mr-3" width="64" height="64" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="xMidYMid slice" focusable="false" role="img" aria-label="Placeholder: 64x64">
          <title>Placeholder</title>
          <rect width="100%" height="100%" fill="#868e96"></rect>
          <text x="50%" y="50%" fill="#dee2e6" dy=".3em">64x64</text>
        </svg>

        <h2 align="left"><router-link :to="`/show/${item.id}`">{{ item.showName }}</router-link></h2>
        <p align="left">Description of {{ item.showName }}</p>

        <!-- b-[Optional: add media children here for nesting] -->
      </b-media>
    </div>
    <p v-if="selectedPartition && items.length === 0 && !isLoading">Sorry, there were no results for letter {{ selectedPartition }}</p>
  </div>
</template>
<script>
export default {
  data () {
    return {
      perPage: 3,
      currentPage: 1,
      breadcrumbs: [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Shows',
          active: true
        }
      ],
      items: [],
      isLoading: false,
      selectedPartition: null,
      partitions: [
        { value: 'A', text: 'A' },
        { value: 'B', text: 'B' },
        { value: 'C', text: 'C' },
        { value: 'D', text: 'D' },
        { value: 'E', text: 'E' },
        { value: 'F', text: 'F' },
        { value: 'G', text: 'G' },
        { value: 'H', text: 'H' },
        { value: 'I', text: 'I' },
        { value: 'J', text: 'J' },
        { value: 'K', text: 'K' },
        { value: 'L', text: 'L' },
        { value: 'M', text: 'M' },
        { value: 'N', text: 'N' },
        { value: 'O', text: 'O' },
        { value: 'P', text: 'P' },
        { value: 'Q', text: 'Q' },
        { value: 'R', text: 'R' },
        { value: 'S', text: 'S' },
        { value: 'T', text: 'T' },
        { value: 'U', text: 'U' },
        { value: 'V', text: 'V' },
        { value: 'W', text: 'W' },
        { value: 'X', text: 'X' },
        { value: 'Y', text: 'Y' },
        { value: 'Z', text: 'Z' },
        { value: '0-9', text: '0-9' }
      ]
    }
  },
  computed: {
    rows () {
      return this.items.length
    }
  },
  watch: {
    selectedPartition: {
      immediate: false,
      handler () {
        this.isLoading = true
        self = this
        getApiWithoutToken(`https://api.theatreers.com/show/shows/${self.selectedPartition}`)
          .catch(function (error) {
            self.alert = {
              visible: true,
              content: `${error}`,
              type: 'danger',
              display: true
            }
          })
          .then(function (response) {
            return response.json()
          })
          .then((jsonData) => {
            this.isLoading = false
            this.items = jsonData
          })
      }
    }
  }
}
</script>
