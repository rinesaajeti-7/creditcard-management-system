using CreditCard.Data;
using CreditCard.Helpers;
using CreditCard.Models;
using CreditCard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditCard.Controllers
{
    [Authorize] // Vetëm përdorues të loguar
    public class CreditCardsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly EncryptionHelper _crypto;

        public CreditCardsController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _crypto = new EncryptionHelper(config["EncryptionSettings:AESKey"] 
                ?? throw new InvalidOperationException("AESKey missing in config"));
        }

        // GET: /CreditCards
        [HttpGet]
        public IActionResult Index()
        {
            var cards = _db.CreditCards
                .Where(c => c.UserId == 1) // TODO: Zëvendëso me User.FindFirstValue(ClaimTypes.NameIdentifier)
                .Select(c => new CreditCardListViewModel
                {
                    Id = c.Id,
                    CardHolderName = c.CardHolderName,
                    Last4Digits = c.CardNumberEncrypted != null 
                        ? _crypto.Decrypt(c.CardNumberEncrypted).Substring(12, 4)
                        : "****",
                    ExpirationMonth = c.ExpirationMonth,
                    ExpirationYear = c.ExpirationYear
                })
                .ToList();

            return View(cards);
        }

        // GET: /CreditCards/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /CreditCards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreditCardCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var card = new CreditCard.Models.CreditCard
            {
                CardHolderName = model.CardHolderName,
                CardNumberEncrypted = _crypto.Encrypt(model.CardNumber),
                CvvEncrypted = _crypto.Encrypt(model.Cvv),
                ExpirationMonth = model.ExpirationMonth,
                ExpirationYear = model.ExpirationYear,
                UserId = 1 // TODO: Merr nga User.Identity
            };

            _db.CreditCards.Add(card);
            _db.SaveChanges();

            TempData["Success"] = "Credit card added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /CreditCards/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var card = _db.CreditCards
                .Where(c => c.Id == id && c.UserId == 1)
                .Select(c => new CreditCardDetailsViewModel
                {
                    Id = c.Id,
                    CardHolderName = c.CardHolderName,
                    Last4Digits = c.CardNumberEncrypted != null 
                        ? _crypto.Decrypt(c.CardNumberEncrypted).Substring(12, 4) 
                        : "****",
                    ExpirationMonth = c.ExpirationMonth,
                    ExpirationYear = c.ExpirationYear
                })
                .FirstOrDefault();

            if (card == null)
                return NotFound();

            return View(card);
        }

        // GET: /CreditCards/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var card = _db.CreditCards
                .Where(c => c.Id == id && c.UserId == 1)
                .Select(c => new CreditCardEditViewModel
                {
                    Id = c.Id,
                    CardHolderName = c.CardHolderName,
                    Last4Digits = c.CardNumberEncrypted != null 
                        ? _crypto.Decrypt(c.CardNumberEncrypted).Substring(12, 4) 
                        : "****",
                    Cvv = "", // Mos e shfaq CVV të vjetër për siguri
                    ExpirationMonth = c.ExpirationMonth,
                    ExpirationYear = c.ExpirationYear
                })
                .FirstOrDefault();

            if (card == null)
                return NotFound();

            return View(card);
        }

        // POST: /CreditCards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreditCardEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var card = _db.CreditCards.FirstOrDefault(c => c.Id == id && c.UserId == 1);
            if (card == null)
                return NotFound();

            card.CardHolderName = model.CardHolderName;
            card.ExpirationMonth = model.ExpirationMonth;
            card.ExpirationYear = model.ExpirationYear;

            // Update CVV vetëm nëse është futur i ri
            if (!string.IsNullOrWhiteSpace(model.Cvv))
                card.CvvEncrypted = _crypto.Encrypt(model.Cvv);

            _db.SaveChanges();

            TempData["Success"] = "Credit card updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /CreditCards/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var card = _db.CreditCards
                .Where(c => c.Id == id && c.UserId == 1)
                .Select(c => new CreditCardDeleteViewModel
                {
                    Id = c.Id,
                    CardHolderName = c.CardHolderName,
                    Last4Digits = c.CardNumberEncrypted != null 
                        ? _crypto.Decrypt(c.CardNumberEncrypted).Substring(12, 4) 
                        : "****",
                    ExpirationMonth = c.ExpirationMonth,
                    ExpirationYear = c.ExpirationYear
                })
                .FirstOrDefault();

            if (card == null)
                return NotFound();

            return View(card);
        }

        // POST: /CreditCards/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, CreditCardDeleteViewModel model)
        {
            var card = _db.CreditCards.FirstOrDefault(c => c.Id == id && c.UserId == 1);
            if (card == null)
                return NotFound();

            _db.CreditCards.Remove(card);
            _db.SaveChanges();

            TempData["Success"] = "Credit card deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        
    }
}