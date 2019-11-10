import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

// Admin
const AdminRoot = () => import('@/views/admin/AdminRoot')
const GetPermissions = () => import('@/views/admin/GetPermissions')

// Event
const GetEvent = () => import('@/views/event/GetEvent')
const GetEvents = () => import('@/views/event/GetEvents')

// Group
const GetGroup = () => import('@/views/group/GetGroup')
const GetGroups = () => import('@/views/group/GetGroups')

// Show
const CreateShow = () => import('@/views/show/CreateShow')
const EditShow = () => import('@/views/show/EditShow')
// const GetShow = (resolve) => require(['@/views/show/GetShow.vue'], m => resolve(m.default))
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
    { path: '/show/create/',
      name: 'createshow',
      component: CreateShow,
      async beforeEnter (to, from, next) {
        try {
          var permissions = await msalInstance.getAccount().idTokenClaims.extension_permissions
          if (permissions.includes('show:owner') || permissions.includes('show:contributor')) {
            next()
          }
        } catch (e) {
          next({
            name: 'root' // back to safety route //
          })
        }
      }
    },
    { path: '/show/:id', name: 'getshow', component: GetShow },
    { path: '/show/:id/edit', name: 'editshow', component: EditShow },
    { path: '/admin/',
      name: 'AdminRoot',
      component: AdminRoot,
      async beforeEnter (to, from, next) {
        try {
          var permissions = await msalInstance.getAccount().idTokenClaims.extension_permissions
          if (permissions.includes('admin:owner') || permissions.includes('admin:contributor')) {
            next()
          }
        } catch (e) {
          next({
            name: 'root' // back to safety route //
          })
        }
      }
    },
    { path: '/admin/permissions',
      name: 'GetPermissions',
      component: GetPermissions,
      async beforeEnter (to, from, next) {
        try {
          var permissions = await msalInstance.getAccount().idTokenClaims.extension_permissions
          if (permissions.includes('admin:owner') || permissions.includes('admin:contributor')) {
            next()
          }
        } catch (e) {
          next({
            name: 'root' // back to safety route //
          })
        }
      }
    },
    { path: '/', name: 'root', component: Home }
  ]
})

Vue.use(Router)
