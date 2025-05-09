using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TMSWeb_Core.Pages.NhaKhach
{
    public class InDeNghiModel : PageModel
    {

        [BindProperty]
        public XtraReport report { get; set; }


        public int? Id = 0;

        public async Task<IActionResult> OnGetAsync(int? denghi)
        {
            if (denghi == null)
            {
                return NotFound();
            }

            this.Id = denghi;
            if (this.Id != null)
            {
                report = new Report_DeNghi();
                report.Parameters["denghiID"].Value = this.Id;
                report.Parameters["denghiID"].Visible = false;

            }
            else
            {
                return NotFound();
            }

            return Page();
        }
    }
}
