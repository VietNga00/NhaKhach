using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class ThongKePhongModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public ThongKePhongModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public List<GhThongKePhong> ThongKe {get; set;}
        public List<GhPhong> DanhSachPhong { get; set; }

        [BindProperty]
        public int namchon { get; set; }
        [BindProperty]
        public int thangchon { get; set; }
        public int namhientai { get; set; } = DateTime.Now.Year;
        public int thanghientai { get; set; } = DateTime.Now.Month;
        [BindProperty]
        public List<int> DanhSachNam { get; set; }
        public string ErrorMessage { get; set; }


        //Danh sách năm
        private async Task LoadDanhSachNam()
        {
            DanhSachNam = await _dbContext.GhTraPhong.Where(x => x.NgayTra.HasValue).Select(x => x.NgayTra.Value.Year).Distinct().OrderByDescending(y => y).ToListAsync();
        }

        //Danh sách phòng
        private async Task LoadDanhSachPhong()
        {
            DanhSachPhong = await _dbContext.GhPhong
                .Select(s => new GhPhong
                {
                    Id = s.Id,
                    MaPhong = s.MaPhong
                }).ToListAsync();
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            if (!(await common.checkRoleAsync(email, url, "Create", HttpContext.GetTokenAsync("access_token").Result)))
            {
                return new RedirectToPageResult("QuanLyNhaKhach/AccessDeny");
            }
            await LoadDanhSachNam();
            await LoadDanhSachPhong();

            namchon = namhientai;
            thangchon = thanghientai;

            await XuLyThongKe(namchon, thangchon);            

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDanhSachNam();
            await LoadDanhSachPhong();

            if (namchon == 0) namchon = namhientai;
            if (thangchon == 0) thangchon = thanghientai;

            await XuLyThongKe(namchon, thangchon);
          
            return Page();
        }

        //
        private async Task XuLyThongKe(int nam, int thang)
        {
            var dulieu = await _dbContext.GhThongKePhong.Where(x => x.Nam == nam && x.Thang == thang).ToListAsync();

            if (dulieu.Any())
            {
                ThongKe = dulieu;
            } else 
            { 
                var thongke = await LayThongKe(nam, thang);
                if (nam > namhientai || (nam == namhientai && thang >= thanghientai))
                {
                    ThongKe = thongke;
                }
                else
                {
                    if (thongke.Any(t => t.SoLichDat > 0))
                    {
                        await _dbContext.GhThongKePhong.AddRangeAsync(thongke);
                        await _dbContext.SaveChangesAsync();
                        ThongKe = thongke;
                    }
                    else
                    {
                        ErrorMessage = $"Không có dữ liệu thống kê cho tháng {thang}/{nam}";
                    }
                }              
            }
        }

        //Tính toán thống kê
        private async Task<List<GhThongKePhong>> LayThongKe(int nam, int thang)
        {
            var thongkephong = await _dbContext.GhDatPhong.Where(x => x.TuNgay.Year == nam && x.TuNgay.Month == thang).GroupBy(x => new {x.PhongId})
                .Select(g => new GhThongKePhong
                {
                    Nam = nam,
                    Thang = thang,
                    PhongId = (int)g.Key.PhongId,
                    SoLichDat = g.Count(),
                    SoLichHuy = g.Count(x => x.TinhTrangId == 4 && x.NgayHuy.HasValue && x.NgayHuy.Value.Year == nam && x.NgayHuy.Value.Month == thang),
                    SoLichQuaHan = g.Count(x => x.TrangThai == true),
                    NgayThongKe = DateTime.Now,

                }).ToListAsync();
            return thongkephong;
        }

       
    }
}
