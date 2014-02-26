using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace SpringApi.Controllers
{
    public class AuthController : Controller
    {
        /// <summary>
        /// /login
        /// </summary>
        /// <returns></returns>
        public void Login()
        {
            var sandbox = (bool.Parse(ConfigurationManager.AppSettings["Sandbox"])) ? "sandbox/" : "";
            Response.Redirect(string.Format(ConfigurationManager.AppSettings["OauthUrl"],
                sandbox,
                ConfigurationManager.AppSettings["SpringAPIKey"],
                "http://localhost:58974/oauth"
            ));
        }

        /// <summary>
        /// /logout
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session["token"] = null;
            return RedirectToAction("Search", "App");
        }

        /// <summary>
        /// /oauth
        /// </summary>
        /// <returns></returns>
        public ActionResult Oauth()
        {
            var token = Request.Params["access_token"];
            Session["token"] = token;
            return RedirectToAction("Search", "App");
        }
    }
}
