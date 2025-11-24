using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Perdoruesi> _userManager;

        public ForgotPasswordModel(UserManager<Perdoruesi> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email adresa është e detyrueshme.")]
            [EmailAddress(ErrorMessage = "Email adresa nuk është e vlefshme.")]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Mos zbulohet nëse përdoruesi nuk ekziston
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // Për demostrim - në realitet do të dërgohej email
                // Aktualisht kjo veçori nuk është e implementuar plotësisht
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
