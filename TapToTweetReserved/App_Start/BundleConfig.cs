﻿using System.Web;
using System.Web.Optimization;

namespace TapToTweetReserved
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.live.js",
                        "~/Scripts/jquery.toggleClass.js",
                        "~/Scripts/site.js"
                        ));
            
            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-resource.js",
                        "~/Scripts/angular-route.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundle.js").Include(
                        "~/Client/app.js",
                        "~/Client/models/*.js",
                        "~/Client/services/*.js",
                        "~/Client/filters/*.js",
                        "~/Client/controllers/editorEditControllerBase.js",
                        "~/Client/controllers/editorEditController.js",
                        "~/Client/controllers/editorAddNewController.js",
                        "~/Client/controllers/editorHomeController.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));
        }
    }
}
