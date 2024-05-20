namespace DemoApi.Models.Request;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PageSort
{
    Asc,
    Desc
}
