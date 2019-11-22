using MvcApplicatioTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplicatioTest.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private const int BlockSize = 5;

        public ActionResult Index()
        {
            DataManagerFlickr dm = new DataManagerFlickr();
            var photos = DataManagerFlickr.GetPhotos(BlockSize);
            return View(photos);
        }

        [ChildActionOnly]
        public ActionResult ListPhotos(List<PhotoFlickr> Model)
        {
            return PartialView(Model);
        }

        [HttpPost]
        public ActionResult InfinateScroll(int BlockNumber)
        {
            var photos = DataManagerFlickr.GetPhotos(BlockSize);
            JsonModel jsonModel = new JsonModel();
            jsonModel.NoMoreData = photos.Count < BlockSize;
            jsonModel.HTMLString = RenderPartialViewToString("ListPhotos", photos);
            return Json(jsonModel);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
