using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;


namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class TaoDatPhongModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public TaoDatPhongModel(TMS_CoreContext dbContext)
        {

           _dbContext = dbContext;
        }


        [BindProperty]
        public GhDatPhong DatPhong { get; set; }
        [BindProperty]
        public GhDeNghi DeNghi { get; set; }
        public GhPhong Phong { get; set; }
        public GhLoaiPhong Loai { get; set; }

        public int? VienChucId = 0;
        public int? TaiKhoanId = 0;
        public List<GhDanhSachKhach> DanhSachKhach { get; set; } = new();       

        [BindProperty(SupportsGet = true, Name = "khach")]
        public int khachID { get; set; }

        [BindProperty(SupportsGet = true, Name = "denghi")]
        public int denghiID { get; set; }
        public int donviID { get; set; }

        [BindProperty(Name = "phong", SupportsGet = true)]
        public int phongID { get; set; }
        public int loaiID { get; set; }
        


        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;                     

            if ( phongID > 0)
            {
                loaiID = _dbContext.GhPhong.Where(p => p.Id == phongID).Select(p => p.LoaiId).FirstOrDefault();
            }
          
            if(denghiID > 0)
            {
                donviID = (int)_dbContext.GhDeNghi.Where(p => p.Id == denghiID).Select(p => p.DonViId).FirstOrDefault();

                var colich = _dbContext.GhDatPhong.Where(p => p.DeNghiId == denghiID).Select(p => p.KhachId).Distinct().ToList();
                DanhSachKhach = _dbContext.GhDanhSachKhach.Where(p => p.DeNghiId == denghiID && !colich.Contains(p.Id)).ToList();
            }

            return Page();
        }

        //Load phòng
        public IActionResult OnGetLoadPhongByLoai(int loaiId)
        {
            var loadPhong = _dbContext.GhPhong.Where(s => s.LoaiId == loaiId).ToList();
            var soLuongKhach = _dbContext.GhLoaiPhong.Where(lp => lp.Id == loaiId).Select(lp => lp.SoLuongToiDa).FirstOrDefault();

            return new JsonResult(new { loadPhong = loadPhong, soluong = soLuongKhach });
        }

        //Load loại phòng
        public IActionResult OnGetLoadLoaiPhong(int phongId)
        {
            var LoadLoai = _dbContext.GhPhong.Where(s => s.Id == phongId)
                .Select(s => new {
                    id = s.LoaiId,
                    ten = s.Loai.TenLoai,                
                    soluong = s.Loai.SoLuongToiDa,
                }).FirstOrDefault();
            return new JsonResult(LoadLoai);
        }


        //Load đơn vị - khách
        public IActionResult OnGetLoadKhach(int denghiId)
        {
            var donvi = _dbContext.GhDeNghi.Where(d => d.Id == denghiId).Select(d => d.DonViId).FirstOrDefault();
            var khachDaCoLich = _dbContext.GhDatPhong.Where(dp => dp.DeNghiId == denghiId).Select(dp => dp.KhachId).Distinct().ToList();

            var loadkhach = _dbContext.GhDanhSachKhach.Where(k => k.DeNghiId == denghiId && !khachDaCoLich.Contains(k.Id))
                .Select(k => new
                {
                    Id = k.Id,
                    TenKhach = k.TenKhach
                }).ToList();           

            return new JsonResult(new { donvi = donvi, loadkhach = loadkhach });
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;
           
            //Kiểm tra trùng lịch
            var isBooking = await _dbContext.GhDatPhong.Where(d => d.PhongId == DatPhong.PhongId
                            &&  (d.TinhTrangId == 1 || d.TinhTrangId == 2 || d.TinhTrangId == 3 ) //Đã đặt - Đã nhận phòng - Hoàn Thành
                            && ((DatPhong.TuNgay >= d.TuNgay && DatPhong.TuNgay < d.DenNgay) || (DatPhong.DenNgay > d.TuNgay && DatPhong.DenNgay <= d.DenNgay) || (DatPhong.TuNgay <= d.TuNgay && DatPhong.DenNgay >= d.DenNgay) )
                            ).FirstOrDefaultAsync();
          
            if (isBooking != null)
            {
                return new JsonResult(new { success = false, message = "Phòng đã có lịch đặt bị trùng trong khoảng thời gian chọn!. Vui lòng đặt lịch lại." });
            }           
            //Lưu thông tin đặt phòng
            DatPhong.DeNghiId = Convert.ToInt32(Request.Form["DeNghiId"]);
            DatPhong.KhachId = Convert.ToInt32(Request.Form["KhachId"]);
            DatPhong.TuNgay = Convert.ToDateTime(Request.Form["TuNgay"]);
            DatPhong.DenNgay = Convert.ToDateTime(Request.Form["DenNgay"]);
            DatPhong.AllDay = Convert.ToBoolean(Request.Form["AllDay"]);                       
            DatPhong.PhongId = Convert.ToInt32(Request.Form["PhongId"]);
            DatPhong.NguoiDatId = Convert.ToInt32(Request.Form["NguoiDatId"]);
            DatPhong.GhiChu = Request.Form["GhiChu"].ToString();
            DatPhong.TinhTrangId = 5; //Chờ duyệt lịch
            DatPhong.TrangThai = false;
            DatPhong.NgayTao = DateTime.Now;
            DatPhong.NguoiThucHienId = (int)VienChucId;

            _dbContext.GhDatPhong.Add(DatPhong);

            //Cập nhật trạng thái khách
            DanhSachKhach = _dbContext.GhDanhSachKhach.Where(d => d.Id == DatPhong.KhachId).ToList();
            if (DanhSachKhach.Any())
            {
                DanhSachKhach[0].TrangThai = 0;  //Chờ duyệt lịch
                _dbContext.GhDanhSachKhach.Update(DanhSachKhach[0]);
            }
            //Cập nhật tình trạng đề nghị
            DeNghi = _dbContext.GhDeNghi.Where(d => d.Id == DatPhong.DeNghiId).FirstOrDefault();
            if (DeNghi != null)
            {
                DeNghi.TinhTrangId = 5;  //Chờ duyệt lịch
                _dbContext.GhDeNghi.Update(DeNghi);
            }
            await _dbContext.SaveChangesAsync();

            var lichid = common.EncryptString("b14ca54545451215451FSDFSEVDa1916", DatPhong.Id.ToString());

            return new JsonResult(new { success = true, id = lichid });
        }


    }
}

