using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Context;
using Biblioteka.Models;
using Microsoft.AspNetCore.Identity;
using Biblioteka.Areas.Identity.Data;
using Biblioteka.Repositories.Interfaces;
using Biblioteka.Services;

namespace Biblioteka.Pages
{
    public class IndexModel : PageModel
    {

        private readonly BibContext _bibContext;
        private readonly UserManager<BibUser> _userManager;
        private IBorrowingRepository _borrowingRepository;
        private readonly IEmailSender _emailSender;

        public IndexModel(BibContext bibContext, UserManager<BibUser> userManager, IBorrowingRepository borrowingRepository, IEmailSender emailSender)
        {
            _userManager = userManager;
            _bibContext = bibContext;
            _borrowingRepository = borrowingRepository;
            _emailSender = emailSender;
        }
        public IList<Borrowing> Borrowing { get; set; } = default!;
        public IList<Reader> Readers { get; set; } = default!;
        public IList<Reader_Borrowings> Reader_Borrowings { get; set; } = default!;

        public IList<Event> Events { get; set; } = default!;

        private async Task SendReminderEmailAsync(string userEmail, string subject, string message)
        {
            await _emailSender.SendEmailAsync(userEmail, subject, message);
        }

        public async Task OnGet()
        {
            var loggedInUserId = _userManager.GetUserId(User);

            if (loggedInUserId != null)
            {
                var user = await _userManager.FindByIdAsync(loggedInUserId);

                if (user != null)
                {
                    string name = user.name;
                    string surname = user.surname;

                    var foundReader = _bibContext.Reader.FirstOrDefault(r => r.name == name && r.surname == surname);

                    if (foundReader != null)
                    {
                        var foundBorrowings = await _bibContext.Borrowing
                            .Include(b => b.book)
                            .ThenInclude(ba => ba.authors)
                            .Where(b =>
                                b.readers.Any(rb => rb.readerId == foundReader.readerId) &&
                                b.IsReturned != true)
                            .ToListAsync();

                        if (foundBorrowings != null)
                        {
                            Borrowing = foundBorrowings;

                            foreach (var item in Borrowing)
                            {
                                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                                var startDate = today;
                                var endDate = item.plannedReturnDate;
                                var timeDifference = endDate - startDate;
                                var daysDifference = (int)Math.Floor(timeDifference.TotalDays);

                                var notificationDays = foundReader.DaysBeforeReturn;
                                ViewData["DaysBeforeReturn"] = notificationDays;
                                if (daysDifference <= notificationDays && daysDifference >= 0)
                                {
                                    var subject = "Przypomnienie o zwrocie ksi��ki";
                                    var daysRemaining = daysDifference == 0 ? "dzi�" : $"{daysDifference} {(daysDifference == 1 ? "dzie�" : "dni")}";
                                    var message = $"Zbli�a si� termin zwrotu ksi��ki {item.book.title}. Prosimy o zwr�cenie jej w terminie. Masz jeszcze {daysRemaining} na zwrot.";

                                    // Wysy�anie powiadomienia e-mail
                                    await SendReminderEmailAsync(user.Email, subject, message);
                                }
                            }
                        }
                        else
                        {
                            Borrowing = null;
                        }
                    }
                    else
                    {
                        Borrowing = null;
                    }
                }
            }

            Events = _bibContext.Event.Include(e => e.author).ToList();
        }

        public void OnPostGetAll()
        {
        }
    }
}