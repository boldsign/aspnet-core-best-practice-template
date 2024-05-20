namespace DemoApi.Controllers;

using DemoApi.Models.Request;
using DemoApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("auth")]
[ApiController]
[IgnoreAntiforgeryToken]
[ApiVersion("1")]
[SwaggerTag("Token generation endpoint to get JWT token")]
public class AuthController(IGrpcClientService grpcClientService) : Controller
{
    /// <summary>
    /// Generates a JWT token for the user. Hint 'admin' as username to access the admin protected endpoints.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Token([FromBody] TokenRequest request)
    {
        var token = await grpcClientService.GetAuthTokenAsync(request);

        return this.Ok(token);
    }
}
