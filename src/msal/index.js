import * as Msal from 'msal'
import config from '../config'
// With a lot of help from ; https://stackoverflow.com/questions/52944052/creating-a-single-instance-of-a-class-within-a-vue-application
// https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-core/src/UserAgentApplication.ts

export default class AuthService {
  constructor () {
    // let redirectUri = window.location.origin;
    let redirectUri = config.redirecturl
    let postLogoutRedirectUri = config.redirecturl
    this.graphUrl = config.graphendpoint
    this.applicationConfig = {
      clientID: config.clientid,
      authority: config.authority
    }
    this.app = new Msal.UserAgentApplication(
      this.applicationConfig.clientID,
      this.applicationConfig.authority,
      () => {
      },
      {
        redirectUri,
        postLogoutRedirectUri,
        cacheLocation: 'localStorage'
      }
    )
  }

  // Core Functionality
  loginPopup () {
    return this.app.loginPopup().then(
      idToken => {
        const user = this.app.getUser()
        if (user) {
          return user
        } else {
          return null
        }
      },
      () => {
        return null
      }
    )
  }

  loginRedirect () {
    this.app.loginRedirect()
  }

  logout () {
    this.app._user = null
    this.app.logout()
  }

  getAccessToken (scopes) {
    return this.app.acquireTokenSilent(scopes).then(
      accessToken => {
        return accessToken
      })
      .catch(function (error) {
        console.log(error)
        return this.app
          .acquireTokenPopup(scopes)
          .then(
            accessToken => {
              return accessToken
            }
          )
          .catch(function (error) {
            console.log('could not get offers', error)
          })
      })
  }

  getApi (uri, token) {
    const headers = { 
      'Authorization': `Bearer ${token}`
    }
    return fetch(`${uri}`, {headers: headers, method: 'GET'})
  }

  postApi (uri, body, token) {
   // const headers = new Headers({ Authorization: `Bearer ${token}` })
    const headers = {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
    return fetch(`${uri}`, {headers: headers, method: 'POST', body: body})
  }

  // Utility
  getUser () {
    return this.app.getUser()
  }
}
