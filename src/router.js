import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'root',
      component: Home
    },
    { path: '/show/',
      name: 'getshows',
      component: () => import(/* webpackChunkName: "about" */ './views/GetShows.vue')
    },
    { path: '/show/:id',
      name: 'getshow',
      component: () => import(/* webpackChunkName: "about" */ './views/GetShow.vue')
    },
    { path: '/event/',
      name: 'getevents',
      component: () => import(/* webpackChunkName: "about" */ './views/GetEvents.vue')
    },
    { path: '/event/:id',
      name: 'getevent',
      component: () => import(/* webpackChunkName: "about" */ './views/GetEvent.vue')
    },
    { path: '/group/',
      name: 'getgroups',
      component: () => import(/* webpackChunkName: "about" */ './views/GetGroups.vue')
    },
    { path: '/group/:id',
      name: 'getgroup',
      component: () => import(/* webpackChunkName: "about" */ './views/GetGroup.vue')
    }
  ]
})
