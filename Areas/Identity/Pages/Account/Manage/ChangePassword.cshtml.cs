using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<Perdoruesi> _userManager;
        private readonly SignInManager<Perdoruesi> _signInManager;

        public ChangePasswordModel(
            UserManager<Perdoruesi> userManager,
            SignInManager<Perdoruesi> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Fjalëkalimi i vjetër është i detyrueshëm")]
            [DataType(DataType.Password)]
            [Display(Name = "Fjalëkalimi i vjetër")]
            public string OldPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Fjalëkalimi i ri është i detyrueshëm")]
            [StringLength(100, ErrorMessage = "{0} duhet të jetë të paktën {2} dhe maksimumi {1} karaktere.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Fjalëkalimi i ri")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Konfirmo fjalëkalimin e ri")]
            [Compare("NewPassword", ErrorMessage = "Fjalëkalimi i ri dhe konfirmimi nuk përputhen.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nuk mund të ngarkohet përdoruesi me ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nuk mund të ngarkohet përdoruesi me ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Fjalëkalimi juaj është ndryshuar me sukses.";

            return RedirectToPage();
        }
    }
}
