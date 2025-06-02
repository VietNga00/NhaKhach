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
    public class ThongTinPhongModel : PageModel
    {
        private readonly TMS_CoreContext _dbContext;

        public ThongTinPhongModel(TMS_CoreContext dbContext)
        {
            _dbContext = dbContext;
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

            return Page();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            using (TMS_CoreContext db = new TMS_CoreContext())
            {
                var phong = await db.GhPhong.FindAsync(id);
                if (phong == null)
                {
                    return new JsonResult(new { success = false, message = "Không có dữ liệu tương ứng với phòng này" });
                }

                phong.TrangThai = !phong.TrangThai;

                await db.SaveChangesAsync();

                return new JsonResult(new { success = true, status = phong.TrangThai });
            }

        }


    }
}
