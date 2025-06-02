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
            var dsAdmin = new List<string>{ "nvvnga1103@gmail.com", };  

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
                var lichquahan = db.GhDatPhong.Where(d => (d.TinhTrangId == 4 || d.TinhTrangId == 5) && d.DenNgay < now).ToList(); //Đã duyệt - Đã nhận phòng
                var lichkhongnhan = db.GhDatPhong.Where(d => d.TinhTrangId == 4 && d.TuNgay < now.Date && d.TinhTrangId != 5).ToList();

                var tonghop = lichquahan.Union(lichkhongnhan).Distinct().ToList();

                foreach (var ds in tonghop)
                {
                    ds.TrangThai = true;                   
                }
                await db.SaveChangesAsync();
            }
            return new JsonResult(new { success = true });
        }


    }
}
