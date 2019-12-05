<template>
  <div>
    <div v-if="newsObjects.length > 0 && newsObjects != null">
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
    <div v-else>
      <h4>There are no articles available</h4>
    </div>
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
    groupedImages (array, size) {
      const chunkedArr = []
      let index = 0
      while (index < array.length) {
        chunkedArr.push(array.slice(index, size + index))
        index += size
      }
      return chunkedArr
    }
  },
  data () {
    return {
      itemsPerRow: 3
    }
  }
}
</script>
