using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RtlMazeApp.Model
{
    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleCast
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("birthday")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Birthday { get; set; }
    }
}