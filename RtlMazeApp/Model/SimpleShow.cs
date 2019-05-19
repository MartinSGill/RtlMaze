using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RtlMazeApp.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleShow
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("cast")] public IList<SimpleCast> Cast { get; set; }
    }
}
