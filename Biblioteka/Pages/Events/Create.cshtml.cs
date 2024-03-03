using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Biblioteka.Context;
using Biblioteka.Models;
using Biblioteka.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Biblioteka.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Pages.Events
{
    public class CreateModel : PageModel
    {
        private IEventRepository _eventRepository;
        private IAuthorRepository _authorRepository;
        private UserManager<BibUser> _userManager;
        private readonly Biblioteka.Context.BibContext _context;

        public CreateModel(Biblioteka.Context.BibContext context, IEventRepository eventRepository, IAuthorRepository authorRepository, UserManager<BibUser> userManager)
        {
            _eventRepository = eventRepository;
            _authorRepository = authorRepository;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Event Event { get; set; } = default!;



        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Event.author");

            if (Event.eventDate < DateTime.Now)
            {
                ModelState.AddModelError("date error", "Data wydarzenia nie może być w przeszłości");
            }

            if (!ModelState.IsValid || Event == null)
            {
                return Page();
            }

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
                        Event.author = foundAuthor;

                        _eventRepository.Add(Event);
                        return RedirectToPage("./Index");
                    }
                }
            }

            return Page();

        }
    }
}
