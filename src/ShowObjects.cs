
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Search;

namespace Theatreers.Show
{

    public class ShowBaseObject
    {
        public string showId { get; set; }
    }

    public class ShowMessage : ShowBaseObject {
        public string id { get; set; }
        public string showId { get; set; }
        //public string showId {get; set;}
        public string doctype {get; set;}
        //public IList<NewsObject> news {get; set;}
        public string showName {get; set;}
        public string description {get; set;}
        //public string relatesto {get; set;}
    }

    public class DecoratedShowMessage : ShowMessage {        
        public MessageHeaders MessageProperties {get; set;}
    }

    public class MessageHeaders
    {
        public string RequestCorrelationId {get; set;}
        public string RequestCreatedAt {get; set;}
        public string RequestStatus {get; set;}
    }

    public class ImageObject : ShowBaseObject
    {
        public string showId { get; set; }
        public string imageId {get; set;}
        public string contentUrl {get; set;}
        public string hostPageUrl {get; set;}
        public string name {get; set; }
        public string doctype { get; set; }
    }
    public class NewsObject : ShowBaseObject
    {
        public string showId { get; set; }
        public string DatePublished { get; set; }
        public string BingId { get; set; }
        public string name {get; set;}
        public string url {get; set; }
        public string doctype { get; set; }
    }

    public class AlphabetisedShow
    {
        public string showId { get; set; }
        public string showName { get; set; }
        public string partition { get; set; }
    }

    public class EnvelopedMessage {
        public MessageHeaders MessageProperties {get; set;}
        public string RequestObject {get; set;}
    }

    public static class MessageHelper {
        // Decorator - Takes the input and augments the object with an additional set of properties to the JSON body as am additional header level property (MessageProperties)
        // Returns a serialized string with the original object at the root
        public static string DecorateJsonBody(string inputjson, Dictionary<string, JToken> HeaderProperties)
        {
            JObject jObject = JObject.Parse(inputjson);
            jObject.Add("MessageProperties", JObject.FromObject(HeaderProperties));
            return jObject.ToString();
        }

        // Classic 'Envelope' - Takes the input and augments the object with an additional set of properties to the JSON body as an additional header level property (EnvelopeProperties) 
        // whilst appending the original object as a new property called 'requestobject' and returns a serialized string
        public static string EnvelopeJSONBody(string inputjson, Dictionary<string, JToken> HeaderProperties)
        {
            JObject jObject = new JObject();
            jObject.Add("RequestObject", JObject.Parse(inputjson));
            jObject.Add("EnvelopeProperties",JObject.FromObject(HeaderProperties)); 
            return jObject.ToString();
        }
    }
}