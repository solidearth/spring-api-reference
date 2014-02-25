using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace SpringApi.Controllers
{
    public class AppController : Controller
    {
        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Search", "App");
        }

        /// <summary>
        /// /search
        /// </summary>
        /// <returns></returns>
        public ActionResult Search()
        {
            var url = GetUrl("search");
            var page = (Request.Params["page"] != null) ? Request.Params["page"] : "0";
            var results = CallApi(string.Format("{0}&MlsStatusin=Active&class=Residential&expand=true&page={1}", url, page));
            ViewBag.MapBoxKey = ConfigurationManager.AppSettings["MapBoxKey"];

            return View(results);
        }

        /// <summary>
        /// /listing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Listing(string id)
        {
            var url = GetUrl("listing", id: id);
            var results = CallApi(string.Format("{0}&expand=true", url));
            ViewBag.MapBoxKey = ConfigurationManager.AppSettings["MapBoxKey"];

            return View(results);
        }

        /// <summary>
        /// /listingcounters
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ListingCounters(string id)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Search", "App");
            }
            var url = GetUrl("listing", function: "counters", id: id, secure: true);
            var results = CallApi(url);

            return View(results);
        }

        /// <summary>
        /// Returns a url to the API endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="subendpoint"></param>
        /// <param name="id"></param>
        /// <param name="secure"></param>
        /// <returns></returns>
        private string GetUrl(string endpoint, string function = null, string id = null, bool secure = false)
        {
            var subendpoint = (function != null) ? string.Format("/{0}", function) : "";
            var identifier = (id != null) ? string.Format("/{0}", id) : "";
            var key = (secure) ? string.Format("access_token={0}", Session["token"]) : string.Format("api_key={0}", ConfigurationManager.AppSettings["SpringAPIKey"]);
            var sandbox = (bool.Parse(ConfigurationManager.AppSettings["Sandbox"])) ? "sandbox/" : "";

            return string.Format("https://{0}/{1}{2}/{3}/{4}{5}{6}?format=json&{7}",
                ConfigurationManager.AppSettings["ApiBaseUrl"],
                sandbox,
                ConfigurationManager.AppSettings["ApiVersion"],
                endpoint,
                ConfigurationManager.AppSettings["Site"],
                subendpoint,
                identifier,
                key
            );
        }

        /// <summary>
        /// Returns a dynamic object using the json returned from the given url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private dynamic CallApi(string url)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(url);
                dynamic result = JsonConvert.DeserializeObject(response);

                return result;
            }
        }
    }
}
