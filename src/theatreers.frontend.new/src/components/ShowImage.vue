<template>
  <div>
    <div v-if="imageObjects.length > 0 && imageObjects != null">
      <b-row v-for="(images, groupIndex) in groupedImages(imageObjects, 3)" v-bind:key="groupIndex">
        <b-col v-for="(image, imageIndex) in images" v-bind:key="imageIndex">
            <b-img thumbnail fluid :src="image.contentUrl" :name="image.name" />
        </b-col>
      </b-row>
    </div>
    <div v-else>
      <h4>There are no images available</h4>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    imageObjects: {
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
