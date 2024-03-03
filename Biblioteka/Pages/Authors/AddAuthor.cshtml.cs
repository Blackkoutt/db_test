using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Biblioteka.Context;
using Biblioteka.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.OracleClient;
using Biblioteka.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Biblioteka.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Biblioteka.Pages.Authors
{
    //[Authorize(Roles = "Admin, Employee")]
    public class AddAuthorModel : PageModel
    {
        private readonly UserManager<BibUser> _userManager;
        private BibContext _context;
        private IAuthorRepository _authorRepository;
        private IReaderRepository _readerRepository;

        public AddAuthorModel(
            IAuthorRepository authorRepository, 
            IReaderRepository readerRepository, 
            UserManager<BibUser> userManager,
            BibContext bibcontext)
        {
            _authorRepository = authorRepository;
            _readerRepository = readerRepository;
            _userManager = userManager;
            _context = bibcontext;
        }
        public List<SelectListItem>? Reader { get; set; }
        public IActionResult OnGet()
        {
            Reader = _context.Reader.Select(r => new SelectListItem { Value = r.readerId.ToString(), Text = r.name + " " + r.surname }).ToList();
            return Page();
        }

        [BindProperty]
        public string ReaderId { get; set; }
        public Author Author { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || ReaderId == null)
            {
                return Page();
            }
            Reader? foundReader = _context.Reader.FirstOrDefault(r => r.readerId.ToString().Equals(ReaderId.ToString()));
            if (foundReader == null)
            {
                ModelState.AddModelError("readerNotFound", "Nie istnieje konto czytelnika z podanym emailem");
                return Page();
            }
            else 
            {
                Author = new Author();
                BibUser user = _userManager.FindByEmailAsync(foundReader.email).Result;
                _userManager.AddToRoleAsync(user, "Author");
                foundReader.isAuthor = true;

                Author.name = foundReader.name;
                Author.surname = foundReader.surname;
                Author.birthDate = foundReader.birthDate ?? default(DateTime);
                Author.email = foundReader.email;
                _authorRepository.Add(Author);
            }
            return RedirectToPage("./Index");
        }
    }
}
