import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'


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
  routes: [
    { path: '/event', name: 'getevents', component: GetEvents },
    { path: '/event/:id', name: 'getevent', component: GetEvent },
    { path: '/group', name: 'getgroups', component: GetGroups },
    { path: '/group/:id', name: 'getgroup', component: GetGroup },
    { path: '/show', name: 'getshows', component: GetShows },
    { path: '/show/create', name: 'createshow', component: CreateShow },
    { path: '/show/:id', name: 'getshow', component: GetShow},
    { path: '/show/:id/edit', name: 'editshow', component: EditShow },
    { path: '/', name: 'root', component: Home }
  ]
})

Vue.use(Router)