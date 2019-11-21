using System.Web.Optimization;

namespace com.SML.BIGTRONS.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-1.10.2.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                      "~/Scripts/modernizr.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/prefixfree").Include(
                        "~/Scripts/prefixfree*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                        "~/Scripts/sidebarmenu.js",
                        "~/Scripts/date.js",
                        "~/Scripts/global.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                        "~/Scripts/jquery.signalR-2.0.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                        "~/Scripts/ckeditor/ckeditor.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Styles/bootstrap.min.css",
                      "~/Content/Styles/font.css",
                      "~/Content/Styles/reset.css",
                      "~/Content/Styles/style.css",
                      "~/Content/Styles/sidebarmenu.css"));
        }
    }
}