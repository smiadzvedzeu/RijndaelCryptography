using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RijndaelCryptography.Framework.Source;

namespace RijndaelCryptography.Controllers
{
    public class HomeController : Controller
    {
        #region Auto-generated
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        #endregion

        public ActionResult EncodeLink(string email, string link)
        {
            var encriptor = new StringCipherCrypt(ConfigurationManager.AppSettings["RijndaelCryptInitVector"],
                int.Parse(ConfigurationManager.AppSettings["RijndaelCryptKeySize"]));

            var linkForEncrypt = encriptor.Encrypt(link, email.ToLower());
            if (linkForEncrypt != null)
            {
                return Json(linkForEncrypt, JsonRequestBehavior.AllowGet);
            }
            //cannot parse link
            return Json(new { error = "Error" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Decrypt Hash from hash, use email as passPhrase
        /// </summary>
        /// <param name="email"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public ActionResult DecryptHash(string email, string hash)
        {
            var encriptor = new StringCipherCrypt(ConfigurationManager.AppSettings["RijndaelCryptInitVector"],
                int.Parse(ConfigurationManager.AppSettings["RijndaelCryptKeySize"]));

            var linkForEncrypt = encriptor.Decrypt(hash, email.ToLower());
            if (linkForEncrypt != null)
            {
                var parsedLink = HttpUtility.ParseQueryString(linkForEncrypt);
                var userName = parsedLink.Get("userName");
                var userPassword = parsedLink.Get("userPassword");

                return Json(new { data = "?userName=" + userName + "&userPassword=" + userPassword }, JsonRequestBehavior.AllowGet);
                }
            //cannot parse link
            return Json(new { error = "Error" }, JsonRequestBehavior.AllowGet);
        }
    }
}