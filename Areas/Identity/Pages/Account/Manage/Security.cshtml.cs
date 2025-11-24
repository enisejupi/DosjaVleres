using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KosovaDoganaModerne.Modelet.Entitetet;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account.Manage
{
    public class SecurityModel : PageModel
    {
        private readonly UserManager<Perdoruesi> _userManager;
        private readonly SignInManager<Perdoruesi> _signInManager;

        public SecurityModel(
            UserManager<Perdoruesi> userManager,
            SignInManager<Perdoruesi> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string? StatusMessage { get; set; }

        public bool Is2faEnabled { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nuk mund të ngarkohet përdoruesi me ID '{_userManager.GetUserId(User)}'.");
            }

            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostDisable2faAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nuk mund të ngarkohet përdoruesi me ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                StatusMessage = "Gabim gjatë çaktivizimit të 2FA.";
                return RedirectToPage();
            }

            StatusMessage = "2FA është çaktivizuar me sukses.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogoutEverywhereAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nuk mund të ngarkohet përdoruesi me ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            
            StatusMessage = "Të gjitha sesionet e tjera janë ndërprerë me sukses.";
            return RedirectToPage();
        }
    }
}
