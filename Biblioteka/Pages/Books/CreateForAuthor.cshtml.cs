using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Biblioteka.Repositories.Interfaces;
using Biblioteka.Context;
using Biblioteka.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Biblioteka.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Biblioteka.Views.Books
{
    [Authorize(Roles = "Author")]
    public class CreateForAuthorModel : PageModel
    {
        private IBookRepository _bookRepository;
        private BibContext _context;
        private readonly ILogger<CreateModel> _logger;
        private UserManager<BibUser> _userManager;

        public CreateForAuthorModel(UserManager<BibUser> userManager, ILogger<CreateModel> logger, IBookRepository bookRepository, BibContext bibcontext)
        {
            _bookRepository = bookRepository;
            _context = bibcontext;
            _logger = logger;
            _userManager = userManager;
        }
        public List<SelectListItem>? Genre { get; set; }
        public List<SelectListItem>? Type { get; set; }
        public List<SelectListItem>? Publisher { get; set; }
        public List<SelectListItem>? Tag { get; set; }
        public IActionResult OnGet()
        {
            Genre = _context.Genre.Select(r => new SelectListItem { Value = r.genreId.ToString(), Text = r.name }).ToList();
            Type = _context.BookType.Select(r => new SelectListItem { Value = r.typeId.ToString(), Text = r.name }).ToList();
            Publisher = _context.Publisher.Select(r => new SelectListItem { Value = r.publisherId.ToString(), Text = r.name }).ToList();
            Tag = _context.Tag.Select(t => new SelectListItem { Value = t.tagId.ToString(), Text = t.name }).ToList();

            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]
        public string GenreId { get; set; } = default!;
        [BindProperty]
        public string[] TagIds { get; set; } = default!;
        [BindProperty]
        public string BookTypeId { get; set; } = default!;

        [BindProperty]
        public string PublisherId { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Book.authors");
            ModelState.Remove("Book.publisher");
            ModelState.Remove("Book.floor");
            ModelState.Remove("Book.alley");
            ModelState.Remove("Book.rowNumber");
            ModelState.Remove("Event.author");
            ModelState.Remove("Book.availableCopys");
            ModelState.Remove("Book.genre");
            ModelState.Remove("Book.type");

            Genre? foundGenre = await _context.Genre.FirstOrDefaultAsync(r => r.genreId.ToString().Equals(GenreId.ToString()));

            if (foundGenre != null)
            {
                Book.genre = foundGenre;
            }

            Book.tags = new List<Book_Tag>();
            bool tag_found = false;

            foreach (var tagId in TagIds)
            {
                Tag? foundTag = await _context.Tag.FirstOrDefaultAsync(r => r.tagId.ToString() == tagId);
                if (foundTag != null)
                {
                    Book.tags.Add(new Book_Tag { tag = foundTag, book = Book });
                    tag_found = true;
                }
            }

            //if(!tag_found) { ModelState.AddModelError("", "Tag jest wymagany"); }


            Publisher? foundPublisher = await _context.Publisher.FirstOrDefaultAsync(r => r.publisherId.ToString().Equals(PublisherId.ToString()));

            if (foundPublisher != null)
            {
                Book.publisher = foundPublisher;
            }
            BookType? foundType = await _context.BookType.FirstOrDefaultAsync(r => r.typeId.ToString().Equals(BookTypeId.ToString()));

            if (foundType != null)
            {
                Book.type = foundType;
            }

            Book.authors = new List<Book_Author>();

            var loggedInUserId = _userManager.GetUserId(User);

            if (loggedInUserId != null)
            {
                var user = await _userManager.FindByIdAsync(loggedInUserId);

                if (user != null)
                {
                    string email = user.Email;

                    var foundAuthor = await _context.Author.FirstOrDefaultAsync(r => r.email == email);

                    if (foundAuthor != null)
                    {

                        // Add to the existing authors list
                        //Book.auth-cors.Add(new Book_Author { author = foundAuthor, book = Book });
                        //
                        int max = 1;
                        if (_context.Book_Author != null)
                        {
                            foreach (var item in _context.Book_Author)
                            {
                                if (max < item.Id)
                                    max = item.Id;
                            }

                        }
                        int idBA = max+1;

                        Book.authors.Add(new Book_Author { Id = idBA, author = foundAuthor, book = Book });

                        // _context.Book_Author.Add(new Book_Author { Id = idBA, author = foundAuthor, book = Book });
                        //await _context.SaveChangesAsync();
                        //Book.authors.Add(new Book_Author { author = foundAuthor, book = Book });
                    }
                }
            }




            if (!ModelState.IsValid || Book == null)
            {
                return Page();
            }

            _bookRepository.Add(Book);

            return RedirectToPage("./AuthorBooks");
        }
    }
}