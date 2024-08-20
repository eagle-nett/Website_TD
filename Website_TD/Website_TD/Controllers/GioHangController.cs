using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Website_TD.Models;

namespace Website_TD.Controllers
{
    public class GioHangController : Controller
    {
        WebsiteEntities1 db = new WebsiteEntities1();
        // GET: GioHang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì mình tiến hành khởi tao list giỏ hàng (sessionGioHang)
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int iMasp, string strURL)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSanPham == iMasp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy ra session giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp này đã tồn tại trong session[giohang] chưa
            GioHang sanpham = lstGioHang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMasp);
                //Add sản phẩm mới thêm vào list
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Cập nhật giỏ hàng 
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            //Kiểm tra masp
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSanPham == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp có tồn tại trong session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            int Marks = int.Parse(f["quantity"]);

            if (sanpham != null)
            {
                sanpham.iSoLuong = Marks;

            }
            return RedirectToAction("GioHang");
        }
        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Kiểm tra masp
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSanPham == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }

        //Tính tổng số lượng và tổng tiền
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien += lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);

        }
        [HttpPost]
        public ActionResult DatHang(FormCollection donhangForm)
        {
            // Kiểm tra đăng nhập
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            using (var db = new WebsiteEntities1())
            {
                try
                {
                    // Khởi tạo dữ liệu cho đơn hàng
                    var taiKhoan = ((TaiKhoan)Session["User"]).MaTaiKhoan;
                    var ngayDatHang = DateTime.Now;
                    var phuongThucThanhToan = Convert.ToInt32(donhangForm["MaTT"]);

                    // Tính tổng tiền
                    decimal tongTien = 0;
                    List<GioHang> gioHang = LayGioHang();
                    foreach (var item in gioHang)
                    {
                        tongTien += item.iSoLuong * (decimal)item.dDonGia;
                    }

                    // Tạo đơn hàng mới
                    var donHang = new DonHang
                    {
                        MaTaiKhoan = taiKhoan,
                        NgayDatHang = ngayDatHang,
                        TongTien = tongTien,
                        ThanhToan = phuongThucThanhToan,
                        TinhTrang = 1 // Trạng thái "Đang chờ xử lý"
                    };
                    db.DonHangs.Add(donHang);
                    db.SaveChanges();

                    // Thêm chi tiết đơn hàng
                    foreach (var item in gioHang)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaSanPham = item.iMasp,
                            SoLuong = item.iSoLuong,
                            DonGia = (decimal)item.dDonGia,
                            ThanhTien = item.iSoLuong * (decimal)item.dDonGia
                        };
                        db.ChiTietDonHangs.Add(chiTietDonHang);
                    }
                    db.SaveChanges();

                    // Gửi email thông báo
                    SendOrderNotificationEmail(donHang);

                    // Xóa giỏ hàng
                    Session["GioHang"] = null;

                    return RedirectToAction("ThanhToanThanhCong");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    Console.WriteLine("Lỗi trong quá trình đặt hàng: " + ex.Message);
                    // Hiển thị thông báo lỗi cho người dùng
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi đặt hàng. Vui lòng thử lại.";
                    return View("ThanhToanDonHang");
                }
            }
        }




        public ActionResult ThanhToanDonHang()
        {

            ViewBag.MaTT = new SelectList(new[]
              {
            new { MaTT = 1, TenPT="Thanh toán tiền mặt" },
            new { MaTT = 2, TenPT="Thanh toán chuyển khoản" },
               }, "MaTT", "TenPT", 1);
            //ViewBag.MaNguoiDung = new SelectList(db.TaiKhoan, "MaTaiKhoan", "HoTen");

            //Kiểm tra đăng đăng nhập
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Account");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            //Thêm đơn hàng
            DonHang ddh = new DonHang();
            TaiKhoan kh = (TaiKhoan)Session["User"];
            List<GioHang> gh = LayGioHang();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }

            ddh.MaTaiKhoan = kh.MaTaiKhoan;
            ddh.NgayDatHang = DateTime.Now;
            ChiTietDonHang ctDH = new ChiTietDonHang();
            ViewBag.tongtien = tongtien;
            return View(ddh);

        }
        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }
        public void SendOrderNotificationEmail(DonHang donHang)
        {
            var toAddress = "tiendat176mt@gmail.com"; // Địa chỉ email nhận thông báo
            var fromAddress = "tiendat176mt@gmail.com"; // Địa chỉ email gửi thông báo
            const string subject = "Thông báo đặt hàng mới";

            var body = $@"
                        <h2>Thông tin đơn hàng</h2>
                        <p>Mã đơn hàng: {donHang.MaDonHang}</p>
                        <p>Ngày đặt hàng: {donHang.NgayDatHang}</p>
                        <p>Tổng tiền: {donHang.TongTien:C}</p>
                        <p>Phương thức thanh toán: {(donHang.ThanhToan == 1 ? "Tiền mặt" : "Chuyển khoản")}</p>";

            var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("tiendat176mt@gmail.com", "flam gjfa poxj icly"), // Sử dụng mật khẩu ứng dụng
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromAddress, toAddress, subject, body)
            {
                IsBodyHtml = true
            };

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email đã được gửi thành công.");
            }
            catch (Exception ex)
            {
                // Ghi lại chi tiết lỗi
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
                // Hoặc sử dụng hệ thống logging của bạn để ghi lỗi
            }
        }
    }
}


        
