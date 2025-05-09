using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TMS_ModelCore.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using TMSWeb_Core.Models;
using System.Diagnostics;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class ThongKeModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public ThongKeModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public List<GhThongKe> ThongKe { get; set; } = new List<GhThongKe>();       
        [BindProperty]
        public List<int> DanhSachNam { get; set; } = new List<int>();
        [BindProperty(SupportsGet = true)]
        public int namchon { get; set; }
        public int namhientai { get; set; } = DateTime.Now.Year;
        public int thanghientai { get; set; } = DateTime.Now.Month;
        public string ErrorMessage { get; set; }


        private async Task LoadDanhSachNam()
        {
            DanhSachNam = await _dbContext.GhTraPhong.Where(x => x.NgayTra.HasValue).Select(x => x.NgayTra.Value.Year).Distinct().OrderByDescending(y => y).ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            if (!(await common.checkRoleAsync(email, url, "Create", HttpContext.GetTokenAsync("access_token").Result)))
            {
                return new RedirectToPageResult("NhaKhach/AccessDeny");
            }

            await LoadDanhSachNam();
            if (namchon == 0) namchon = namhientai;

            await XuLyThongKe(namchon);

            ThongKe = await _dbContext.GhThongKe.Where(x => x.Nam == namchon).ToListAsync();

            var TKhientai = await _dbContext.GhThongKe.FirstOrDefaultAsync(x => x.Nam == namhientai && x.Thang == thanghientai);
            if (TKhientai == null)
            {
                var dulieutam = await LayThongKeThang(namhientai, thanghientai);
                ThongKe.Add(dulieutam);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDanhSachNam();
            if (namchon == 0) namchon = namhientai;
            if(!await _dbContext.GhThongKe.AnyAsync(x => x.Nam == namchon))
            {
                if(namchon == namhientai)
                {
                    await XuLyThongKe(namhientai);
                }
                else
                {
                    ErrorMessage = $"Không có dữ liệu thống kê theo tháng cho năm {namchon}";
                    return Page();
                }
            }

            ThongKe = await _dbContext.GhThongKe.Where(x => x.Nam == namchon).ToListAsync();
            var TKhientai = await _dbContext.GhThongKe.FirstOrDefaultAsync(x => x.Nam == namhientai && x.Thang == thanghientai);
            if (TKhientai == null)
            {
                var dulieutam = await LayThongKeThang(namhientai, thanghientai);
                ThongKe.Add(dulieutam);
            }
            return Page();
        }

        private async Task XuLyThongKe(int nam)
        {
            for (int thang = 1; thang <= thanghientai; thang ++ )
            {
                bool isHienTai = (nam == namhientai && thang == thanghientai);
                var duieutam = await LayThongKeThang(nam, thang);

                if (duieutam.TongLichDat == 0) continue;

                var thongke = await _dbContext.GhThongKe.FirstOrDefaultAsync(x => x.Nam == nam && x.Thang == thang);
                if(thongke != null)
                {
                    if(thongke.TongLichDat != duieutam.TongLichDat || thongke.TongLichQuaHan != duieutam.TongLichQuaHan || thongke.TongLichHuy != duieutam.TongLichHuy || thongke.TongSoKhach != duieutam.TongSoKhach)
                    {
                        thongke.TongLichDat = duieutam.TongLichDat;
                        thongke.TongLichHuy = duieutam.TongLichHuy;
                        thongke.TongLichQuaHan = duieutam.TongLichQuaHan;
                        thongke.TongSoKhach = duieutam.TongSoKhach;

                    }
                } else if (!isHienTai)
                {
                    _dbContext.GhThongKe.Add(duieutam);
                }
                else
                {
                    continue;
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task<GhThongKe> LayThongKeThang(int nam, int thang)
        {
            try
            {
                var datphong = await _dbContext.GhDatPhong.Where(x => x.TuNgay.Year == nam && x.TuNgay.Month == thang).ToListAsync();
                var khachhang = await _dbContext.GhDatPhong.Where(x => x.TuNgay.Year == nam && x.TuNgay.Month == thang && x.KhachId != null).Select(x => x.KhachId).Distinct().CountAsync();


                int solichdat = datphong.Count();
                int solichhuy = datphong.Count(x => x.TinhTrangId == 4 && x.NgayHuy.HasValue);
                int solichquahan = datphong.Count(x => x.TrangThai == true);
                int sokhach = khachhang;               

                return new GhThongKe
                {
                    Nam = nam,
                    Thang = thang,
                    TongLichDat = solichdat,
                    TongLichHuy = solichhuy,
                    TongLichQuaHan = solichquahan,
                    TongSoKhach = sokhach,
                    NgayThongKe = DateTime.Now,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi truy vấn dữ liệu: {ex.Message}");

                ErrorMessage = "Xảy ra lỗi trong qua trình thống kê dữ liệu";
                return new GhThongKe
                {
                    Nam = nam,
                    Thang = thang,
                    TongLichDat = 0,
                    TongLichHuy = 0,
                    TongLichQuaHan = 0,
                    TongSoKhach = 0,                 
                };
            }
        }
    }
}
