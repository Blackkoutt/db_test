using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteka.Repositories.Interfaces;
using Biblioteka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Areas.Identity.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Biblioteka.Views.Books
{
    public class AuthorBooksModel : PageModel
    {
        private readonly Context.BibContext _context;
        private IBookRepository _bookRepository;
        private readonly UserManager<BibUser> _userManager;
        public AuthorBooksModel(Context.BibContext context, IBookRepository bookRepository, UserManager<BibUser> userManager)
        {
            _bookRepository = bookRepository;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public IEnumerable<Book> Book { get; set; }
        public int AuthorIdForLoggedUser { get; set; }

        public async Task OnGetAsync()
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

                        Book = _context.Book
                            .Include(b => b.tags)
                            .ThenInclude(b => b.tag)
                            .Include(b => b.publisher)
                            .Include(b => b.genre)
                            .Include(b => b.type)
                            .Include(b => b.authors)
                            .ThenInclude(a => a.author)
                            .Where(b => b.authors.Any(a => a.authorId == foundAuthor.authorId))
                            .ToList();
                    }
                }

            }

        }

        /* public void OnPost(int bookId)
         {
             Book book = _bookRepository.getOne(bookId);
             if (book.availableCopys < 1)
             {
                 TempData["ErrorMessage"] = "No copies available for borrowing.";
                 Page();
             }

             RedirectToPage("../Books/Create");
         }*/
    }
}