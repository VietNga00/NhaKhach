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
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class LichDatPhongModel : PageModel
    {
        [BindProperty]
        public GhDatPhong DatPhong { get; set; }
        public int? VienChucId = 0;


        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            return Page();
        }

        //Chi tiết lịch đặt
        public async Task<IActionResult> OnPostQuyenChuyenTrang(int lichId)
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            //Người quản trị
            var dsAdmin = new List<string>{ "nvvnga1103@gmail.com", };  // nhukhanhtv052@gmail.com / nvvnga1103@gmail.com

            DatPhong = await client.For<GhDatPhong>().Filter(f => f.Id == lichId).FindEntryAsync();
            if (DatPhong != null)
            {
                bool isAdmin = dsAdmin.Contains(email);
                bool isNguoiDat = DatPhong.NguoiDatId == taikhoan.VienChucId;
                if(isAdmin || isNguoiDat)
                {
                    var ldID = common.EncryptString("b14ca54545451215451FSDFSEVDa1916", lichId.ToString());
                    return new JsonResult(new { success = true, ldID });
                }
            }
            return new JsonResult(new { success = false });
        }


        //Lịch quá hạn
        public async Task<IActionResult> OnPostUpdateLichQuaHanAsync()
        {
            using (var db = new TMS_CoreContext())
            {
                var now = DateTime.Now;
                var lich = db.GhDatPhong.Where(d => (d.TinhTrangId == 1 || d.TinhTrangId == 2) && d.DenNgay < now).ToList(); //Đã đặt - Đã nhận phòng
                foreach (var ds in lich)
                {
                    ds.TrangThai = true;
                    db.GhDatPhong.Update(ds);
                }
                await db.SaveChangesAsync();
            }
            return new JsonResult(new { success = true });
        }


    }
}
