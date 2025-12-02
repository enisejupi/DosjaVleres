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

        public ChangePasswordModel(UserManager<Perdoruesi> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Redirect to profile page with message that password changes are disabled
            TempData["Warning"] = "Ndryshimi i fjalëkalimit është çaktivizuar. Fjalëkalimet menaxhohen nga Active Directory. Për ndryshime, kontaktoni departamentin IT.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Redirect to profile page with message that password changes are disabled
            TempData["Warning"] = "Ndryshimi i fjalëkalimit është çaktivizuar. Fjalëkalimet menaxhohen nga Active Directory. Për ndryshime, kontaktoni departamentin IT.";
            return RedirectToPage("./Index");
        }
    }
}
