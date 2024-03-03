using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Biblioteka.Repositories.Interfaces;
using Biblioteka.Models;
using System.Diagnostics;

namespace Biblioteka.Pages.FAQs
{
    public class CreateModel : PageModel
    {
        private readonly Biblioteka.Context.BibContext _context;
        public CreateModel(Biblioteka.Context.BibContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FAQ FAQ { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPost()
        {            
            if (!ModelState.IsValid ||  FAQ == null)
            {
                return Page();
            }

            if (_context.FAQ.Count() <= 0)
            {
                FAQ.FAQId = 1;

                _context.FAQ.Add(FAQ);
                _context.SaveChanges();
                return RedirectToPage("./All");
            }
            else
            {
                var temp = _context.FAQ.OrderByDescending(f => f.FAQId).FirstOrDefault();

                if (temp != null)
                {
                    FAQ.FAQId = temp.FAQId + 1;

                    _context.FAQ.Add(FAQ);
                    _context.SaveChanges();
                    return RedirectToPage("./All");
                }
                else
                    return NotFound();
            }

            
        }
    }
}
