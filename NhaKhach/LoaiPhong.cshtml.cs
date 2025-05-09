using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TMS_ModelCore.Models;
using TMSWeb_Core.Models;

namespace TMSWeb_Core.Pages.NhaKhach
{
    [Authorize(Roles = "Users,Administrators")]
    [IgnoreAntiforgeryToken]
    public class LoaiPhongModel : PageModel
    {

        [BindProperty]
        public int id { get; set; }

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


        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            using (TMS_CoreContext db = new TMS_CoreContext())
            {
                var loai = await db.GhLoaiPhong.FindAsync(id);
                if (loai == null)
                {
                    return new JsonResult(new { success = false, message = "Không có dữ liệu tương ứng với loại phòng này" });
                }

                loai.TrangThai = !loai.TrangThai;

                await db.SaveChangesAsync();

                return new JsonResult(new { success = true, status = loai.TrangThai });
            }

        }



    }
}

