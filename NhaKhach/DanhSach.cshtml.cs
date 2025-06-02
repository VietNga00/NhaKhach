using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simple.OData.Client;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;

namespace TMSWeb_Core.Pages.NhaKhach
{
    public class DanhSachModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public DanhSachModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GhDatPhong DatPhong { get; set; }
        public GhDeNghi DeNghi { get; set; }
        public int? VienChucId { get; set; } = 0;
        public List<GhDanhSachKhach> DanhSachKhach { get; set; } = new();



        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string email = User.FindFirst("Email").Value;

            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            return Page();
        }


        //Khách trong đề nghị có đủ lịch chưa
        public IActionResult OnGetCheckStatus()
        {
            var dsdenghi = _dbContext.GhDeNghi.ToList();

            foreach (var dn in dsdenghi)
            {
                var dskhach = _dbContext.GhDanhSachKhach.Where(k => k.DeNghiId == dn.Id).ToList();
                if (dskhach.Count() == 0) continue;
                bool isStatus = dskhach.All(k => k.TrangThai == 1 || k.TrangThai == 2); //Khách có trạng thái Lịch đã duyệt or Không duyệt lịch

                var solich = _dbContext.GhDatPhong.Where(l => l.DeNghiId == dn.Id && l.KhachId != null).Select(l => l.KhachId.Value).Distinct().Count();
                bool ischeck = dskhach.Count() == solich;
                if (isStatus && ischeck)
                {
                    dn.TinhTrangId = 6; //Hoàn tất
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
                DeNghi.TinhTrangId = 7; //Đã hủy                            
                _dbContext.GhDeNghi.Update(DeNghi);

                // Cập nhật trạng thái khách
                DanhSachKhach = _dbContext.GhDanhSachKhach.Where(d => d.DeNghiId == DeNghi.Id).ToList();
                if (DanhSachKhach.Any())
                {
                    foreach (var khach in DanhSachKhach)
                    {
                        khach.TrangThai = 3; // Hủy
                        khach.IsCancel = true; // Đề nghị đã hủy
                        _dbContext.GhDanhSachKhach.Update(khach);
                    }
                }
                _dbContext.SaveChanges();
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false, message = "Không tìm thấy đề nghị" });
        }

    }
}
