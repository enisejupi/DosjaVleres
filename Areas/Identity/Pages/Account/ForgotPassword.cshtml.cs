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
        public ForgotPasswordModel()
        {
        }

        public IActionResult OnGet()
        {
            // Redirect to login with message that password reset is disabled
            TempData["Info"] = "Rivendosja e fjalëkalimit është çaktivizuar. Fjalëkalimet menaxhohen nga Active Directory. Për ndryshime, kontaktoni departamentin IT.";
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost()
        {
            // Redirect to login with message that password reset is disabled
            TempData["Info"] = "Rivendosja e fjalëkalimit është çaktivizuar. Fjalëkalimet menaxhohen nga Active Directory. Për ndryshime, kontaktoni departamentin IT.";
            return RedirectToPage("./Login");
        }
    }
}
