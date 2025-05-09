using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simple.OData.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class DanhSachLichModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public DanhSachLichModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GhDatPhong DatPhong { get; set; }
        public GhDanhSachKhach DanhSachKhach {  get; set; }
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
            return Page();
        }


        //Duyệt lịch
        public async Task<IActionResult> OnPostDuyetLich(int lichId, int khachId)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            DatPhong = _dbContext.GhDatPhong.Where(d => d.Id == lichId).FirstOrDefault();
            if (DatPhong == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy thông tin lịch đặt" });
            }
            else
            {                
                var isTrungLich = await client.For<GhDatPhong>().Filter(d => d.Id != DatPhong.Id && d.PhongId == DatPhong.PhongId && (d.TinhTrangId == 1 || d.TinhTrangId == 2) && d.TuNgay <= DatPhong.DenNgay && d.DenNgay >= DatPhong.TuNgay).FindEntriesAsync();
                if (isTrungLich.Any())
                {
                    return new JsonResult(new {success = false, phanhoi = true });
                }

                DatPhong.TinhTrangId = 1; //duyệt lịch -> đã đặt
                DatPhong.NguoiDuyetId = (int)VienChucId;
                DatPhong.NgayDuyet = DateTime.Now;

                _dbContext.GhDatPhong.Update(DatPhong);

                // Cập nhật trạng thái khách
                DanhSachKhach = _dbContext.GhDanhSachKhach.Where(d => d.Id == khachId).FirstOrDefault();
                if (DanhSachKhach != null)
                {
                    DanhSachKhach.TrangThai = 1; //Lịch đã duyệt
                    _dbContext.GhDanhSachKhach.Update(DanhSachKhach);
                }
                _dbContext.SaveChanges();

                return new JsonResult(new { success = true });
            }           
        }


        //Phản hồi không duyệt
        public async Task<IActionResult> OnPostPhanHoiLich(int lichId, string ghichu, int khachId)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            DatPhong = _dbContext.GhDatPhong.Where(d => d.Id == lichId).FirstOrDefault();
            if(DatPhong == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy thông tin lịch đặt" });
            }
            else
            {
                DatPhong.TinhTrangId = 6; //Không duyệt
                DatPhong.GhiChu = ghichu;
                DatPhong.NguoiDuyetId = (int)VienChucId;
                DatPhong.NgayDuyet = DateTime.Now;

                _dbContext.GhDatPhong.Update(DatPhong);

                //cập nhật trạng thái khách
                DanhSachKhach = _dbContext.GhDanhSachKhach.Where(d => d.Id == khachId).FirstOrDefault();
                if (DanhSachKhach != null)
                {
                    DanhSachKhach.TrangThai = 2; //Không duyệt
                    _dbContext.GhDanhSachKhach.Update(DanhSachKhach);
                }
                _dbContext.SaveChanges();

                return new JsonResult(new { success = true });
            }
        }

    }
}
