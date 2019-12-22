<template>
  <div id="app">
    <div>
      <b-navbar toggleable="lg" type="dark" variant="info">
        <b-navbar-brand to="/">Theatreers</b-navbar-brand>
        <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
        <b-collapse id="nav-collapse" is-nav>
          <b-navbar-nav>
            <b-nav-item to="/">Home</b-nav-item>
            <b-nav-item to="/show">Shows</b-nav-item>
            <b-nav-item to="/event">Events</b-nav-item>
            <b-nav-item to="/group">Groups</b-nav-item>
          </b-navbar-nav>
          <!-- Right aligned nav items -->
          <b-navbar-nav class="ml-auto">
            <b-nav-form>
              <b-form-input size="sm" class="mr-sm-2" placeholder="Search" />
              <b-button size="sm" class="my-2 my-sm-0" type="submit">Search</b-button>
            </b-nav-form>
            <HeaderDropdown />
          </b-navbar-nav>
        </b-collapse>
      </b-navbar>
    </div>
    <AlertsComponent />
    <BreadcrumbsComponent />
    <b-container class="app-content">
      <router-view></router-view>
    </b-container>
    <Footer />
  </div>
</template>

<script>
import HeaderDropdown from "./components/HeaderDropdown";
import AlertsComponent from "./components/AlertsComponent";
import BreadcrumbsComponent from "./components/BreadcrumbsComponent";
import Footer from "./components/structure/Footer";

export default {
  components: {
    HeaderDropdown,
    AlertsComponent,
    BreadcrumbsComponent,
    Footer
  }
};
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}

#breadcrumbs {
  margin-top: 10px;
}
</style>
