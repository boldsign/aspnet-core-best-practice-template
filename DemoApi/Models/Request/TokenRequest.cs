namespace DemoApi.Models.Request;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class TokenRequest
{
    [Required]
    [DefaultValue("NormalUser")]
    public required string Username { get; init; }

    [Required]
    [DefaultValue("SuperSecretPassword")]
    public required string Password { get; init; }
}
