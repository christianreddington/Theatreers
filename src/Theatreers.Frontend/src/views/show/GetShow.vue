<template>
  <div class="about">
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs" v-if="breadcrumbs"></b-breadcrumb>
    <div align="right">
      <b-button
        variant="primary"
        v-if="show"
        :to="{ name: 'editshow', params: { id: $route.params.id } }"
      >Edit</b-button>
    </div>
    <h1 v-if="show">{{ show.showName }}</h1>
    <p v-if="show">{{ show.showName }}</p>
    <p v-if="show">Composer: {{ show.composer }}</p>
    <p v-if="show">Author: {{ show.author }}</p>

    <h1>News</h1>
    <ShowNews :newsObjects="news" v-if="news" />

    <h1>Gallery</h1>
    <ShowImage :imageObjects="images" v-if="images" />
  </div>
</template>

<script>
import ShowImage from '../../components/ShowImage'
import ShowNews from '../../components/ShowNews'
export default {
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
      this.breadcrumbs = [
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
      ]
    })
      .catch((error) => {
        // console.log('error here ' + error)
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
