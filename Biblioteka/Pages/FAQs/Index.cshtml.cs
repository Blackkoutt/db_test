using Biblioteka.Areas.Identity.Data;
using Biblioteka.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteka.Pages.FAQs
{
    public class IndexModel : PageModel
    {
        private readonly Context.BibContext _context;
        public IndexModel(Context.BibContext context)
        {
            _context = context;
        }

        public IList<FAQ> FAQList { get; set; } = default!;

        public void OnGet()
        {
            if (FAQList == null)
            {
                FAQList = _context.FAQ.ToList();
            }
        }
    }
}
