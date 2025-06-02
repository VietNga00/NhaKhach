using Abp.Extensions;
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

        public bool IsEdit => DatPhong != null && DatPhong.Id > 0;
        public bool isAdmin { get; set; }




        public async Task<IActionResult> OnGetAsync(int? datphongID)
        {          
            var accessToken = common.RefreshAccessToken(HttpContext);
             var email = User.FindFirst("Email").Value;
          
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            // Kiểm tra quyền
            List<string> dsAdmin = new List<string> { "nvvnga1103@gmail.com" }; 
            this.isAdmin = dsAdmin.Contains(email.ToLower());
            

            // Nếu là chỉnh sửa
            if (datphongID.HasValue)
            {              
                DatPhong = await client.For<GhDatPhong>().Key(datphongID.Value).FindEntryAsync();
                if (DatPhong == null)
                {
                    return NotFound();
                }

                bool isNguoiTao = (DatPhong.NguoiDatId == taikhoan.VienChucId);
                if (!isAdmin && !isNguoiTao)
                {
                    return RedirectToPage("NhaKhach/AccessDeny");
                }

                this.denghiID = DatPhong.DeNghiId ?? 0;
                this.phongID = DatPhong.PhongId ?? 0;
                this.khachID = DatPhong.KhachId ?? 0;                
                this.loaiID = _dbContext.GhPhong.Where(p => p.Id == phongID).Select(p => p.LoaiId).FirstOrDefault();
                this.donviID = _dbContext.GhDeNghi.Where(p => p.Id == denghiID).Select(p => p.DonViId ?? 0).FirstOrDefault();
               
                DanhSachKhach = _dbContext.GhDanhSachKhach.Where(p => p.DeNghiId == denghiID && (p.TrangThai == null || p.Id == DatPhong.KhachId) ).ToList();               
            }
            else
            {
                // Nếu là tạo mới
                DatPhong = new GhDatPhong
                {
                    NguoiDatId = taikhoan.VienChucId,
                    AllDay = false
                };
            
                if (phongID > 0)
                {
                    this.loaiID = _dbContext.GhPhong.Where(p => p.Id == phongID).Select(p => p.LoaiId).FirstOrDefault();
                }

                if (denghiID > 0)
                {                    
                    this.donviID = _dbContext.GhDeNghi.Where(p => p.Id == denghiID).Select(p => p.DonViId ?? 0).FirstOrDefault();

                    var colich = _dbContext.GhDatPhong.Where(p => p.DeNghiId == denghiID).Select(p => p.KhachId).Distinct().ToList();
                    DanhSachKhach = _dbContext.GhDanhSachKhach.Where(p => p.DeNghiId == denghiID && !colich.Contains(p.Id)).ToList();
                    
                }
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


        //Tạo mới đặt phòng
        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;
           
            //Kiểm tra trùng lịch
            var isBooking = await _dbContext.GhDatPhong.Where(d => d.PhongId == DatPhong.PhongId
                            &&  (d.TinhTrangId == 4 || d.TinhTrangId == 5 || d.TinhTrangId == 6 ) //Đã đặt - Đã nhận phòng - Hoàn Thành
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
            DatPhong.GhiChu = Request.Form["GhiChu"].ToString();
            DatPhong.TinhTrangId = 2; //Chờ duyệt lịch
            DatPhong.TrangThai = false;
            DatPhong.NguoiDatId = Convert.ToInt32(Request.Form["NguoiDatId"]);
            DatPhong.NgayTao = DateTime.Now;            

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
                DeNghi.TinhTrangId = 2;  //Chờ duyệt lịch
                _dbContext.GhDeNghi.Update(DeNghi);
            }
            await _dbContext.SaveChangesAsync();

            var lichid = common.EncryptString("b14ca54545451215451FSDFSEVDa1916", DatPhong.Id.ToString());

            return new JsonResult(new { success = true, id = lichid });
        }


        //Chỉnh sửa đặt phòng
        public async Task<IActionResult> OnPostEditLichAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            var form = await Request.ReadFormAsync();
            var id = Convert.ToInt32(form["DatPhongId"]);

            DatPhong = await _dbContext.GhDatPhong.FindAsync(id);
            if (DatPhong == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy lịch đặt" });
            }

            //Kiểm tra trùng lịch
            bool kiemtra = _dbContext.GhDatPhong.Any(d => d.PhongId == DatPhong.PhongId && d.Id != DatPhong.Id && (d.TinhTrangId == 4 || d.TinhTrangId == 5) && !(DatPhong.DenNgay <= d.TuNgay || DatPhong.TuNgay >= d.DenNgay));           
            if (kiemtra)
            {
                return new JsonResult(new { success = false, message = "Thời gian mới bị trùng với lịch đặt khác!" });
            }

            //Lưu thông tin đặt phòng                   
            DatPhong.TuNgay = Convert.ToDateTime(Request.Form["TuNgay"]);
            DatPhong.DenNgay = Convert.ToDateTime(Request.Form["DenNgay"]);
            DatPhong.AllDay = Convert.ToBoolean(Request.Form["AllDay"]);
            DatPhong.PhongId = Convert.ToInt32(Request.Form["PhongId"]);         
            DatPhong.GhiChu = Request.Form["GhiChu"].ToString();                
            DatPhong.NguoiThucHienId = (int)VienChucId;
            DatPhong.NgayCapNhat = DateTime.Now;

            _dbContext.GhDatPhong.Update(DatPhong);
            await _dbContext.SaveChangesAsync();
           
            return new JsonResult(new { success = true });
        }


    }
}

