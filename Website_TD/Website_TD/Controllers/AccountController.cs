using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Website_TD.Models;

namespace Website_TD.Controllers
{
    public class AccountController : Controller
    {
        WebsiteEntities1 db = new WebsiteEntities1();
        // GET: Account
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(TaiKhoan tk)
        {

            if (ModelState.IsValid)
            {
                tk.MatKhau = HashPassword(tk.MatKhau);
                db.TaiKhoans.Add(tk);
                db.SaveChanges();
                return RedirectToAction("DangNhap");
            }
            else
            {
                ViewBag.DangKyThatBai = "Đăng ký thất bại";
                return View("DangKy");
            }
        }
        private string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển đổi byte[] sang chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(TaiKhoan tk)
        {
            var email = tk.Email;
            var matkhau = tk.MatKhau;
            var mahoa = "";
            if (matkhau != null && email != null)
            {
                mahoa = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(matkhau))).Replace("-", "");
            }

            var admin = db.TaiKhoans.SingleOrDefault(row => row.Email.Equals(email) && row.MatKhau.Equals(mahoa) && row.MaQuyen == 1);
            var user = db.TaiKhoans.SingleOrDefault(row => row.Email.Equals(email) && row.MatKhau.Equals(mahoa) && row.MaQuyen == 2);
            if (admin != null)
            {
                Session["Admin"] = admin;
                return RedirectToAction("Index", "Home");
            }
            else if (user != null)
            {
                Session["User"] = user;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.DangNhapSai = "Đăng nhập thất bại";
                return View("DangNhap");
            }

        }
        public ActionResult DangXuat()
        {
            Session["Admin"] = null;
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ThongTinNguoiDung(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }
        public ActionResult PhanHoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PhanHoi(PhanHoi ph)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin tài khoản từ Session
                var currentUser = Session["User"] as TaiKhoan;
                if (currentUser != null)
                {
                    // Gán ID tài khoản vào phản hồi
                    ph.MaTaiKhoan = currentUser.MaTaiKhoan;

                    // Thêm phản hồi vào cơ sở dữ liệu
                    db.PhanHois.Add(ph);
                    db.SaveChanges();

                    // Chuyển hướng đến hành động khác sau khi lưu thành công
                    return RedirectToAction("PhanHoi");
                }
                else
                {
                    // Nếu không tìm thấy thông tin tài khoản trong session
                    ViewBag.PhanHoiThatBai = "Bạn cần đăng nhập để gửi phản hồi.";
                    return View(ph); // Trả lại view với thông tin phản hồi hiện tại
                }
            }
            else
            {
                // Nếu ModelState không hợp lệ, chuyển hướng đến trang Index
                ViewBag.PhanHoiThatBai = "Dữ liệu phản hồi không hợp lệ.";
                return View(ph); // Trả lại view với thông tin phản hồi hiện tại
            }
        }

    }
}