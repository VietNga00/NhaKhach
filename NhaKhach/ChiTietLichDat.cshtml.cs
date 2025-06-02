using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;

namespace TMSWeb_Core.Pages.NhaKhach
{
    
    [IgnoreAntiforgeryToken]
    public class ChiTietLichDatModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public ChiTietLichDatModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty(Name = "id", SupportsGet = true)]
        public string lichId { get; set; }
        public GhDatPhong DatPhong { get; set; }
        public GhPhong Phong { get; set; }
        public GhLoaiPhong Loai { get; set; }
        public GhDeNghi DeNghi { get; set; }
        public GhTinhTrang TinhTrang { get; set; }
        public List<GhDanhSachKhach> Khach { get; set; }
        public VienChuc NguoiDat { get; set; }
        public VienChuc NguoiDuyet { get; set; }
        public DonVi DonVi { get; set; }

        public int? VienChucId = 0;



        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
          
            if (!(await common.checkRoleAsync(email, url, "Create", HttpContext.GetTokenAsync("access_token").Result)))
            {
                return new RedirectToPageResult("NhaKhach/AccessDeny");
            }

            int ID = 0;
            if (lichId != null)
            {                
                var decode = common.DecryptString("b14ca54545451215451FSDFSEVDa1916", lichId);
                lichId = decode;
                ID = Convert.ToInt32(lichId);        

                DatPhong = _dbContext.GhDatPhong.FirstOrDefault(d => d.Id == ID);

                if (DatPhong != null)
                {
                    Phong = _dbContext.GhPhong.FirstOrDefault(d => d.Id == DatPhong.PhongId);
                    Loai = _dbContext.GhLoaiPhong.FirstOrDefault(d => d.Id == Phong.LoaiId);                                 
                    TinhTrang = _dbContext.GhTinhTrang.FirstOrDefault(d => d.Id == DatPhong.TinhTrangId);
                    NguoiDat = _dbContext.VienChuc.FirstOrDefault(d => d.Id == DatPhong.NguoiDatId);
                    NguoiDuyet = _dbContext.VienChuc.FirstOrDefault(d => d.Id == DatPhong.NguoiDuyetId);
                    DeNghi = _dbContext.GhDeNghi.FirstOrDefault(d => d.Id == DatPhong.DeNghiId);
                    DonVi = _dbContext.DonVi.FirstOrDefault(d => d.Id == DeNghi.DonViId);
                    Khach = _dbContext.GhDanhSachKhach.Where(k => k.Id == DatPhong.KhachId).ToList();
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy thông tin lịch đặt" });
                }
            }
            else
            {
                return new JsonResult(new { success = false, message = "Mã lịch đặt không hợp lệ" });
            }

            return Page();
        }


        public async Task<IActionResult> OnPostHuyLich(int Id)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            DatPhong = _dbContext.GhDatPhong.Where(d => d.Id == Id).FirstOrDefault();
            if (DatPhong == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy thông tin lịch đặt" });
            }
            else
            {
                DatPhong.TinhTrangId = 7; // Đã hủy
                DatPhong.NgayHuy = DateTime.Now;
                DatPhong.NguoiHuyId = (int)VienChucId;

                _dbContext.GhDatPhong.Update(DatPhong);
                _dbContext.SaveChanges();
            }

            return new JsonResult(new { success = true });
        }


    }
}
