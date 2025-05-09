using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class DanhSachDeNghiModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public DanhSachDeNghiModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GhDatPhong DatPhong { get; set; }
        public GhDeNghi DeNghi { get; set; }

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

        //Khách trong đề nghị có đủ lịch chưa
        public IActionResult OnGetCheckStatus()
        {
            var dsdenghi = _dbContext.GhDeNghi.ToList();

            foreach(var dn in dsdenghi)
            {
                var dskhach = _dbContext.GhDanhSachKhach.Where(k => k.DeNghiId == dn.Id).ToList();
                if (dskhach.Count() == 0) continue;
                bool isStatus = dskhach.All(k => k.TrangThai == 1 || k.TrangThai == 2); //Khách có trạng thái Lịch đã duyệt or Không duyệt lịch

                var solich = _dbContext.GhDatPhong.Where(l => l.DeNghiId == dn.Id && l.KhachId != null).Select(l => l.KhachId.Value).Distinct().Count();
                bool ischeck = dskhach.Count() == solich;
                if(isStatus && ischeck)
                {
                    dn.TinhTrangId = 8; //Hoàn tất
                }               
            }
             _dbContext.SaveChanges();

            return new JsonResult(new { success = false });
        }

        //Hủy đề nghị
        public async Task<IActionResult> OnPostHuyDeNghi(int id)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            DeNghi = await client.For<GhDeNghi>().Filter(f => f.Id == id).FindEntryAsync();
            if (DeNghi != null)
            {
                DeNghi.TinhTrangId = 4; //Đã hủy             
                //DeNghi.NgayHuy = DateTime.Now;
                //DeNghi.NguoiHuyId = (int)VienChucId;

                _dbContext.GhDeNghi.Update(DeNghi);
                _dbContext.SaveChanges();
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false, message = "Không tìm thấy đề nghị" });
        }



    }
}
