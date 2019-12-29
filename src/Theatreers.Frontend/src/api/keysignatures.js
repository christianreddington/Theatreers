/**
 * Mocking client-server processing
 */
const _keySignatures = [
  {
    'value': 'C Major',
    'text': 'C Major'
  },
  {
    'value': 'F Major',
    'text': 'F Major'
  },
  {
    'value': 'B♭ Major',
    'text': 'B♭ Major'
  },
  {
    'value': 'E♭ Major',
    'text': 'E♭ Major'
  },
  {
    'value': 'A♭ Major',
    'text': 'A♭ Major'
  },
  {
    'value': 'D♭ Major',
    'text': 'D♭ Major'
  },
  {
    'value': 'G♭ Major',
    'text': 'G♭ Major'
  },
  {
    'value': 'G Major',
    'text': 'G Major'
  },
  {
    'value': 'D Major',
    'text': 'D Major'
  },
  {
    'value': 'A Major',
    'text': 'A Major'
  },
  {
    'value': 'E Major',
    'text': 'E Major'
  },
  {
    'value': 'B Major',
    'text': 'B Major'
  },
  {
    'value': 'F# Major',
    'text': 'F# Major'
  },
  {
    'value': 'C# Major',
    'text': 'C# Major'
  },
  {
    'value': 'A Minor',
    'text': 'A Minor'
  },
  {
    'value': 'D Minor',
    'text': 'D Minor'
  },
  {
    'value': 'G Minor',
    'text': 'G Minor'
  },
  {
    'value': 'C Minor',
    'text': 'C Minor'
  },
  {
    'value': 'F Minor',
    'text': 'F Minor'
  },
  {
    'value': 'B♭ Minor',
    'text': 'B♭ Minor'
  },
  {
    'value': 'E♭ Minor',
    'text': 'E♭ Minor'
  },
  {
    'value': 'A♭ Minor',
    'text': 'A♭ Minor'
  },
  {
    'value': 'E Minor',
    'text': 'E Minor'
  },
  {
    'value': 'B Minor',
    'text': 'B Minor'
  },
  {
    'value': 'F# Minor',
    'text': 'F# Minor'
  },
  {
    'value': 'C# Minor',
    'text': 'C# Minor'
  },
  {
    'value': 'G# Minor',
    'text': 'G# Minor'
  },
  {
    'value': 'D# Minor',
    'text': 'D# Minor'
  },
  {
    'value': 'A# Minor',
    'text': 'A# Minor'
  },
]
  
  export default {
    getKeySignatures (cb) {
      setTimeout(() => cb(_keySignatures), 100)
    }
  }