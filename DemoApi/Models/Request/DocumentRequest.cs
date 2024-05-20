namespace DemoApi.Models.Request;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class DocumentRequest
{
    [Required]
    [StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "The {0} is required")]
    [JsonPropertyName("accessLevel")]
    public AccessLevel? AccessLevel { get; set; }
}
