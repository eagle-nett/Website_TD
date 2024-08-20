using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_TD.Models;
using System.Data.Entity;

namespace Website_TD.Controllers
{
    public class AdminController : Controller
    {
        WebsiteEntities1 db = new WebsiteEntities1();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(SanPham sp)
        {
            if (ModelState.IsValid)
            {
                db.SanPhams.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Index");
            }
        }

        public ActionResult XoaSp(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }

        public ActionResult Delete(int? id)
        {
            SanPham sp = db.SanPhams.Where(row => row.MaSanPham == id).FirstOrDefault();
            db.SanPhams.Remove(sp);
            db.SaveChanges();
            return View("XoaThanhCong");
        }
        public ActionResult SuaSp(string search = "")
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            List<SanPham> sp = db.SanPhams.Where(row => row.TenSanPham.Contains(search)).ToList();
            return View(sp);
        }
        public ActionResult IdSua(int? id)
        {
            SanPham sp = db.SanPhams.Where(row => row.MaSanPham == id).FirstOrDefault();

            return View(sp);
        }
        [HttpPost]
        public ActionResult IdSua(SanPham sua)
        {
            // Tìm sản phẩm theo MaSanPham
            SanPham sp = db.SanPhams.Where(row => row.MaSanPham == sua.MaSanPham).FirstOrDefault();

            // Cập nhật thông tin sản phẩm
            sp.TenSanPham = sua.TenSanPham;
            sp.Gia = sua.Gia;
            sp.MaLoaiSP = sua.MaLoaiSP;
            sp.MoTa = sua.MoTa;
            sp.HinhAnh = sua.HinhAnh;
            sp.MaNhaCungCap = sua.MaNhaCungCap;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Chuyển hướng về trang chủ hoặc danh sách sản phẩm
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Chitiet(int? id)
        {
            WebsiteEntities1 db = new WebsiteEntities1();
            SanPham sp = db.SanPhams.Where(row => row.MaSanPham == id).FirstOrDefault();
            return View(sp);
        }
        public ActionResult XemDonHang()
        {
            // Lấy danh sách tất cả các đơn hàng
            var donHangList = db.DonHangs
                .Include("ChiTietDonHangs.SanPham") // Bao gồm thông tin chi tiết đơn hàng và sản phẩm
                .ToList();

            return View(donHangList);
        }
        [HttpPost]
        public ActionResult CapNhatTinhTrang(int id)
        {
            // Tìm đơn hàng theo id
            var donHang = db.DonHangs.Find(id);
            if (donHang != null)
            {
                // Cập nhật tình trạng đơn hàng
                donHang.TinhTrang = 0;
                db.SaveChanges();
            }

            // Trở lại view XemDonHang sau khi cập nhật
            return RedirectToAction("XemDonHang");
        }

        // Phương thức để hiển thị chi tiết đơn hàng
        public ActionResult ChiTietDonHang(int id)
        {
            // Lấy đơn hàng cùng với chi tiết đơn hàng và sản phẩm liên quan
            var donHang = db.DonHangs
                .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.SanPham)) // Bao gồm chi tiết đơn hàng và sản phẩm
                .SingleOrDefault(dh => dh.MaDonHang == id);

            if (donHang == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy đơn hàng
            }

            return View(donHang);
        }
        public ActionResult XemPhanHoi()
        {
            // Lấy danh sách phản hồi từ cơ sở dữ liệu
            var phanHoiList = db.PhanHois.ToList();

            // Chuyển danh sách phản hồi đến view
            return View(phanHoiList);
        }
    }
}