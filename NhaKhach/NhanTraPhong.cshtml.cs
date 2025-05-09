using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;
using System.Linq;
using Simple.OData.Client;
using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class NhanTraPhongModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public NhanTraPhongModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

       
        public GhDatPhong DatPhong { get; set; }
        [BindProperty]
        public GhDanhSachKhach DanhSachKhach { get; set; }

        [BindProperty]
        public GhNhanPhong NhanPhong { get; set; }
      
        public GhTraPhong TraPhong {  get; set; }

        public int? VienChucId = 0;

        public List<dynamic> Data { get; set; }




        public async Task<IActionResult> OnGetAsync()
        {
            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            if (!(await common.checkRoleAsync(email, url, "Create", HttpContext.GetTokenAsync("access_token").Result)))
            {
                return new RedirectToPageResult("NhaKhach/AccessDeny");
            }
            //Hiển thị danh sách trả phòng
            Data = _dbContext.GhDatPhong.Join(_dbContext.GhNhanPhong, dp => dp.Id, np => np.DatPhongId,
                (dp, np) => new
                {
                    dp.Id,
                    dp.PhongId,
                    dp.NguoiDatId,                  
                    dp.TinhTrangId,
                    dp.TuNgay,
                    dp.DenNgay,
                    dp.AllDay,
                    dp.TrangThai,   
                    dp.DeNghiId,
                    dp.KhachId,
                    np.NgayNhan,
                }
            ).ToList<dynamic>();

            return Page();
        }

        //Nhận phòng
        public async Task<IActionResult> OnPostNhanLich(int lichId)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;
          
            if (lichId <= 0)
            {
                return new JsonResult(new { success = false, message = "Mã lịch đặt không hợp lệ" });
            }

            DatPhong = _dbContext.GhDatPhong.FirstOrDefault(d => d.Id == lichId);        
            if (DatPhong != null)
            {
                DatPhong.TinhTrangId = 2; // Đã nhận phòng
                _dbContext.GhDatPhong.Update(DatPhong);
            }

            NhanPhong = new GhNhanPhong
            {
                DatPhongId = lichId,
                NgayNhan = DateTime.Now,
                NguoiThucHienId = (int)VienChucId
            };

            _dbContext.GhNhanPhong.Add(NhanPhong);
            _dbContext.SaveChanges();

            return new JsonResult(new { success = true });
        }

        //Load edit lịch
        public JsonResult OnGetShowLich(int idlich)
        {
            var datalich = (from dp in _dbContext.GhDatPhong
                            join p in _dbContext.GhPhong on dp.PhongId equals p.Id                           
                            join nt in _dbContext.VienChuc on dp.NguoiDatId equals nt.Id
                            join kh in _dbContext.GhDanhSachKhach on dp.KhachId equals kh.Id                        
                            where dp.Id == idlich
                            select new
                            {
                                dp.Id,
                                dp.TuNgay,
                                dp.DenNgay,
                                dp.AllDay,                               
                                TenKhach = kh.TenKhach,
                                MaPhong = p.MaPhong,                             
                                TenNguoiDat = nt.FullName,                              

                            }).FirstOrDefault();

            return new JsonResult(datalich);
        }

        /* Cập nhật thời gian lịch */
        public async Task<IActionResult> OnPostUpdateLich(int idlich, DateTime tungay, DateTime denngay, bool cangay)
        {          
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            DatPhong = _dbContext.GhDatPhong.FirstOrDefault(d => d.Id == idlich);
            if (DatPhong == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy lịch đặt!" });
            }           
            //Kiểm tra có lịch nào trùng không
            bool kiemtra = _dbContext.GhDatPhong.Any(d => d.PhongId == DatPhong.PhongId && d.Id != DatPhong.Id && (d.TinhTrangId == 1 || d.TinhTrangId == 2 ) && !(denngay <= d.TuNgay || tungay >= d.DenNgay) );           
            //((tungay >= d.TuNgay && tungay < d.DenNgay) || (denngay >= d.TuNgay && denngay < d.DenNgay) || (tungay <= d.TuNgay && denngay >= d.DenNgay))

            if (kiemtra)
            {
                return new JsonResult(new { success = false, message = "Thời gian mới bị trùng với lịch đặt khác!" });
            }
            DatPhong.TuNgay = tungay;
            DatPhong.DenNgay = denngay;
            DatPhong.AllDay = cangay;

            _dbContext.GhDatPhong.Update(DatPhong);
            await _dbContext.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }


        /* Trả phòng */      
        public async Task<IActionResult> OnPostTraPhong(int lichId)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;
          
            if (lichId <= 0)
            {
                return new JsonResult(new { success = false, message = "Mã lịch đặt không hợp lệ" });
            }

            DatPhong = _dbContext.GhDatPhong.FirstOrDefault(d => d.Id == lichId);           
            if (DatPhong != null)
            {
                DatPhong.TinhTrangId = 3; // Trả phòng -> Hoàn thành
                _dbContext.GhDatPhong.Update(DatPhong);              
            }
            var tongthoigian = DatPhong.DenNgay - DatPhong.TuNgay;
           
            TraPhong = new GhTraPhong
            {
                DatPhongId = lichId,
                TongThoiGian = $"{tongthoigian.Hours + tongthoigian.Days * 24} giờ {tongthoigian.Minutes} phút",
                NgayTra = DateTime.Now,
                NguoiThucHienId = (int)VienChucId
            };
            _dbContext.GhTraPhong.Add(TraPhong);
            _dbContext.SaveChanges();

            return new JsonResult(new { success = true });
        }



    }
}
