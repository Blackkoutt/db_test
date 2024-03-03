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
using Biblioteka.Repositories;
using Biblioteka.Repositories.DbImplementations;
using Biblioteka.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteka.Pages.Borrowings
{
    [Authorize(Roles = "Reader")]
    public class BookLostModel : PageModel
    {
        private readonly BibContext _context;
        private BorrowingRepository _borrowingRepository;

        public BookLostModel(BibContext context)
        {
            _context = context;
            _borrowingRepository = new BorrowingRepository(context);
        }

        [BindProperty]
        public Borrowing Borrowing { get; set; } = default!;
        public Reader Reader { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (_context.Borrowing == null)
            {
                return NotFound();
            }

            var borrowing = await _context.Borrowing.Include(b => b.book).FirstOrDefaultAsync(b => b.borrowId == id);

            if (borrowing == null && borrowing?.book == null)
            {
                return NotFound();
            }
            else
            {
                Borrowing = borrowing;
                var br = await _context.Reader_Borrowings.FirstOrDefaultAsync(r => r.borrowId == borrowing.borrowId);
                if (br != null)
                {
                    var reader = await _context.Reader.FirstOrDefaultAsync(b => b.readerId == br.readerId);
                    if (reader != null)
                        Reader = reader;
                    else
                        return NotFound();
                }
                else
                    return NotFound();

            }
            return Page();

        }

        public async Task<IActionResult> OnPost(int id)
        {
            if (_context.Borrowing == null)
            {
                return Page();
            }
            else
            {
                var borrowing = await _context.Borrowing.Include(b => b.book).FirstOrDefaultAsync(b => b.borrowId == id);

                if (borrowing != null && borrowing?.book != null && borrowing.Confirmation == true)
                {
                    borrowing.bookLost = true;
                    borrowing.IsReturned = true;
                    Borrowing = borrowing;

                    ModelState.Remove("Borrowing.employee");
                    ModelState.Remove("Borrowing.book");

                    try
                    {
                        _borrowingRepository.Update(Borrowing);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BorrowingExists(Borrowing.borrowId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToPage("./IndexReader");
                }
            }
            return NotFound();
        }

        bool BorrowingExists(int id)
        {
            var isExisted = _borrowingRepository.GetOne(id);

            return isExisted != null ? true : false;
        }
    }
}
