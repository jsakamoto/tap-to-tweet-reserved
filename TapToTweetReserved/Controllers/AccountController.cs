using System;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Newtonsoft.Json;
using TapToTweetReserved.Code;
using TapToTweetReserved.Models;

namespace TapToTweetReserved.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignIn(string provider, string returnUrl)
        {
            provider = provider ?? "twitter";
            return new LamdaResult(_ =>
            {
                OAuthWebSecurity.RequestAuthentication(
                    provider,
                    Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            });
        }

        [HttpPost]
        public ActionResult SignOut()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return Json(new { url = this.Url.Content("~/") });
        }

        [HttpGet]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var result = OAuthWebSecurity.VerifyAuthentication(
                Url.Action("ExternalLoginCallback",
                new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return Redirect("~/");
            }

            RegistUserAndIssueAuthCookie(result);

            return Redirect("~/");
        }

        private void RegistUserAndIssueAuthCookie(AuthenticationResult result)
        {
            var salt = ConfigurationManager.AppSettings["salt"];
            var userExtraData = new UserExtraData {
                UserId = GetHashedText(string.Join("@", salt, result.ProviderUserId, result.Provider)),
                AccessToken = result.ExtraData["accesstoken"],
                AccessSecret = result.ExtraData["accesssecret"]
            };
            var userInfoJson = JsonConvert.SerializeObject(userExtraData);

            var cookie = FormsAuthentication.GetAuthCookie(result.UserName, false);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            ticket.GetType().InvokeMember("_UserData",
                BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance,
                null, ticket, new object[] { userInfoJson });
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(cookie);
        }

        private string GetHashedText(string text)
        {
            return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(text)));
        }
    }
}
