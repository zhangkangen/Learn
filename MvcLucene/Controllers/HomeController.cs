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

        public ActionResult Index()
        {
            ViewData["str"] = _jiebaTest.JiebaSegment();
            return View();
        }

    }
}
