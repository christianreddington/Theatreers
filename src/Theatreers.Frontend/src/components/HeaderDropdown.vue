<template>
  <div>
  <b-nav-item-dropdown text="Account" right>
    <!-- Anonymous -->
    <strong v-if="!user">Anonymous</strong>
    <b-dropdown-item @click="login" v-if="!user"
      ><i class="fa fa-lock" />Login</b-dropdown-item
    >
    <b-dropdown-item :href="forgotPasswordLink" v-if="!user">Forgot Password</b-dropdown-item
    >
    <!-- Authenticated -->
    <strong v-if="user">{{ user.name }}</strong>
    <b-dropdown-item v-if="user" :href="editProfileLink"
      >Edit Profile</b-dropdown-item
    >
    <b-dropdown-item @click="logout" v-if="user"><i class="fa fa-lock" /> Logout</b-dropdown-item>
    <b-dropdown-item @click="accesstoken" v-if="user"><i class="fa fa-lock" /> AccessToken</b-dropdown-item>
  </b-nav-item-dropdown>
  <b-nav-item-dropdown text="Admin" right v-if="admin">
    <b-dropdown-item to="/admin/">Admin Home</b-dropdown-item>
    <b-dropdown-item to="/admin/permissions">Admin Permissions</b-dropdown-item>
  </b-nav-item-dropdown>
  </div>
</template>

<script>
import { config } from '../config'
export default {
  data () {
    return {
      editProfileLink: 'https://theatreers.b2clogin.com/theatreers.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_SiPe&client_id=' + config.clientId + '&redirect_uri= ' + config.redirectUri + '&nonce=defaultNonce&scope=openid&response_type=id_token',
      forgotPasswordLink: 'https://theatreers.b2clogin.com/theatreers.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_SSPR&client_id=' + config.clientId + '&redirect_uri=' + config.redirectUri + '&nonce=defaultNonce&scope=openid&response_type=id_token&prompt=login'
    
    }
  },
  mounted () {
    if (this.$AuthService.getAccount() != null) {
      this.user = this.$AuthService.getAccount()
    }
  },
  methods: {
    accesstoken(){
      this.$AuthService.acquireToken({scopes: ["https://theatreers.onmicrosoft.com/show-api/user_impersonation"]}).then(
        response => {
          console.log(response);
        });
    },
    login() {
      this.$AuthService.login();
    },
    logout() {
      this.$AuthService.logout();
    }
  },
  computed: {
    user: function () {
      return this.$AuthService.getAccount();
    },
    admin: function(){
      try {
      var permissions = this.user.idTokenClaims.extension_permissions
      if (permissions.includes('admin:owner') || permissions.includes('admin:contributor')) {
        return true
      }

      return false
      } catch {
        return false
      }
    }
  }
};
</script>