<template>
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs" v-if="breadcrumbs"></b-breadcrumb>
    <div align="right">
      <b-button variant="primary" v-if="show" :to="{ name: 'editshow', params: { id: $route.params.id } }" >Edit</b-button> <b-button variant="danger" v-if="show" @click="deleteShow">Delete</b-button>
    </div>
    <h1 v-if="show">{{ show.showName }}</h1>
    <p v-if="show">{{ show.showName }}</p>
    <p v-if="show">Composer: {{ show.composer }}</p>
    <p v-if="show">Author: {{ show.author }}</p>
    <p v-if="show">Composer: {{ show.composer }}</p>

    <h2>Songs</h2>
    <div id="wrapper" v-if="show && show.songs">
      <div class="list-group" v-for="song in show.songs" v-bind:key="song.name">
        <a href="#" class="list-group-item list-group-item-action">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">{{song.name}}</h5>
            <small>{{ search(song.low, notes).text }} - {{ search(song.high, notes).text }}, {{ song.key }}</small>
          </div>
          <p class="mb-1" v-if="song.participants">{{ song.participants.join(', ') }}</p>
        </a>
      </div>
    </div>

    <h2>Characters</h2>
    <div id="wrapper" v-if="show && show.characters">
      <div class="list-group" v-for="character in show.characters" v-bind:key="character.name">
        <a href="#" class="list-group-item list-group-item-action">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">{{character.name}}</h5>
            <small>{{character.vocalType}}</small>
          </div>
          <p class="mb-1">{{ character.description }}</p>
        </a>
      </div>
    </div>

    <h1>News</h1>
    <ShowNews :newsObjects="news" v-if="news" />

    <h1>Gallery <b-button variant="primary" v-b-modal.modal-1>Upload</b-button></h1>
    <ShowImage :imageObjects="images" v-if="images" />
    
  <b-modal id="modal-1" :title="'Upload images to ' + show.showName + ' Gallery'">

    
      <div v-if="isSuccess">
        <h2>Uploaded {{ uploadedFiles.length }} file(s) successfully, pending approval.</h2>
        <p>
          <a href="javascript:void(0)" @click="reset()">Upload again</a>
        </p>
      </div>
      <!--FAILED-->
      <div v-if="isFailed">
        <h2>Uploaded failed.</h2>
        <p>
          <a href="javascript:void(0)" @click="reset()">Try again</a>
        </p>
        <pre>{{ uploadError }}</pre>
      </div>

      <form enctype="multipart/form-data" novalidate v-if="isInitial || isSaving">
        <div class="dropbox">
          <input type="file" multiple :name="uploadFieldName" :disabled="isSaving" @change="filesChange($event.target.name, $event.target.files); fileCount = $event.target.files.length"
            accept="image/*" class="input-file">
            <p v-if="isInitial">
              Drag your file(s) here to begin<br> or click to browse
            </p>
            <p v-if="isSaving">
              Uploading {{ fileCount }} files...
            </p>
        </div>
      </form>
  </b-modal>
  </div>
</template>

<script>
const STATUS_INITIAL = 0, STATUS_SAVING = 1, STATUS_SUCCESS = 2, STATUS_FAILED = 3;

import ShowImage from '../../components/ShowImage'
import ShowNews from '../../components/ShowNews'
import upload from '../../services/blobuploader'

