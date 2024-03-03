using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteka.Repositories.Interfaces;
using Biblioteka.Models;

namespace Biblioteka.Views.Books
{
    public class IndexModel : PageModel
    {
        private IBookRepository _bookRepository;
        private IGenreRepository _genreRepository;
        private ITagRepository _tagRepository;
        private IAuthorRepository _authorRepository;


        public IndexModel(IBookRepository bookRepository, IGenreRepository genreRepository, ITagRepository tagRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _tagRepository = tagRepository;
            _authorRepository = authorRepository;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public IEnumerable<Book> Book { get; set; }
        public List<Genre> Genres { get; set; }

        public List<Tag> Tags { get; set; }
        public List<Author> Author { get; set; }



        public void OnGet()
        {

            Book = _bookRepository.SearchBooks(SearchTerm);
            Genres = _genreRepository.getAll().ToList();
            Tags = _tagRepository.getAll().ToList();
            Author = _authorRepository.getAll().ToList();
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