import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

// Authentication
const AuthCallback = () => import('@/views/auth/Callback')
const AuthLogin = () => import('@/views/auth/Login')
const AuthRegister = () => import('@/views/auth/Register')
const AuthTest = () => import('@/views/auth/Test')

// Event
const GetEvent = () => import('@/views/event/GetEvent')
const GetEvents = () => import('@/views/event/GetEvents')

// Group
const GetGroup = () => import('@/views/group/GetGroup')
const GetGroups = () => import('@/views/group/GetGroups')

// Show
const CreateShow = () => import('@/views/show/CreateShow')
const EditShow = () => import('@/views/show/EditShow')
const GetShow = () => import('@/views/show/GetShow')
const GetShows = () => import('@/views/show/GetShows')

export default new Router({
  // mode: 'history', // https://router.vuejs.org/api/#mode
  routes: [
    { path: '/auth',
      name: 'Auth',
      redirect: '/auth/login',
      component: {
        render (c) { return c('router-view') }
      },
      children: [
        { path: 'callback', name: 'Callback', component: AuthCallback },
        { path: 'test', name: 'Test', component: AuthTest },
        { path: 'login', name: 'Login', component: AuthLogin },
        { path: 'register', name: 'Register', component: AuthRegister }
      ]
    },
    { path: '/event', name: 'getevents', component: GetEvents },
    { path: '/event/:id', name: 'getevent', component: GetEvent },
    { path: '/group', name: 'getgroups', component: GetGroups },
    { path: '/group/:id', name: 'getgroup', component: GetGroup },
    { path: '/show', name: 'getshows', component: GetShows },
    { path: '/show/:id', name: 'getshow', component: GetShow},
    { path: '/show/:id/edit', name: 'editshow', component: EditShow },
    { path: '/show/create', name: 'createshow', component: CreateShow },
    { path: '/', name: 'root', component: Home }
  ]
})
