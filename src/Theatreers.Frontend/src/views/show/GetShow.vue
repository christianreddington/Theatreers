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
    <p v-if="show">Composer: {{ show.composer }}</p>

    <h2>Songs</h2>
    <div id="wrapper" v-if="show && show.songs">
      <div class="list-group" v-for="song in show.songs" v-bind:key="song.name">
        <a href="#" class="list-group-item list-group-item-action">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">{{song.name}}</h5>
            <small>{{ search(song.low, notes).note }} - {{ search(song.high, notes).note }}, {{ song.key }}</small>
          </div>
          <p class="mb-1">{{ song.participants.join(', ') }}</p>
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
import ShowImage from '../../components/ShowImage'
import ShowNews from '../../components/ShowNews'
export default {
  data () {
    return {
      breadcrumbs: null,
      show: null,
      images: null,
      myData: null,
      news: null,
      notes: [
        {
          'midi': 21,
          'note': 'A0'
        },
        {
          'midi': 22,
          'note': 'A#0 / B♭0'
        },
        {
          'midi': 23,
          'note': 'B0'
        },
        {
          'midi': 24,
          'note': 'C1'
        },
        {
          'midi': 25,
          'note': 'C#1 / D♭1'
        },
        {
          'midi': 26,
          'note': 'D1'
        },
        {
          'midi': 27,
          'note': 'D#1 / E♭1'
        },
        {
          'midi': 28,
          'note': 'E1'
        },
        {
          'midi': 29,
          'note': 'F1'
        },
        {
          'midi': 30,
          'note': 'F#1 / G♭1'
        },
        {
          'midi': 31,
          'note': 'G1'
        },
        {
          'midi': 32,
          'note': 'G#1 / A♭1'
        },
        {
          'midi': 33,
          'note': 'A1'
        },
        {
          'midi': 34,
          'note': 'A#1 / B♭1'
        },
        {
          'midi': 35,
          'note': 'B1'
        },
        {
          'midi': 36,
          'note': 'C2'
        },
        {
          'midi': 37,
          'note': 'C#2 / D♭2'
        },
        {
          'midi': 38,
          'note': 'D2'
        },
        {
          'midi': 39,
          'note': 'D#2 / E♭2'
        },
        {
          'midi': 40,
          'note': 'E2'
        },
        {
          'midi': 41,
          'note': 'F2'
        },
        {
          'midi': 42,
          'note': 'F#2 / G♭2'
        },
        {
          'midi': 43,
          'note': 'G2'
        },
        {
          'midi': 44,
          'note': 'G#2 / A♭2'
        },
        {
          'midi': 45,
          'note': 'A2'
        },
        {
          'midi': 46,
          'note': 'A#2 / B♭2'
        },
        {
          'midi': 47,
          'note': 'B2'
        },
        {
          'midi': 48,
          'note': 'C3'
        },
        {
          'midi': 49,
          'note': 'C#3 / D♭3'
        },
        {
          'midi': 50,
          'note': 'D3'
        },
        {
          'midi': 51,
          'note': 'D#3 / E♭3'
        },
        {
          'midi': 52,
          'note': 'E3'
        },
        {
          'midi': 53,
          'note': 'F3'
        },
        {
          'midi': 54,
          'note': 'F#3 / G♭3'
        },
        {
          'midi': 55,
          'note': 'G3'
        },
        {
          'midi': 56,
          'note': 'G#3 / A♭3'
        },
        {
          'midi': 57,
          'note': 'A3'
        },
        {
          'midi': 58,
          'note': 'A#3 / B♭3'
        },
        {
          'midi': 59,
          'note': 'B3'
        },
        {
          'midi': 60,
          'note': 'C4'
        },
        {
          'midi': 61,
          'note': 'C#4 / D♭4'
        },
        {
          'midi': 62,
          'note': 'D4'
        },
        {
          'midi': 63,
          'note': 'D#4 / E♭4'
        },
        {
          'midi': 64,
          'note': 'E4'
        },
        {
          'midi': 65,
          'note': 'F4'
        },
        {
          'midi': 66,
          'note': 'F#4 / G♭4'
        },
        {
          'midi': 67,
          'note': 'G4'
        },
        {
          'midi': 68,
          'note': 'G#4 / A♭4'
        },
        {
          'midi': 69,
          'note': 'A4'
        },
        {
          'midi': 70,
          'note': 'A#4 / B♭4'
        },
        {
          'midi': 71,
          'note': 'B4'
        },
        {
          'midi': 72,
          'note': 'C5'
        },
        {
          'midi': 73,
          'note': 'C#5 / D♭5'
        },
        {
          'midi': 74,
          'note': 'D5'
        },
        {
          'midi': 75,
          'note': 'D#5 / E♭5'
        },
        {
          'midi': 76,
          'note': 'E5'
        },
        {
          'midi': 77,
          'note': 'F5'
        },
        {
          'midi': 78,
          'note': 'F#5 / G♭5'
        },
        {
          'midi': 79,
          'note': 'G5'
        },
        {
          'midi': 80,
          'note': 'G#5 / A♭5'
        },
        {
          'midi': 81,
          'note': 'A5'
        },
        {
          'midi': 82,
          'note': 'A#5 / B♭5'
        },
        {
          'midi': 83,
          'note': 'B5'
        },
        {
          'midi': 84,
          'note': 'C6'
        },
        {
          'midi': 85,
          'note': 'C#6 / D♭5'
        },
        {
          'midi': 86,
          'note': 'D6'
        },
        {
          'midi': 87,
          'note': 'D#6 / E♭6'
        },
        {
          'midi': 88,
          'note': 'E6'
        },
        {
          'midi': 89,
          'note': 'F6'
        },
        {
          'midi': 90,
          'note': 'F#6 / G♭6'
        },
        {
          'midi': 91,
          'note': 'G6'
        },
        {
          'midi': 92,
          'note': 'G#6 / A♭6'
        },
        {
          'midi': 93,
          'note': 'A6'
        },
        {
          'midi': 94,
          'note': 'A#6 / B♭6'
        },
        {
          'midi': 95,
          'note': 'B6'
        },
        {
          'midi': 96,
          'note': 'C7'
        },
        {
          'midi': 97,
          'note': 'C#7 / D♭7'
        },
        {
          'midi': 98,
          'note': 'D7'
        },
        {
          'midi': 99,
          'note': 'D#7 / E♭7'
        },
        {
          'midi': 100,
          'note': 'E7'
        },
        {
          'midi': 101,
          'note': 'F7'
        },
        {
          'midi': 102,
          'note': 'F#7 / G♭7'
        },
        {
          'midi': 103,
          'note': 'G7'
        },
        {
          'midi': 104,
          'note': 'G#7 / A♭7'
        },
        {
          'midi': 105,
          'note': 'A7'
        },
        {
          'midi': 106,
          'note': 'A#7 / B♭7'
        },
        {
          'midi': 107,
          'note': 'B7'
        },
        {
          'midi': 108,
          'note': 'C8'
        }
      ]
    }
  },
  components: {
    ShowImage,
    ShowNews
  },
  methods: {
    search: function (nameKey, myArray) {
      return search(nameKey, myArray)
    }
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

function search (nameKey, myArray) {
  for (var i = 0; i < myArray.length; i++) {
    if (myArray[i].midi === nameKey) {
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
