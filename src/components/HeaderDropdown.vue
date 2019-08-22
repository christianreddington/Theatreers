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
import * as auth from '.././msal'

export default {
  name: 'HeaderDropdown',
  data: () => {
    return {
      itemsCount: 42,
      user: null

    }
  },
  mounted () {
    if (msalInstance.getAccount() != null){
      this.user = msalInstance.getAccount()
    }
  },
  methods: {
    logout () {
      this.user = null
      logout()
    },
    login () {
      // this.$AuthService.loginPopup() // with a popup
      login() // with a redirect
      this.user = msalInstance.getAccount()
    },
    setUser () {
      this.user = msalInstance.getAccount()
    }
  }
}
</script>
