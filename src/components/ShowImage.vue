<template>
  <div>
    <b-row v-for="(images, groupIndex) in groupedImages(imageObjects, 3)" v-bind:key="groupIndex">
      <b-col v-for="(image, imageIndex) in images" v-bind:key="imageIndex">
          <b-img thumbnail fluid :src="image.contentUrl" :thing="image.name" />
      </b-col>
    </b-row>
  </div>
</template>

<script src="https://cdn.jsdelivr.net/lodash/4.17.4/lodash.min.js"></script>
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
