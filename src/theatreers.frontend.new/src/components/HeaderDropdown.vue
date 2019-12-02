<template>
  <b-nav-item-dropdown text="Account" right>
    <!-- Anonymous -->
    <strong v-if="!user">Anonymous</strong>
    <b-dropdown-item @click="login" v-if="!user"
      ><i class="fa fa-lock" />Login</b-dropdown-item
    >
    <b-dropdown-item :href="forgotPasswordLink"
      >Forgot Password</b-dropdown-item
    >
    <!-- Authenticated -->
    <strong v-if="user">{{ user.name }}</strong>
    <b-dropdown-item v-if="user" :href="editProfileLink"
      >Edit Profile</b-dropdown-item
    >
    <b-dropdown-item @click="logout" v-if="user"
      ><i class="fa fa-lock" /> Logout</b-dropdown-item
    >
  </b-nav-item-dropdown>
</template>