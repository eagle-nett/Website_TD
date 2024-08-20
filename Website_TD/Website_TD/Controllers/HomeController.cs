using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_TD.Models;


namespace Website_TD.Controllers
{
    public class HomeController : Controller
    {
        WebsiteEntities1 db = new WebsiteEntities1();
        // GET: Home
        public ActionResult Index(string search = "")
        {
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult Gioithieu()
        {
            return View();
        }

    }
}