using Newtonsoft.Json;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{
  public class ShowObject : ShowDomainObject
  {
    public ShowObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "ShowName", Mandatory = true });
    }
    [JsonProperty("showName")]
    public string ShowName { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("composer")]
    public string Composer { get; set; }
    [JsonProperty("author")]
    public string Author { get; set; }
    [JsonProperty("songs")]
    public List<Song> Songs { get; set; }
    [JsonProperty("characters")]
    public List<Character> Characters { get; set; }
  }

  public class Song
  {
    [JsonProperty("name")]    
    public string Name { get; set; }
    [JsonProperty("low")]
    public int Low { get; set; }
    [JsonProperty("high")]
    public int High { get; set; }
    [JsonProperty("key")]
    public string Key { get; set; }
    [JsonProperty("participants")]
    public List<string> Participants { get; set; }
  }

  public class Characters
  {
    [JsonProperty("name")]    
    public string Name { get; set; }
    [JsonProperty("low")]
    public int Low { get; set; }
    [JsonProperty("high")]
    public int High { get; set; }
    [JsonProperty("vocalType")]
    public string VocalType { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
  }
}