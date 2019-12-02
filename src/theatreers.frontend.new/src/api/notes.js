/**
 * Mocking client-server processing
 */
const _notes = [
  {
    'value': 21,
    'text': 'A0'
  },
  {
    'value': 22,
    'text': 'A#0 / B♭0'
  },
  {
    'value': 23,
    'text': 'B0'
  },
  {
    'value': 24,
    'text': 'C1'
  },
  {
    'value': 25,
    'text': 'C#1 / D♭1'
  },
  {
    'value': 26,
    'text': 'D1'
  },
  {
    'value': 27,
    'text': 'D#1 / E♭1'
  },
  {
    'value': 28,
    'text': 'E1'
  },
  {
    'value': 29,
    'text': 'F1'
  },
  {
    'value': 30,
    'text': 'F#1 / G♭1'
  },
  {
    'value': 31,
    'text': 'G1'
  },
  {
    'value': 32,
    'text': 'G#1 / A♭1'
  },
  {
    'value': 33,
    'text': 'A1'
  },
  {
    'value': 34,
    'text': 'A#1 / B♭1'
  },
  {
    'value': 35,
    'text': 'B1'
  },
  {
    'value': 36,
    'text': 'C2'
  },
  {
    'value': 37,
    'text': 'C#2 / D♭2'
  },
  {
    'value': 38,
    'text': 'D2'
  },
  {
    'value': 39,
    'text': 'D#2 / E♭2'
  },
  {
    'value': 40,
    'text': 'E2'
  },
  {
    'value': 41,
    'text': 'F2'
  },
  {
    'value': 42,
    'text': 'F#2 / G♭2'
  },
  {
    'value': 43,
    'text': 'G2'
  },
  {
    'value': 44,
    'text': 'G#2 / A♭2'
  },
  {
    'value': 45,
    'text': 'A2'
  },
  {
    'value': 46,
    'text': 'A#2 / B♭2'
  },
  {
    'value': 47,
    'text': 'B2'
  },
  {
    'value': 48,
    'text': 'C3'
  },
  {
    'value': 49,
    'text': 'C#3 / D♭3'
  },
  {
    'value': 50,
    'text': 'D3'
  },
  {
    'value': 51,
    'text': 'D#3 / E♭3'
  },
  {
    'value': 52,
    'text': 'E3'
  },
  {
    'value': 53,
    'text': 'F3'
  },
  {
    'value': 54,
    'text': 'F#3 / G♭3'
  },
  {
    'value': 55,
    'text': 'G3'
  },
  {
    'value': 56,
    'text': 'G#3 / A♭3'
  },
  {
    'value': 57,
    'text': 'A3'
  },
  {
    'value': 58,
    'text': 'A#3 / B♭3'
  },
  {
    'value': 59,
    'text': 'B3'
  },
  {
    'value': 60,
    'text': 'C4'
  },
  {
    'value': 61,
    'text': 'C#4 / D♭4'
  },
  {
    'value': 62,
    'text': 'D4'
  },
  {
    'value': 63,
    'text': 'D#4 / E♭4'
  },
  {
    'value': 64,
    'text': 'E4'
  },
  {
    'value': 65,
    'text': 'F4'
  },
  {
    'value': 66,
    'text': 'F#4 / G♭4'
  },
  {
    'value': 67,
    'text': 'G4'
  },
  {
    'value': 68,
    'text': 'G#4 / A♭4'
  },
  {
    'value': 69,
    'text': 'A4'
  },
  {
    'value': 70,
    'text': 'A#4 / B♭4'
  },
  {
    'value': 71,
    'text': 'B4'
  },
  {
    'value': 72,
    'text': 'C5'
  },
  {
    'value': 73,
    'text': 'C#5 / D♭5'
  },
  {
    'value': 74,
    'text': 'D5'
  },
  {
    'value': 75,
    'text': 'D#5 / E♭5'
  },
  {
    'value': 76,
    'text': 'E5'
  },
  {
    'value': 77,
    'text': 'F5'
  },
  {
    'value': 78,
    'text': 'F#5 / G♭5'
  },
  {
    'value': 79,
    'text': 'G5'
  },
  {
    'value': 80,
    'text': 'G#5 / A♭5'
  },
  {
    'value': 81,
    'text': 'A5'
  },
  {
    'value': 82,
    'text': 'A#5 / B♭5'
  },
  {
    'value': 83,
    'text': 'B5'
  },
  {
    'value': 84,
    'text': 'C6'
  },
  {
    'value': 85,
    'text': 'C#6 / D♭5'
  },
  {
    'value': 86,
    'text': 'D6'
  },
  {
    'value': 87,
    'text': 'D#6 / E♭6'
  },
  {
    'value': 88,
    'text': 'E6'
  },
  {
    'value': 89,
    'text': 'F6'
  },
  {
    'value': 90,
    'text': 'F#6 / G♭6'
  },
  {
    'value': 91,
    'text': 'G6'
  },
  {
    'value': 92,
    'text': 'G#6 / A♭6'
  },
  {
    'value': 93,
    'text': 'A6'
  },
  {
    'value': 94,
    'text': 'A#6 / B♭6'
  },
  {
    'value': 95,
    'text': 'B6'
  },
  {
    'value': 96,
    'text': 'C7'
  },
  {
    'value': 97,
    'text': 'C#7 / D♭7'
  },
  {
    'value': 98,
    'text': 'D7'
  },
  {
    'value': 99,
    'text': 'D#7 / E♭7'
  },
  {
    'value': 100,
    'text': 'E7'
  },
  {
    'value': 101,
    'text': 'F7'
  },
  {
    'value': 102,
    'text': 'F#7 / G♭7'
  },
  {
    'value': 103,
    'text': 'G7'
  },
  {
    'value': 104,
    'text': 'G#7 / A♭7'
  },
  {
    'value': 105,
    'text': 'A7'
  },
  {
    'value': 106,
    'text': 'A#7 / B♭7'
  },
  {
    'value': 107,
    'text': 'B7'
  },
  {
    'value': 108,
    'text': 'C8'
  }
]
  
  export default {
    getNotes (cb) {
      setTimeout(() => cb(_notes), 100)
    },
  
    buyNotes (notes, cb, errorCb) {
      setTimeout(() => {
        // simulate random checkout failure.
        (Math.random() > 0.5 || navigator.userAgent.indexOf('PhantomJS') > -1)
          ? cb()
          : errorCb()
      }, 100)
    }
  }