export default {
  computed: {
      notes() {
        return this.$store.state.notes.all;
      }, 
      isInitial() {
        return this.currentStatus === STATUS_INITIAL;
      },
      isSaving() {
        return this.currentStatus === STATUS_SAVING;
      },
      isSuccess() {
        return this.currentStatus === STATUS_SUCCESS;
      },
      isFailed() {
        return this.currentStatus === STATUS_FAILED;
      }
  },
  data () {
    return {
      breadcrumbs: null,
      show: null,
      images: null,
      myData: null,
      news: null,
      uploadedFiles: [],
      uploadError: null,
      currentStatus: null,
      uploadFieldName: 'photos',
      fileCount: null
    }
  },
  components: {
    ShowImage,
    ShowNews
  },
  methods: {
    
      reset() {
        // reset form to initial state
        this.currentStatus = STATUS_INITIAL;
        this.uploadedFiles = [];
        this.uploadError = null;
      },
      save(formData) {
        // upload data to the server
        this.currentStatus = STATUS_SAVING;

        var tokenRequest = {
            scopes: ['https://theatreers.onmicrosoft.com/show-api/user_impersonation']
          }
       
      this.$AuthService.acquireToken(tokenRequest)
        .then(bearerToken => {
          upload(formData, this.show.id, bearerToken)
            .then(x => {
              console.log("X marks the spot " + x);
              this.uploadedFiles = [].concat(x);
              this.currentStatus = STATUS_SUCCESS;
            })
            .catch(err => {
              this.uploadError = err.response;
              this.currentStatus = STATUS_FAILED;
            });
        });
      },
      filesChange(fieldName, fileList) {
        // handle file changes
        const formData = new FormData();

        if (!fileList.length) return;

        // append the files to FormData
        Array
          .from(Array(fileList.length).keys())
          .map(x => {
            formData.append(fieldName, fileList[x], fileList[x].name);
          });

        // save it
        this.save(formData);
      },
    search: function (nameKey, myArray) {
      return search(nameKey, myArray)
    },
    deleteShow() {
      this.$bvModal.msgBoxConfirm(`Please confirm that you want to delete the show ${this.show.showName}.`, {
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
                this.$AuthService.deleteApi(`https://api.theatreers.com/show/show/${this.$route.params.id}`, bearerToken.accessToken)
                  .catch(function (error) {
                    console.log('Error: ' + error)
                  })
                  .then(function (response) {
                    console.log(response);
                    //console.log(self);
                    self.$router.push("/show/");
                    self.$store.commit('alerts/setAlerts', [
                      {"variant": "success", "message": `${self.show.showName} was successfully deleted`},
                    ]);
                  })
              })

          }
        })
        .catch(err => {
          console.log(err);
          // An error occurred
        })
    }
  },
  created () {
    this.$store.dispatch('notes/getAllNotes')
  },
  mounted: async function () {
    this.reset()
    this.isLoading = true
    var urls = [
      `https://api.theatreers.com/show/show/${this.$route.params.id}/show`,
      `https://api.theatreers.com/show/show/${this.$route.params.id}/image`,
      `https://api.theatreers.com/show/show/${this.$route.params.id}/news`
    ]

    this.myData = getAllUrls(urls).then((data) => {
      this.show = data[0]
      this.images = data[1]
      this.news = data[2]

      this.isLoading = false

      
    this.$store.commit('breadcrumbs/setBreadcrumbs', 
    [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Shows',
          href: this.$router.resolve({ name: 'getshows' }).href
        },
        {
          text: this.show.showName,
          active: true
        }
      ])
    })
      .catch((error) => {
        console.log('error here ' + error)
      })
  }
}

async function getAllUrls (urls) {
  try {
    var data = await Promise.all(
      urls.map(url => getContent(url))
    )
    return data
  } catch (error) {
    // console.log(error)
    throw error
  }
}

function search (nameKey, myArray) {
  for (var i = 0; i < myArray.length; i++) {
    if (myArray[i].value === nameKey) {
      return myArray[i]
    }
  }
}

async function getContent (url) {
  let response = await fetch(url)

  if (response.ok) { // if HTTP-status is 200-299
    // console.log('returning response')
    return response.json()
  } else {
    // console.log('HTTP-Error: ' + response.status)
    // var emptyObject = new Array()
    return []
  }
}
</script>


<style lang="scss">
  .dropbox {
    outline: 2px dashed grey; /* the dash box */
    outline-offset: -10px;
    background: lightcyan;
    color: dimgray;
    padding: 10px 10px;
    min-height: 200px; /* minimum height */
    position: relative;
    cursor: pointer;
  }

  .input-file {
    opacity: 0; /* invisible but it's there! */
    width: 100%;
    height: 200px;
    position: absolute;
    cursor: pointer;
  }

  .dropbox:hover {
    background: lightblue; /* when mouse over to the drop zone, change color */
  }

  .dropbox p {
    font-size: 1.2em;
    text-align: center;
    padding: 50px 0;
  }
</style>