using Biblioteka.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Biblioteka.Areas.Identity.Pages.Account.Manage
{
    public class NotifyFreqModel : PageModel
    {
        private readonly BibContext _context;

        public NotifyFreqModel(BibContext context)
        {
            _context = context;
        }

        [TempData]
        public int CurrentDaysBeforeReturn { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var reader = await _context.Reader.FirstOrDefaultAsync(r => r.email == userEmail);

            if (reader != null)
            {
                CurrentDaysBeforeReturn = reader.DaysBeforeReturn;
                return Page();
            }
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync(int notificationDays)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var reader = await _context.Reader.FirstOrDefaultAsync(r => r.email == userEmail);

            if (reader != null)
            {
                reader.DaysBeforeReturn = notificationDays;
                await _context.SaveChangesAsync();

                CurrentDaysBeforeReturn = notificationDays;

                return Page();
            }

            return Page();
        }
    }
}
