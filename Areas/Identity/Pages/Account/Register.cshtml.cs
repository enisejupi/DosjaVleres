using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Sherbime;
using KosovaDoganaModerne.Depo;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Perdoruesi> _signInManager;
        private readonly UserManager<Perdoruesi> _userManager;
        private readonly IUserStore<Perdoruesi> _userStore;
        private readonly IUserEmailStore<Perdoruesi> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly SherbimetActiveDirectory _adSherbimi;
        private readonly IDepoja_KerkeseRegjistrim _depojaKerkesave;

        public RegisterModel(
            UserManager<Perdoruesi> userManager,
            IUserStore<Perdoruesi> userStore,
            SignInManager<Perdoruesi> signInManager,
            ILogger<RegisterModel> logger,
            SherbimetActiveDirectory adSherbimi,
            IDepoja_KerkeseRegjistrim depojaKerkesave,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _adSherbimi = adSherbimi;
            _depojaKerkesave = depojaKerkesave;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email adresa ose username (emri.mbiemri) është i detyrueshëm.")]
            [Display(Name = "Email ose Username")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Kontrollo nëse Active Directory është aktivizuar
            var adEnabled = bool.Parse(_configuration["ActiveDirectory:Enabled"] ?? "false");
            
            if (adEnabled)
            {
                // Nëse AD është aktivizuar, nuk lejohet regjistrimi manual
                TempData["Info"] = "Regjistrimi manual nuk është i disponueshëm. Ju lutem përdorni kredencialet tuaja të Active Directory për të hyrë në sistem.";
                _logger.LogInformation("Tentativë për të aksesuar faqen e regjistrimit kur AD është aktivizuar");
                return RedirectToPage("./Login", new { returnUrl });
            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Ridrejto te Login sepse regjistrimi është çaktivizuar
            var adEnabled = bool.Parse(_configuration["ActiveDirectory:Enabled"] ?? "false");
            
            if (adEnabled)
            {
                TempData["Info"] = "Regjistrimi manual nuk është i disponueshëm. Ju lutem përdorni kredencialet tuaja të Active Directory për të hyrë në sistem.";
                return RedirectToPage("./Login");
            }

            return Page();
        }

        private Perdoruesi CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Perdoruesi>();
            }
            catch
            {
                throw new InvalidOperationException($"Nuk mund të krijohet një instancë e '{nameof(Perdoruesi)}'. " +
                    $"Sigurohuni që '{nameof(Perdoruesi)}' nuk është një klasë abstrakte dhe ka një konstruktor pa parametra.");
            }
        }

        private IUserEmailStore<Perdoruesi> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("UI e parazgjedhur kërkon një magazinë përdoruesi me mbështetje për email.");
            }
            return (IUserEmailStore<Perdoruesi>)_userStore;
        }

        private string TranslateError(string error)
        {
            // Përkthimi i gabimeve të zakonshme nga anglishtja në shqip
            if (error.Contains("is already taken"))
                return "Ky email është tashmë në përdorim.";
            if (error.Contains("Passwords must have at least one"))
                return "Fjalëkalimi duhet të ketë të paktën një shkronjë të madhe, një të vogël, një numër dhe një karakter special.";
            if (error.Contains("requires a unique email"))
                return "Kërkohet një email unik.";
            
            return error;
        }
    }
}
