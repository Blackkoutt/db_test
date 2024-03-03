using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Repositories.Interfaces;
using Biblioteka.Models;

namespace Biblioteka.Pages.FAQs
{
    public class DeleteModel : PageModel
    {
        private readonly Biblioteka.Context.BibContext _context;
        public DeleteModel(Biblioteka.Context.BibContext context)
        {
            _context = context;
        }

        [BindProperty]
      public FAQ FAQ { get; set; } = default!;

        public IActionResult OnGet(int id)
        {

            var faq = _context.FAQ.FirstOrDefault(f => f.FAQId == id);

            if (faq == null)
            {
                return NotFound();
            }
            else 
            {
                FAQ = faq;
            }
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            _context.FAQ.Remove(FAQ);
            _context.SaveChanges();

            return RedirectToPage("./All");
        }
    }
}
