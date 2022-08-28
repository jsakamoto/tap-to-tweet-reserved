using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Controllers;

public class AuthenticationController : Controller
{
    public AuthenticationController()
    {
    }

    [HttpGet("/auth/signin")]
    public IActionResult OnSignIn()
    {
        return this.Challenge(
            new AuthenticationProperties
            {
                RedirectUri = "/",
                IsPersistent = true // これが必須
            },
            TwitterDefaults.AuthenticationScheme);
    }

    [HttpPost("/auth/signout")]
    public IActionResult OnSignOut()
    {
        return this.SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("/api/auth/currentuser")]
    public AuthUserInfo GetCurrentUser()
    {
        var name = this.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? this.User.Identity?.Name ?? "";
        return new AuthUserInfo { Name = name };
    }
}
