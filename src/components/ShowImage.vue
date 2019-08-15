<template>
  <div>
    <div v-if="imageObjects.length > 0 && imageObjects != null">
      <b-row v-for="(images, groupIndex) in groupedImages(imageObjects, 3)" v-bind:key="groupIndex">
        <b-col v-for="(image, imageIndex) in images" v-bind:key="imageIndex">
            <b-img thumbnail fluid :src="image.innerObject.contentUrl" :thing="image.innerObject.name" />
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
