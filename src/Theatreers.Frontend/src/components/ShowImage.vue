<template>
  <div>
    <div v-if="imageObjects.length > 0 && imageObjects != null">
      <b-row v-for="(images, groupIndex) in groupedImages(imageObjects, 3)" v-bind:key="groupIndex">
        <b-col v-for="(image, imageIndex) in images" v-bind:key="imageIndex">
          <div class="img-wrapper">
            <b-img thumbnail fluid :src="image.contentUrl" :name="image.name"  class="img-resposnive" />
            <div class="img-overlay">
              <button class="btn btn-md btn-danger" @click="deleteShowImage(image.id)">X</button>
            </div>
        </div>
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
    deleteShowImage(imageId) {
      this.$bvModal.msgBoxConfirm(`Please confirm that you want to delete this image ${imageId}`, {
        title: 'Please Confirm',
        size: 'sm',
        buttonSize: 'sm',
        okVariant: 'danger',
        okTitle: 'YES',
        cancelTitle: 'NO',
        footerClass: 'p-2',
        hideHeaderClose: false,
        centered: true
      })
        .then(value => {
          if (value){
            var tokenRequest = {
              scopes: ['https://theatreers.onmicrosoft.com/show-api/user_impersonation']
            }

            this.$AuthService.acquireToken(tokenRequest)
              .then(bearerToken => {
                let self = this
                this.$AuthService.deleteApi(`https://api.theatreers.com/show/show/${this.$route.params.id}/image/${imageId}`, bearerToken.accessToken)
                  .catch(function (error) {
                    self.$store.commit('alerts/setAlerts', [
                      {"id": 1, "variant": "danger", "message": `Image could not be deleted. ${error}`},
                    ]);
                  })
                  .then(function (response) {
                    console.log(response);
                    //console.log(self);
                    self.$store.commit('alerts/setAlerts', [
                      {"id": 1, "variant": "success", "message": `Image was successfully deleted`},
                    ]);

                    self.spliceArray(self.imageObjects, imageId);
                  })
              })

          }
        })
        .catch(err => {
          console.log(err);
          // An error occurred
        })
    },
    groupedImages (array, size) {
      const chunkedArr = []
      let index = 0
      while (index < array.length) {
        chunkedArr.push(array.slice(index, size + index))
        index += size
      }
      return chunkedArr
    },
    spliceArray(array, itemReference){
      console.log(array);
      var index = array.map(x => {
        return x.id;
      }).indexOf(itemReference);
      array.splice(index, 1)
      console.log(array);
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
.img-wrapper {
    position: relative;
   }
  
  .img-responsive {
    width: 100%;
    height: auto;
  }
  
  .img-overlay {
    position: absolute;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    text-align: center;
  }
  
  .img-overlay:before {
    content: ' ';
    display: block;
    /* adjust 'height' to position overlay content vertically */
    height: 5%;

  }
</style>