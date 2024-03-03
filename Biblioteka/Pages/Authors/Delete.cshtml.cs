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
using Microsoft.AspNetCore.Authorization;
using Biblioteka.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Biblioteka.Pages.Authors
{
   // [Authorize(Roles = "Admin, Employee")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<BibUser> _userManager;
        private IAuthorRepository _authorRepository;
        private IReaderRepository _readerRepository;

        public DeleteModel(
            IAuthorRepository authorRepository, 
            IReaderRepository readerRepository,
            UserManager<BibUser> userManager)
        {
            _authorRepository = authorRepository;
            _readerRepository = readerRepository;
            _userManager = userManager;
        }

        [BindProperty]
        public Author Author { get; set; } = default!;

        public IActionResult OnGet(int id)
        {

            var author = _authorRepository.getOne(id);

            if (author == null)
                return NotFound();
            else
                Author = author;

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var author = _authorRepository.getOne(id);
            var reader = _readerRepository.GetByMail(author.email);
            if (reader != null)
            {
                reader.isAuthor = false;
                BibUser user = _userManager.FindByEmailAsync(author.email).Result;
                _userManager.RemoveFromRoleAsync(user, "Author");
            }
            _authorRepository.Delete(id);

            return RedirectToPage("./Index");
        }
    }
}
