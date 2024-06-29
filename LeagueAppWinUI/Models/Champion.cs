using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LeagueAppWinUI.Models;
internal class Champion
{
    [JsonProperty("key")]
    public int key { get; set; }

    [JsonProperty("name")]
    public string? name { get; set; }

    [JsonProperty("title")]
    public string? title { get; set; }

    [JsonProperty("id")]
    public string? id { get; set; }

    public Champion() {
    
    }
}
