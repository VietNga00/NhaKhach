using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;
using static TMSWeb_Core.Pages.NhaKhach.DanhSachLichModel;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Administrators")]
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

        [BindProperty]
        public List<ListLichDuyet> ListLich { get; set; } 

        [BindProperty]
        public List<ListPhanHoi> ListPhanHoi { get; set; }
    


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

        //Duyệt nhiều lịch
        public async Task<IActionResult> OnPostDuyetNhieuLich([FromBody] List<ListLichDuyet> ListLich)
        {           
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            var lichtrung = new List<object>();

            foreach (var list in ListLich.OrderBy(x => x.lichId))
            {
                DatPhong = _dbContext.GhDatPhong.FirstOrDefault(x => x.Id == list.lichId);
                if (DatPhong == null) continue;

                //Lịch trùng
                var Istrung = await client.For<GhDatPhong>().Filter(d => d.Id != DatPhong.Id && d.PhongId == DatPhong.PhongId && (d.TinhTrangId == 4 || d.TinhTrangId == 5) && d.TuNgay <= DatPhong.DenNgay && d.DenNgay >= DatPhong.TuNgay).FindEntriesAsync();
                if (Istrung.Any())
                {
                    var khach = _dbContext.GhDanhSachKhach.FirstOrDefault(k => k.DeNghiId == DatPhong.DeNghiId);
                    lichtrung.Add(new { Id = list.lichId, KhachId = list.khachId });
                    continue;
                }

                //Duyệt lịch
                DatPhong.TinhTrangId = 4;  //Lịch đã duyệt
                DatPhong.NguoiDuyetId = (int)VienChucId;
                DatPhong.NgayDuyet = DateTime.Now;


                // Cập nhật trạng thái khách
                DanhSachKhach = _dbContext.GhDanhSachKhach.Where(d => d.Id == list.khachId).FirstOrDefault();
                if (DanhSachKhach != null)
                {
                    DanhSachKhach.TrangThai = 1; //Lịch đã duyệt
                    _dbContext.GhDanhSachKhach.Update(DanhSachKhach);
                }
                await _dbContext.SaveChangesAsync();

            }
            if (lichtrung.Any())
            {               
                return new JsonResult(new { success = false, phanhoi = true, lichTrung = lichtrung });
            }
            return new JsonResult(new { success = true });
        }

    
        //Phản hồi không duyệt
        public async Task<IActionResult> OnPostPhanHoiLich([FromBody] List<ListPhanHoi> ListPhanHoi )
        {         
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            foreach (var item in ListPhanHoi.OrderBy(x => x.lichId))
            {
                var datPhong = _dbContext.GhDatPhong.FirstOrDefault(d => d.Id == item.lichId);
                if (datPhong == null) continue;

                datPhong.TinhTrangId = 3; // Không duyệt
                datPhong.GhiChu = item.ghichu;
                datPhong.NguoiDuyetId = (int)VienChucId;
                datPhong.NgayDuyet = DateTime.Now;
                _dbContext.GhDatPhong.Update(datPhong);

                var khach = _dbContext.GhDanhSachKhach.FirstOrDefault(k => k.Id == item.khachId);
                if (khach != null)
                {
                    khach.TrangThai = 2; // Không duyệt
                    _dbContext.GhDanhSachKhach.Update(khach);
                }
            }
            await _dbContext.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

    }




    public class ListLichDuyet
    {
        public int lichId { get; set; }
        public int khachId { get; set; }
    }

    public class ListPhanHoi
    {
        public int lichId { get; set; }
        public int khachId { get; set; }
        public string ghichu { get; set; }
    }

}
