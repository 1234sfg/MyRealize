using System.Web;
using System.Web.Optimization;

namespace Realization
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/vue").Include(
                      "~/Scripts/vue.js"));
            bundles.Add(new ScriptBundle("~/bundles/swiper").Include(
                      "~/Scripts/swiper-3.4.2.min.js"));

            //css打包。打包后只用请求一次。如果再次请求，优先找缓存。
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css",
                       "~/Content/swiper-3.4.2.min.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/index").Include(
                      "~/Content/index.css",
                      "~/Content/xyz_indexbody.css"

                      ));
            //  ~/Content/css随便起名的，用@Styles.Render("~/Content/css","~/Content/css n ") 来调用
        }
    }
}
