<template>
  <div>
    <b-row v-for="(newsItems, groupIndex) in groupedImages(newsObjects, 5)" v-bind:key="groupIndex">
      <b-col v-for="(newsItem, newsIndex) in newsItems" v-bind:key="newsIndex">
          <b-card tag="article" style="max-width: 20rem;" class="mb-2">
            <b-card-text>
              <b-link :href="newsItem.url">{{ newsItem.name }}</b-link>
            </b-card-text>
          </b-card>
      </b-col>
    </b-row>
  </div>
</template>
<script>
export default {
  props: {
    newsObjects: {
      type: Array
    }
  },
  methods: {
    groupedImages(array, size) {
      const chunked_arr = [];
      let index = 0;
      while (index < array.length) {
        chunked_arr.push(array.slice(index, size + index));
        index += size;
      }
      return chunked_arr;
    }
  },
  data() {
    return {
      itemsPerRow: 3,
    }
  }
}
</script>
