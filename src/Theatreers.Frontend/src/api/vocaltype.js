/**
 * Mocking client-server processing
 */
const _vocalTypes = [
  {
    'value': 'Soprano',
    'text': 'Soprano'
  },
  {
    'value': 'Mezzo Soprano',
    'text': 'Mezzo Soprano'
  },
  {
    'value': 'Alto',
    'text': 'Alto'
  },
  {
    'value': 'Tenor',
    'text': 'Tenor'
  },
  {
    'value': 'Barritone',
    'text': 'Barritone'
  },
  {
    'value': 'Bass',
    'text': 'Bass'
  }
]
  
  export default {
    getVocalTypes (cb) {
      setTimeout(() => cb(_vocalTypes), 100)
    }
  }