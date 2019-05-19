namespace RtlMazeApp.Model
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleShow
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("cast")] public IList<SimpleCast> Cast { get; set; }
    }
}