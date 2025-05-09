using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
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
    public class ChinhSuaLichDatModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public ChinhSuaLichDatModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int? idlich = 0;
        public GhDatPhong DatPhong { get; set; }
        public GhPhong Phong { get; set; }
        public GhLoaiPhong Loai { get; set; }
        public GhDanhSachKhach Khach { get; set; }
        public GhDeNghi DeNghi { get; set; }
        public DonVi DonVi { get; set; }


        public async Task<IActionResult> OnGetAsync(int? lich)
        {
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));

            if(lich == null)
            {
                return NotFound();
            }
            this.idlich = lich;      

            this.DatPhong = await client.For<GhDatPhong>().Filter(f => f.Id == this.idlich).Expand("DeNghi($expand=DonVi)", "Phong", "Phong($expand=Loai)", "Khach").FindEntryAsync();      
           
            return Page();
        }

        //Load danh sách phòng trống
        public async Task<IActionResult> OnGetCheckPhong(DateTime start, DateTime end, bool allDay, int idlich)
        {
            
            var accessToken = common.RefreshAccessToken(HttpContext);
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));

            var dsLich = await client.For<GhDatPhong>().FindEntriesAsync();  Debug.WriteLine(JsonConvert.SerializeObject(dsLich));
            var dsPhong = await client.For<GhPhong>().FindEntriesAsync();

            var lichcungngay = dsLich.Where(l => l.Id != idlich && (l.TinhTrangId == 1 || l.TinhTrangId == 2 ) && (l.TuNgay.Date <= end.Date && l.DenNgay.Date >= start.Date )).ToList();
            //Debug.WriteLine(JsonConvert.SerializeObject(lichcungngay));

            var phongcolich = lichcungngay.Where(l => l.AllDay == true || !(l.DenNgay <= start || l.TuNgay >= end ) ).Select(l => l.PhongId).Where(id => id.HasValue).Distinct().ToList();
          
            var dstrong = dsPhong.Where(p => !phongcolich.Contains(p.Id)).Select(p => new
            {
                p.Id,
                p.MaPhong,
                p.TenPhong
            }).ToList();
          
            var json = JsonConvert.SerializeObject(dstrong, Formatting.Indented);
            return Content(json, "application/json");
        }


        public IActionResult OnGetLoadLoaiPhong(int phongId)
        {
            var LoadLoai = _dbContext.GhPhong.Where(s => s.Id == phongId)
                .Select(s => new {
                    id = s.LoaiId,
                    ten = s.Loai.TenLoai,                   
                }).FirstOrDefault();
            return new JsonResult(LoadLoai);
        }

        public async Task<IActionResult> OnPostUpdateLich(int idlich, int idphong)
        {
            var lich = await _dbContext.GhDatPhong.FindAsync(idlich);
            if (lich == null)
            {
                return new JsonResult(new { success = false, message = "Lịch không tồn tại" });
            }

            lich.PhongId = idphong;
            lich.TinhTrangId = 1; //Đã đặt
            lich.TrangThai = false; //Lịch còn hoạt động
            lich.GhiChu += $" - (Đã chuyển phòng)";
            //lich.NgayCapNhat = DateTime.Now;

            _dbContext.GhDatPhong.Update(lich);

            var khach = _dbContext.GhDanhSachKhach.Where(k => k.Id == lich.KhachId).FirstOrDefault();
            khach.TrangThai = 1; //Đã có lịch
            _dbContext.GhDanhSachKhach.Update(khach);

            await _dbContext.SaveChangesAsync();
            return new JsonResult(new { success = true});
        }

    }
}
