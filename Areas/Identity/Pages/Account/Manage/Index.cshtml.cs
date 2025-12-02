using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KosovaDoganaModerne.Modelet.Entitetet;
using System.Threading.Tasks;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Perdoruesi> _userManager;

        public IndexModel(UserManager<Perdoruesi> userManager)
        {
            _userManager = userManager;
        }

        public Perdoruesi CurrentUser { get; set; }
        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CurrentUser = user;
            IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            return Page();
        }
    }
}
