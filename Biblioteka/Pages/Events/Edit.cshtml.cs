using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Context;
using Biblioteka.Models;
using Biblioteka.Repositories.Interfaces;

namespace Biblioteka.Pages.Events
{
    public class EditModel : PageModel
    {
        private readonly Biblioteka.Context.BibContext _context;
        private IEventRepository _eventRepository;

        public EditModel(Biblioteka.Context.BibContext context, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            var eventa = _eventRepository.getOne(id);
            if (eventa == null)
            {
                return NotFound();
            }
            else
            {
                Event = eventa;
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPostAsync()
        {
            ModelState.Remove("Event.author");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                 _eventRepository.Update(Event);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(Event.eventId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EventExists(int id)
        {
            var isExisted = _eventRepository.getOne(id);
          return isExisted != null ? true : false;
        }
    }
}
