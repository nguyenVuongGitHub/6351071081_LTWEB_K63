﻿using QLBanSach.Models;
using QLBanSach.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBanSach.Controllers
{
    public class GiohangController : Controller
    {
        QLBanSachEntities data = new QLBanSachEntities();
        // GET: Giohang
        public ActionResult Index()
        {
            return View();
        }
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                // Neu gio hang chua ton tai thi khoi tao listGiohang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int iMasach, string strURL)
        {
            // Lay ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            // Kiem tra sach nà y ton tai trong Session["Giohang"] chua?
            Giohang sanpham = lstGiohang.Find(n => n.iMasach == iMasach);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        // Tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        // Tính tổng tiền
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        // Xây dựng trang Giỏ hàng
        public ActionResult GioHang()
        {
            List<Giohang> lstGioHang = Laygiohang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult Giohangpartical()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            if(sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("GioHang");
            }
            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            if(sanpham != null)
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());

            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "BookStore");
        }
        // Hiển thị View Đặt Hàng để cập nhật các thông tin cho Đơn hàng
        [HttpGet]

        public ActionResult DatHang()
        {
            // Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }

            // Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            Session["Tongsoluong"] = ViewBag.Tongsoluong;
            Session["Tongtien"] = ViewBag.Tongtien;
            return View(lstGiohang);
        }
        
        public ActionResult DatHang(FormCollection collection)
        {
            // Them Don hang
            DONDATHANG ddh = new DONDATHANG();

            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();

            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;

            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);

            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;

            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();

            // Them chi tiet don hang
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();

                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.Masach = item.iMasach;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;

                data.CHITIETDONTHANGs.Add(ctdh);
                data.SaveChanges();
            }

            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }

    }
}