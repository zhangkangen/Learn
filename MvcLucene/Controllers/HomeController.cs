using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JiebaNet.Segmenter;
using log4net;
using LuceneSearch.Data.Model;
using LuceneSearch.Data.Repository;
using LuceneSearch.Library;

namespace MvcLucene.Controllers
{
    public class HomeController : Controller
    {
        private ILog _logger = LogManager.GetLogger(typeof (HomeController));

        public ActionResult Index(string key)
        {
            _logger.InfoFormat("测试");
            if (!string.IsNullOrEmpty(key))
            {
                GoLucene.AddWords(key);
                var results = GoLucene.Search(key);
                ViewData["str"] = GoLucene.GetSegmenter().CutForSearch(key).Join("/");
                ViewData["result"] = results;
            }
            else
            {
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
                if (GoLucene.Search(data.Id.ToString(), "Id").Any())
                {
                    ViewData["msg"] = "Id已存在";
                }
                else
                {
                    IndexQueue.GetInstance().AddQueue(new Data{SampleData = data,OptionType = IndexOptionType.Add});
                }
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            if (id > 0)
            {
                var record = GoLucene.SearchDefault(id.ToString(), "Id").FirstOrDefault();
                GoLucene.ClearLuceneIndexRecord(id);
            }
            return View();
        }
    }
}
