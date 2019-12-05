<template>
  <b-nav-item-dropdown text="Account" right>
    <!-- Anonymous -->
      <strong v-if="!user">Anonymous</strong>
      <b-dropdown-item @click="login" v-if="!user"><i class="fa fa-lock" />Login</b-dropdown-item>
      <b-dropdown-item :href="forgotPasswordLink">Forgot Password</b-dropdown-item>
    <!-- Authenticated -->
      <strong v-if="user">{{ user.name }}</strong>
      <b-dropdown-item v-if="user" :href="editProfileLink">Edit Profile</b-dropdown-item>
      <b-dropdown-item @click="logout" v-if="user"><i class="fa fa-lock" /> Logout</b-dropdown-item>
  </b-nav-item-dropdown>
</template>

<script>

export default {
  name: 'HeaderDropdown',
  data: () => {
    return {
      itemsCount: 42,
      user: msalInstance.getAccount(),
      editProfileLink: 'https://theatreers.b2clogin.com/theatreers.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_SiPe&client_id=' + this.config.clientId + '&redirect_uri= ' + this.config.redirectUrl + '&nonce=defaultNonce&scope=openid&response_type=id_token',
      forgotPasswordLink: 'https://theatreers.b2clogin.com/theatreers.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_SSPR&client_id=' + this.config.clientId + '&redirect_uri=' + this.config.redirectUrl + '&nonce=defaultNonce&scope=openid&response_type=id_token&prompt=login'
    }
  },
  mounted () {
    if (msalInstance.getAccount() != null) {
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
