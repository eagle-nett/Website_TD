using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_TD.Models;
namespace Website_TD.Controllers
{
    public class DanhSachSpController : Controller
    {
        // GET: DanhSachSP
        public ActionResult Ao(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult Quan(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult Giay(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult PhuKien(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult DongHo(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult Best(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult Limited(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult Luxury(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
    }
}