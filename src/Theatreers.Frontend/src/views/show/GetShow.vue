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

    <h1>Gallery</h1>
    <ShowImage :imageObjects="images" v-if="images" />
  </div>
</template>

<script>
import { mapState } from 'vuex'
import ShowImage from '../../components/ShowImage'
import ShowNews from '../../components/ShowNews'
export default {
  computed: mapState({
    notes: state => state.notes.all
  }),
  data () {
    return {
      breadcrumbs: null,
      show: null,
      images: null,
      myData: null,
      news: null
    }
  },
  components: {
    ShowImage,
    ShowNews
  },
  methods: {
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
                      {"id": 1, "variant": "success", "message": `${self.show.showName} was successfully deleted`},
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
