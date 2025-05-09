using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
    public class EditLichModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public EditLichModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty(Name = "idlich", SupportsGet = true)]
        public string lichId { get; set; }

        public GhDatPhong DatPhong { get; set; }
        public GhPhong Phong { get; set; }
        public GhLoaiPhong Loai { get; set; }
        public GhDanhSachKhach Khach { get; set; }
        public GhDeNghi DeNghi { get; set; }
        public VienChuc NguoiDat { get; set; }
        public DonVi DonVi { get; set; }

        public int? VienChucId = 0;
        public int? id = 0;



        public async Task<IActionResult> OnGetAsync()
        {

            var accessToken = common.RefreshAccessToken(HttpContext);
            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            if (!(await common.checkRoleAsync(email, url, "Create", HttpContext.GetTokenAsync("access_token").Result)))
            {
                return new RedirectToPageResult("NhaKhach/AccessDeny");
            }

            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));

            Debug.WriteLine($"lichId {lichId}");

            int ID = 0;
            if (lichId != null)
            {
                var decode = common.DecryptString("b14ca54545451215451FSDFSEVDa1916", lichId);
                lichId = decode;
                ID = Convert.ToInt32(lichId);
                Debug.WriteLine($"lichId {lichId} - ID {ID}");

                this.id = ID;

                Debug.WriteLine($"this {this.id}");
                this.DatPhong = await client.For<GhDatPhong>().Filter(f => f.Id == this.id).Expand("DeNghi", "Phong($expand=Loai)").FindEntryAsync();
                        
            }
            else
            {
                return new JsonResult(new { success = false, message = "Mã lịch đặt không hợp lệ" });
            }
            return Page();
        }
    }
}



//DatPhong = _dbContext.GhDatPhong.FirstOrDefault(d => d.Id == ID);
//if (DatPhong != null)
//{
//    NguoiDat = _dbContext.VienChuc.FirstOrDefault(v => v.Id == DatPhong.NguoiDatId);
//    DeNghi = _dbContext.GhDeNghi.FirstOrDefault(d => d.Id == DatPhong.DeNghiId);
//    DonVi = _dbContext.DonVi.FirstOrDefault(d => d.Id == DeNghi.DonViId);
//    Phong = _dbContext.GhPhong.FirstOrDefault(d => d.Id == DatPhong.PhongId);
//    Loai = _dbContext.GhLoaiPhong.FirstOrDefault(d => d.Id == Phong.LoaiId);
//    Khach = _dbContext.GhDanhSachKhach.FirstOrDefault(k => k.Id == DatPhong.KhachId);
//    //DatPhong = await _dbContext.GhDatPhong
//    //    .Include(p => p.Phong)
//    //    .ThenInclude(p => p.Loai)
//    //    .Include(p => p.TinhTrang)
//    //    .Include(p => p.NguoiDat)
//    //    .Include(p => p.NguoiDuyet)
//    //    .Include(p => p.DeNghi)
//    //    .FirstOrDefaultAsync(d => d.Id == ID);

//    //DonVi = await _dbContext.DonVi.FirstOrDefaultAsync(d => d.Id == DatPhong.DeNghi.DonViId);
//    //Khach = await _dbContext.GhDanhSachKhach.Where(k => k.Id == DatPhong.KhachId).FirstOrDefaultAsync();

//    Debug.WriteLine($"dp {DatPhong.Id} - {DatPhong.Phong.MaPhong} / DonVi {DonVi.TenDonVi} - {Khach.TenKhach}");

//}
//else
//{
//    return new JsonResult(new { success = false, message = "Không tìm thấy thông tin lịch đặt" });
//}