/**
 * Mocking client-server processing
 */
const _alerts = [
    {"id": 1, "variant": "success", "message": "You have successfully done the thing"},
    {"id": 2, "variant": "danger", "message": "Sorry, you didn't do the thing"},
    {"id": 3, "variant": "warning", "message": "Be warned... the thing didn't happen"},
    {"id": 4, "variant": "info", "message": "Just FYI.. Something about the thing"},
  ]
  
  export default {
    getAlerts (cb) {
      setTimeout(() => cb(_alerts), 100)
    },
  
    buyAlerts (alerts, cb, errorCb) {
      setTimeout(() => {
        // simulate random checkout failure.
        (Math.random() > 0.5 || navigator.userAgent.indexOf('PhantomJS') > -1)
          ? cb()
          : errorCb()
      }, 100)
    }
  }