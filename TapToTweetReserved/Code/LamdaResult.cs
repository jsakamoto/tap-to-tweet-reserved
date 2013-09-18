using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TapToTweetReserved.Code
{
    public class LamdaResult : ActionResult
    {
        private Action<ControllerContext> _CallBack;

        public LamdaResult(Action<ControllerContext> callback)
        {
            _CallBack = callback;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            _CallBack(context);
        }
    }
}