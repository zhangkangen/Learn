using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JiebaNet.Segmenter;
using LuceneSearch.Data.Model;
using LuceneSearch.Data.Repository;
using LuceneSearch.Library;

namespace MvcLucene.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                GoLucene.AddWords(key);
                var results = GoLucene.Search(key);
                ViewData["str"] = GoLucene.GetSegmenter().CutForSearch(key).Join("/");
                ViewData["result"] = results;
            }
            else
            {
                GoLucene.UpdateLuceneIndex(SampleDataRepository.GetAll());
                ViewData["str"] = "请输入";
            }

            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(SampleData data)
        {

            if (data.Id == 0)
            {
                ViewData["msg"] = "Id不能为空";
            }
            else if (string.IsNullOrEmpty(data.Name))
            {
                ViewData["msg"] = "Name不能为空";
            }
            else if (string.IsNullOrEmpty(data.Description))
            {
                ViewData["msg"] = "Description不能为空";
            }
            else
            {
                var b = GoLucene.Search(data.Id.ToString(), "Id");
                if (b.Count() > 0)
                {
                    ViewData["msg"] = "Id已存在";
                }
                else
                {
                    GoLucene.UpdateLuceneIndex(data);
                }
            }
            return View();
        }
    }
}
