<template>
  <b-nav-item-dropdown text="Account" right>
    <!-- Anonymous -->
      <strong v-if="!user">Anonymous</strong>
      <b-dropdown-item @click="login" v-if="!user"><i class="fa fa-lock" />Login</b-dropdown-item>
    <!-- Authenticated -->
      <strong v-if="user">{{ user.name }}</strong>
      <b-dropdown-item @click="logout" v-if="user"><i class="fa fa-lock" /> Logout</b-dropdown-item>
  </b-nav-item-dropdown>
</template>

<script>
export default {
  name: 'HeaderDropdown',
  data: () => {
    return {
      itemsCount: 42,
      user: null
    }
  },
  mounted () {
    this.setUser()
  },
  methods: {
    logout () {
      this.user = null
      this.$AuthService.logout()
    },
    login () {
      // this.$AuthService.loginPopup() // with a popup
      this.$AuthService.loginRedirect() // with a redirect
    },
    setUser () {
      this.user = this.$AuthService.getUser()
    }
  }
}
</script>
