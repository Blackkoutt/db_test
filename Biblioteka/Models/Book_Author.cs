using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Biblioteka.Models
{
    public class Book_Author
    {
        [
        Range(0, 9999)]
        public int Id { get; set; }

        [BindProperty(SupportsGet = true),
            Display(Name = "Id książki")]
        public int bookId { get; set; }
        [BindProperty(SupportsGet = true),
            Display(Name = "Id autora")]
        public int authorId { get; set; }
        public Book book { get; set; }
        public Author author { get; set; }
    }
}