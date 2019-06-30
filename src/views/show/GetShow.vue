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
    <h1 v-if="show">{{ show.innerObject.showName }}</h1>
    <p v-if="show">{{ show.innerObject.showName }}</p>
    <p v-if="show">Composer: {{ show.innerObject.composer }}</p>
    <p v-if="show">Author: {{ show.innerObject.author }}</p>

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
  data() {
    return {
      breadcrumbs: null,
      show: null,
      images: null,
      myData: null,
      news: null
    };
  },
  components: {
    ShowImage,
    ShowNews
  },   
  mounted: function() {
    this.isLoading = true;
    var urls = [
      `https://api.theatreers.com/show/show/${this.$route.params.id}/show`,
      `https://api.theatreers.com/show/show/${this.$route.params.id}/image`,
      `https://api.theatreers.com/show/show/${this.$route.params.id}/news`
    ]

    this.myData = getAllUrls(urls). then((data) => {
      this.show = data[0][0]
      this.images = data[1]
      this.news = data[2]

      this.isLoading = false
      this.breadcrumbs = [
        {
          text: "Theatreers",
          href: this.$router.resolve({ name: "root" }).href
        },
        {
          text: "Shows",
          href: this.$router.resolve({ name: "getshows" }).href
        },
        {
          text: this.show.showName,
          active: true
        }
      ];
    }) 
    /*
    fetch(
      `https://th-show-neu-dev-func.azurewebsites.net/api/show/${
        this.$route.params.id
      }/show`,
      {
        method: "get"
      }
    )
      .then(function(response) {
        return response.json();
      })
      .then(jsonData => {
        this.isLoading = false;
        this.show = jsonData[0];
        this.breadcrumbs = [
          {
            text: "Theatreers",
            href: this.$router.resolve({ name: "root" }).href
          },
          {
            text: "Shows",
            href: this.$router.resolve({ name: "getshows" }).href
          },
          {
            text: this.show.showName,
            active: true
          }
        ];
      });*/
  }
};

async function getAllUrls(urls) {
  try {
    var data = await Promise.all(
      urls.map(url => fetch(url).then(response => response.json()))
    );
    return data;
  } catch (error) {
    console.log(error);
    throw error;
  }
}
</script>
