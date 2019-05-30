import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)


// Authentication
const AuthCallback = () => import('@/views/auth/Callback')
const AuthLogin = () => import('@/views/auth/Login')
const AuthRegister = () => import('@/views/auth/Register')
const AuthTest = () => import('@/views/auth/Test')

export default new Router({  
  mode: 'history', // https://router.vuejs.org/api/#mode
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
    },    
    {
      path: '/auth',
      redirect: '/auth/login',
      name: 'Auth',
      component: {
        render(c) { return c('router-view') }
      },
      children: [
        {
          path: 'callback',
          name: 'Callback',
          component: AuthCallback
        },
        {
          path: 'test',
          name: 'Test',
          component: AuthTest
        },
        {
          path: 'login',
          name: 'Login',
          component: AuthLogin
        },
        {
          path: 'register',
          name: 'Register',
          component: AuthRegister
        }
      ]
    }
  ]
})
