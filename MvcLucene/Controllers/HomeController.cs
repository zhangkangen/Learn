using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LuceneSearch.Library;

namespace MvcLucene.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private readonly JiebaTest _jiebaTest = new JiebaTest();

        public ActionResult Index(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                ViewData["str"] = _jiebaTest.JiebaSegment(key);
            }
            else
            {
                ViewData["str"] = "请输入";
            }
            return View();
        }
    }
}
