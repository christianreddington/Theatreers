//import * as Msal from 'msal'
//import * as Msal from 'msal';
//import config from '../config'
// With a lot of help from ; https://stackoverflow.com/questions/52944052/creating-a-single-instance-of-a-class-within-a-vue-msalInstancelication
// https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-core/src/UserAgentmsalInstancelication.ts


export var config = {
  clientId: "74568295-9e88-47db-bd81-e99a23fdcee8",
  redirecturl: 'https://www.theatreers.com',
  //redirectUrl: "http://localhost:8080",
  authority: "https://theatreers.b2clogin.com/theatreers.onmicrosoft.com/B2C_1_SiUpIn",
  showScopes: "https://theatreers.onmicrosoft.com/show-api/user_impersonation",
  apiUrl: "https://th-show-dev-neu-func.azurewebsites.net",
  scopes: ["https://theatreers.onmicrosoft.com/show-api/user_impersonation"]
};

export var authConfig = {
  auth: {
    clientId: config.clientId,
    authority: config.authority,
    validateAuthority: false,
    redirectUri: config.redirectUrl
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: true
  }
};

export const msalInstance = new Msal.UserAgentApplication(authConfig);
msalInstance.handleRedirectCallback((error, response) => {
  // if error is not null, something went wrong
  // if not, response is a successful login response
});


export function login() {
  var loginRequest = {
    scopes: config.scopes
  };
  msalInstance.loginPopup(loginRequest).then(function (loginResponse) {
    var tokenRequest = {
        scopes: config.scopes
    };
    msalInstance.acquireTokenSilent(tokenRequest).then(function (tokenResponse) {
      const user = this.msalInstance.getUser()
        }).catch(function (error) {
          msalInstance.acquireTokenPopup(tokenRequest).then(function (tokenResponse) {
                }).catch (function (error) {
                    console.log("Error acquiring the popup:\n" + error);
                });
        })
    }).catch (function (error) {
        console.log("Error during login:\n" + error);
  });
};

// Core Functionality
/*export function loginPopup() {
  var loginRequest = {
    scopes: ['user.read']
  }
  return this.msalInstance.loginPopup(loginRequest).then(
    idToken => {
      const user = this.msalInstance.getUser()
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
};

export function loginRedirect() {
  var loginRequest = ["https://theatreers.onmicrosoft.com/show-api/user_impersonation"]
  this.msalInstance.loginRedirect(loginRequest)
};*/

export function logout() {
  Msal._user = null
  Msal.logout()
};

export function getAccessToken(scopes) {
  return Msal.acquireTokenSilent(scopes).then(
    accessToken => {
      return accessToken
    })
    .catch(function (error) {
      console.log(error)
      return this.msalInstance
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
};

export function getApiWithoutToken(uri) {
  return fetch(`${uri}`, { method: 'GET' })
};

export function getApiWithToken(uri, token = null) {
  const headers = {
    'Authorization': `Bearer ${token}`
  }
  return fetch(`${uri}`, { headers: headers, method: 'GET' })
};

export function postApiWithToken(uri, body, token) {
  // const headers = new Headers({ Authorization: `Bearer ${token}` })
  const headers = {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'msalInstancelication/json'
  }
  return fetch(`${uri}`, { headers: headers, method: 'POST', body: body })
};