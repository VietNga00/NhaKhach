using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Administrators")]
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
                return new RedirectToPageResult("NhaKhach/AccessDeny");
            }
            await LoadDanhSachNam();
            await LoadDanhSachPhong();

            namchon = namhientai;
            thangchon = thanghientai;

            await CapNhatThongKe(namchon, thangchon);

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
                    if (thongke.Any(t => t.TongSoLich > 0))
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
            var thongkephong = await _dbContext.GhDatPhong.Where(x => x.TuNgay.Year == nam && x.TuNgay.Month == thang).GroupBy(x => new { x.PhongId })
                .Select(g => new GhThongKePhong
                {
                    Nam = nam,
                    Thang = thang,
                    PhongId = (int)g.Key.PhongId,
                    TongSoLich = g.Count(),
                    LichHopLe = g.Count(x => x.TinhTrangId != 2 && x.TinhTrangId != 3 && x.TinhTrangId != 7 && x.TrangThai != true), //Chờ duyệt - Không duyệt - Đã hủy - Quá hạn
                    LichHuy = g.Count(x => x.TinhTrangId == 7 && x.NgayHuy.HasValue && x.NgayHuy.Value.Year == nam && x.NgayHuy.Value.Month == thang),
                    LichQuaHan = g.Count(x => x.TrangThai == true),                    
                    NgayThongKe = DateTime.Now,

                }).ToListAsync();

            //Debug.WriteLine(JsonConvert.SerializeObject(thongkephong));

            return thongkephong;
        }


        private async Task CapNhatThongKe(int nam, int thang)
        {
            for (int i = 1; i < thang; i++)
            {
                var thongkemoi = await LayThongKe(nam, i);
                if (!thongkemoi.Any(t => t.TongSoLich > 0)) continue;

                foreach (var item in thongkemoi)
                {

                    var dulieu = await _dbContext.GhThongKePhong.FirstOrDefaultAsync(x => x.Nam == nam  && x.Thang == i && x.PhongId == item.PhongId );
                    if (dulieu == null)
                    {
                        await _dbContext.GhThongKePhong.AddAsync(item);
                    } else
                    {
                        if(dulieu.TongSoLich != item.TongSoLich || dulieu.LichHopLe != item.LichHopLe || dulieu.LichHuy != item.LichHuy || dulieu.LichQuaHan != item.LichQuaHan)
                        {
                            dulieu.TongSoLich = item.TongSoLich;
                            dulieu.LichHopLe = item.LichHopLe;
                            dulieu.LichHuy = item.LichHuy;
                            dulieu.LichQuaHan = item.LichQuaHan;
                            dulieu.NgayThongKe = DateTime.Now;

                            _dbContext.GhThongKePhong.Update(dulieu);
                        }
                    }
                }               
            }
            await _dbContext.SaveChangesAsync();
        }

    }
}
