using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard.Controllers
{
    [Authorize]
    public class ListController : Controller
    {
        // GET: List
        public ActionResult Index()
        {
            return View();
        }
    }
}