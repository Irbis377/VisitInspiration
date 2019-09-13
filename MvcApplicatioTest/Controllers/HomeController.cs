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

        public ActionResult Index()
        {
            int BlockSize = 5;
            DataManager dm = new DataManager();
            var photos = DataManager.GetPhotos(BlockSize);
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
            ////////////////// THis line of code only for demo. Needs to be removed ///////////////
            //System.Threading.Thread.Sleep(3000);
            //////////////////////////////////////////////////////////////////////////////////////////

            int BlockSize = 5;
            var photos = DataManager.GetPhotos(BlockSize);

            string jphotos = RenderPartialViewToString("ListPhotos", photos);

            return Json(jphotos);
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
