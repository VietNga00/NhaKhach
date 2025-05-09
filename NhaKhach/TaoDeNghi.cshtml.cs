using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class TaoDeNghiModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;
        public TaoDeNghiModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public GhDeNghi DeNghi { get; set; }
        public GhPhong Phong { get; set; }
        public GhLoaiPhong Loai { get; set; }
        public int? VienChucId = 0;
        public List<GhDanhSachKhach> DanhSachKhach { get; set; } = new List<GhDanhSachKhach>();


        public async Task<IActionResult> OnGet(int? denghiID)
        {         
            var accessToken = common.RefreshAccessToken(HttpContext);
            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            if (denghiID.HasValue)
            {
                this.DeNghi = await client.For<GhDeNghi>().Key(denghiID.Value).FindEntryAsync();                              
                if(DeNghi == null)
                {
                    return NotFound();
                }

                //Check quyền
                List<string> dsAdmin = new List<string> { "nvvnga1103@gmail.com", }; // "nhukhanhtv052@gmail.com" / nvvnga1103@gmail.com
                bool isAdmin = dsAdmin.Contains(email.ToLower());
                bool isNguoiTao = (DeNghi.NguoiTaoId == taikhoan.VienChucId);
                if (!isAdmin && !isNguoiTao)
                {
                    return new RedirectToPageResult("NhaKhach/AccessDeny");
                }

                this.DanhSachKhach = (await client.For<GhDanhSachKhach>().Filter(f => f.DeNghiId == denghiID.Value).FindEntriesAsync()).ToList();               
            }
            else
            {
                this.DeNghi = new GhDeNghi();  //Tạo đề nghị

            }

            return Page();
        }

        //Tạo mới đề nghị
        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);
            string url = Request.Path;
            string email = User.FindFirst("Email").Value;
            var client = new ODataClient(common.SetODataToken(HttpContext.GetTokenAsync("access_token").Result));
            var taikhoan = await client.For<TaiKhoan>().Filter(f => f.Email == email).FindEntryAsync();
            this.VienChucId = taikhoan.VienChucId;

            //Lưu đề nghị
            DeNghi.DonViId = Convert.ToInt32(Request.Form["DonViId"]);
            DeNghi.NguoiTaoId = (int)VienChucId;
            DeNghi.NgayTao = DateTime.Now;
            DeNghi.NoiDung = Request.Form["NoiDung"];
            DeNghi.SoLuongKhach = Convert.ToInt32(Request.Form["SoLuongKhach"]);
            DeNghi.GhiChu = Request.Form["GhiChu"];
            DeNghi.TinhTrangId = 7;     //Khởi tạo

            _dbContext.GhDeNghi.Add(DeNghi);
            await _dbContext.SaveChangesAsync();

            //Lưu danh sách khách
            int id = DeNghi.Id;
            var listkhach = JsonConvert.DeserializeObject<List<GhDanhSachKhach>>(Request.Form["DanhSachKhach"]);
            foreach(var khach in listkhach)
            {
                khach.DeNghiId = id;
               // khach.NgayTao = DateTime.Now;
                _dbContext.GhDanhSachKhach.Add(khach);
            }
            await _dbContext.SaveChangesAsync();

            return new JsonResult(new {success = true}); 
        }


        //Cập nhật đề nghị
        public async Task<IActionResult> OnPostUpdateDeNghiAsync()
        {
            var accessToken = common.RefreshAccessToken(HttpContext);

            var form = await Request.ReadFormAsync();
            var id = Convert.ToInt32(form["DeNghiId"]);
            var ds = form["DanhSachKhach"];          

            DeNghi = await _dbContext.GhDeNghi.FindAsync(id);
            if (DeNghi == null)
            {
                return new JsonResult(new {success = false, message = "Không tìm thấy đề nghị"});
            }

            //Lưu đề nghị
            DeNghi.DonViId = Convert.ToInt32(Request.Form["DonViId"]);                     
            DeNghi.NoiDung = Request.Form["NoiDung"];
            DeNghi.SoLuongKhach = Convert.ToInt32(Request.Form["SoLuongKhach"]);
            DeNghi.GhiChu = Request.Form["GhiChu"];
            //DeNghi.NgayCapNhat = DateTime.Now;

            _dbContext.GhDeNghi.Update(DeNghi);

            //Xử lý danh sách khách
            var dsKhachMoi = JsonConvert.DeserializeObject<List<GhDanhSachKhach>>(Request.Form["DanhSachKhach"]);
            var dsKhachCu = await _dbContext.GhDanhSachKhach.Where(k => k.DeNghiId == id).ToListAsync();

            foreach(var old in dsKhachCu)
            {
                if(!dsKhachMoi.Any(x => x.Id == old.Id))
                {
                    _dbContext.GhDanhSachKhach.Remove(old); //Xóa khách 
                }
            }
            
            foreach(var newKh in dsKhachMoi.Where(x => x.Id == 0 || x.Id == null )){
                var khachmoi = new GhDanhSachKhach
                {
                    DeNghiId = id,
                    TenKhach = newKh.TenKhach,
                    SoCccd = newKh.SoCccd,
                    GioiTinh = newKh.GioiTinh,
                    ChucDanh = newKh.ChucDanh,
                    Sdt = newKh.Sdt
                   // NgayTao = DateTime.Now
                };
                _dbContext.GhDanhSachKhach.Add(khachmoi);  //Thêm khách mới
            }

            foreach (var newKh in dsKhachMoi.Where(x => x.Id > 0))
            {
                var khachcu = dsKhachCu.FirstOrDefault(x => x.Id == newKh.Id);
                if (khachcu != null)
                {
                    khachcu.TenKhach = newKh.TenKhach;
                    khachcu.SoCccd = newKh.SoCccd;
                    khachcu.GioiTinh = newKh.GioiTinh;
                    khachcu.ChucDanh = newKh.ChucDanh;
                    khachcu.Sdt = newKh.Sdt;
                    //khachcu.NgayCapNhat = DateTime.Now;

                    _dbContext.GhDanhSachKhach.Update(khachcu); //Cập nhật khách
                }
            }
            await _dbContext.SaveChangesAsync();

            return new JsonResult(new { success = true});
        }

    }
}
