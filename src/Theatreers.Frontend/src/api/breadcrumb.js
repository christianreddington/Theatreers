/**
 * Mocking client-server processing
 */
const _breadcrumbs = [
  {
    text: "Home",
    href: "#"
  },
  {
    text: "Library",
    active: true
  }
]
  
  export default {
    getBreadcrumbs (cb) {
      setTimeout(() => cb(_breadcrumbs), 100)
    },
  
    buyBreadcrumbs (breadcrumbs, cb, errorCb) {
      setTimeout(() => {
        // simulate random checkout failure.
        (Math.random() > 0.5 || navigator.userAgent.indexOf('PhantomJS') > -1)
          ? cb()
          : errorCb()
      }, 100)
    }
  }