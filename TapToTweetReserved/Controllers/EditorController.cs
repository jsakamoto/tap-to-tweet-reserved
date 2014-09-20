using System;
using System.Linq;
using System.Web.Mvc;
using TapToTweetReserved.Models;

namespace TapToTweetReserved.Controllers
{
    [Authorize]
    public class EditorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
