using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Context;
using Biblioteka.Models;
using Microsoft.AspNetCore.Identity;
using Biblioteka.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteka.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly Biblioteka.Context.BibContext _context;
        private readonly UserManager<BibUser> _userManager;
        public IndexModel(Biblioteka.Context.BibContext context, UserManager<BibUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Event> Event { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Event != null)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                {
                    Event = await _context.Event.Include(e => e.author).ToListAsync();
                }
                else if(User.IsInRole("Author"))
                {
                    var loggedInUserId = _userManager.GetUserId(User);

                    if (loggedInUserId != null)
                    {
                        var user = await _userManager.FindByIdAsync(loggedInUserId);

                        if (user != null)
                        {
                            string email = user.Email;

                            var foundAuthor = _context.Author.FirstOrDefault(r => r.email == email);

                            if (foundAuthor != null)
                            {
                                Event = await _context.Event.Where(e => e.author.authorId == foundAuthor.authorId).ToListAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}
