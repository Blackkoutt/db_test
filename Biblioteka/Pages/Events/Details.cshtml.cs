using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Context;
using Biblioteka.Models;
using Biblioteka.Repositories.Interfaces;

namespace Biblioteka.Pages.Events
{
    public class DetailsModel : PageModel
    {
        private readonly Biblioteka.Context.BibContext _context;
        private IEventRepository _eventRepository;

        public DetailsModel(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

      public Event Event { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            var eventa = _eventRepository.getOne(id);
            if (eventa == null)
            {
                return NotFound();
            }else {
                Event = eventa;
            }
            return Page();
        }
    }
}
