
import axios from 'axios'

export default function upload(formData, showId, token) {
    const url = `https://api.theatreers.com/show/show/${showId}/image`;
    
    return axios.post(url, formData, {
        headers: {
            'Authorization': `Bearer ${token.accessToken}`
        }
    })
    // get data
    .then(x => x.data)
};