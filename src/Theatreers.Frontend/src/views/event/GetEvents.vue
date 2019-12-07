<template>
  <div class="overflow-auto">
    <h1 >Events</h1>
    <b-row width="100%">
      <b-col width="2">
        <b-card
          title="Card Title"
          img-src="" img-alt="Image" img-top
          tag="article"
          style="max-widt: 20rem;"
          class="mb-2">
            <b-card-text>Some quick text</b-card-text>
            <b-button href="blah">Go somewhere</b-button>
        </b-card>
      </b-col>
      <b-col>
        <b-card
          title="Card Title"
          img-src="" img-alt="Image" img-top
          tag="article"
          style="max-widt: 20rem;"
          class="mb-2">
            <b-card-text>Some quick text</b-card-text>
            <b-button href="blah">Go somewhere</b-button>
        </b-card>
      </b-col>
      <b-col>
        <b-card
          title="Card Title"
          img-src="" img-alt="Image" img-top
          tag="article"
          style="max-widt: 20rem;"
          class="mb-2">
            <b-card-text>Some quick text</b-card-text>
            <b-button href="blah">Go somewhere</b-button>
        </b-card>
      </b-col>
    </b-row><b-row width="100%">
      <b-col width="2">
        <b-card
          title="Card Title"
          img-src="" img-alt="Image" img-top
          tag="article"
          style="max-widt: 20rem;"
          class="mb-2">
            <b-card-text>Some quick text</b-card-text>
            <b-button href="blah">Go somewhere</b-button>
        </b-card>
      </b-col>
      <b-col>
        <b-card
          title="Card Title"
          img-src="" img-alt="Image" img-top
          tag="article"
          style="max-widt: 20rem;"
          class="mb-2">
            <b-card-text>Some quick text</b-card-text>
            <b-button href="blah">Go somewhere</b-button>
        </b-card>
      </b-col>
      <b-col>
        <b-card
          title="Card Title"
          img-src="" img-alt="Image" img-top
          tag="article"
          style="max-widt: 20rem;"
          class="mb-2">
            <b-card-text>Some quick text</b-card-text>
            <b-button href="blah">Go somewhere</b-button>
        </b-card>
      </b-col>
    </b-row>

  </div>
</template>
<script>
export default {
  mounted: async function() {      
    this.$store.commit('breadcrumbs/setBreadcrumbs', 
    [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Events',
          active: true
        }
      ])
  }
}
</script>
