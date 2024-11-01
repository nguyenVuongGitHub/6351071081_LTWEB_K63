using QLBanSach.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBanSach.Controllers
{
    public class BookStoreController : Controller
    {
        // GET: BookStore
        QLBanSachEntities data = new QLBanSachEntities();
        
        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        
        public ActionResult Index()
        {
            var sachMoi = LaySachMoi(5);
            return View(sachMoi);
        }
        public ActionResult ChuDe()
        {
            var chude = from cd in data.CHUDEs select cd;
            return PartialView(chude);
        }
        public ActionResult NhaXuatBan()
        {
            var nxb = from cd in data.NHAXUATBANs select cd;
            return PartialView(nxb);
        }
        public ActionResult SPTheochude(int MaCD)
        {
            var sach = from s in data.SACHes where s.CHUDE.MaCD == MaCD select s;
            return View(sach);
        }
        public ActionResult SPTheoNXB(int MaNXB)
        {
            var sach = from s in data.SACHes where s.MaNXB == MaNXB select s;
            return View(sach);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes where s.Masach == id select s;

            return View(sach.Single());
        }
        public ActionResult SignUp()
        {
            return View();
        }
    }
}