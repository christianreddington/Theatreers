import * as Msal from 'msal'
import config from '../config'

export default class AuthService {

  constructor() {
    this.app = new Msal.UserAgentApplication(
      {
        auth: {
          clientId: config.clientId,
          authority: config.authority,
          validateAuthority: config.validateAuthority,
          redirectUri: config.redirectUri
        },
        request: {
          scopes: ["https://theatreers.onmicrosoft.com/show-api/user_impersonation", "https://theatreers.onmicrosoft.com/permissions/user_impersonation"]
        },
        cache: {
          cacheLocation: 'localStorage',
          storeAuthStateInCookie: true
        }
      }
    )
    this.app.handleRedirectCallback((error, response) => {
      console.log(error);
      console.log(response);
      // if error is not null, something went wrong
      // if not, response is a successful login response
    });
  }


  login() {
    var loginRequest = {
      scopes: config.scopes
    };
    this.app.loginRedirect(loginRequest).then(function (loginResponse) {
      console.log("login response" + loginResponse);
      var tokenRequest = {
        scopes: config.scopes
      };
      this.app.acquireTokenSilent(tokenRequest).then(function (tokenResponse) {
        console.log("token response" + tokenResponse);
        //const user = this.app.getUser()
      }).catch(function (error) {
        console.log(error);
        this.app.acquireTokenPopup(tokenRequest).then(function (tokenResponse) {
          console.log(tokenResponse);
        }).catch(function (error) {
          console.log("Error acquiring the popup:\n" + error);
        });
      })
    }).catch(function (error) {
      console.log("Error during login:\n" + error);
    });
  };

  logout() {
    this.app.logout();
  }

  getAccount() {
    return this.app.getAccount();
  }

  
  getApi(uri) {
    return fetch(uri, { method: 'GET' })
  };
  
  getApiWithToken(uri, token) {
    const headers = {
      'Authorization': 'Bearer ' + token,
      'Content-Type': 'application/json'
    }
    
    return fetch(uri, { headers: headers, method: 'GET' })
  };

  postApi(uri, body, token) {
    const headers = {
      'Authorization': 'Bearer ' + token,
      'Content-Type': 'application/json'
    }
    return fetch(uri, { headers: headers, method: 'POST', body: body })
  };

  putApi(uri, body, token) {
    const headers = {
      'Authorization': 'Bearer ' + token,
      'Content-Type': 'application/json'
    }
    return fetch(uri, { headers: headers, method: 'PUT', body: body })
  };

  acquireToken(scopes) {
    // console.log("Scopes passed in: " + scopes);
    return this.app.acquireTokenSilent(scopes).then(
      accessToken => {
        // console.log("Looking for scopes " + scopes)
        return accessToken
      })
      .catch(function (error) {
        console.log(error)
        return this.app.acquireTokenPopup(scopes).then(
            accessToken => {
              return accessToken
            }
          )
          .catch(function (error) {
            console.log('could not get offers', error)
          })
      })
    }
}