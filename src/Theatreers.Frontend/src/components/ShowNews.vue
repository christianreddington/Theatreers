<template>
  <div>
		<div v-if="newsObjects.length > 0 && newsObjects != null">
			<h4>Latest News</h4>
			<ul class="timeline" v-for="item in newsObjects" v-bind:key="item.id">
				<li>
          <b-link :href="item.url">{{item.name}}</b-link>
					<div class="float-right">{{item.datePublished}}</div>
        </li>
			</ul>
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
<style scoped>
ul.timeline {
    list-style-type: none;
    position: relative;
}
ul.timeline:before {
    content: ' ';
    background: #d4d9df;
    display: inline-block;
    position: absolute;
    left: 29px;
    width: 2px;
    height: 100%;
    z-index: 400;
}
ul.timeline > li {
    margin: 20px 0;
    padding-left: 20px;
}
ul.timeline > li:before {
    content: ' ';
    background: white;
    display: inline-block;
    position: absolute;
    border-radius: 50%;
    border: 3px solid #22c0e8;
    left: 20px;
    width: 20px;
    height: 20px;
    z-index: 400;
}
</style>
