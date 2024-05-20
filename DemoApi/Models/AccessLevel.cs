namespace DemoApi.Models;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccessLevel
{
    Confidential = 0,
    Private = 1,
    Public = 2,
}
