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
using Microsoft.Extensions.Logging;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Sherbime;
using KosovaDoganaModerne.Depo;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
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
            IDepoja_KerkeseRegjistrim depojaKerkesave)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _adSherbimi = adSherbimi;
            _depojaKerkesave = depojaKerkesave;
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

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid)
            {
                // Kontrollo nëse përdoruesi ekziston tashmë në sistem
                var ekzistonPerdoruesi = await _userManager.FindByEmailAsync(Input.Email) ?? 
                                         await _userManager.FindByNameAsync(Input.Email);
                
                if (ekzistonPerdoruesi != null)
                {
                    ModelState.AddModelError(string.Empty, "Ky përdorues ekziston tashmë në sistem.");
                    return Page();
                }

                // Kontrollo nëse ka një kërkesë në pritje
                var kerkesaEkzistuese = await _depojaKerkesave.MerrSipasEmail(Input.Email);
                if (kerkesaEkzistuese != null)
                {
                    if (kerkesaEkzistuese.Statusi == StatusiKerkeses.NePritje)
                    {
                        ModelState.AddModelError(string.Empty, "Keni tashmë një kërkesë në pritje. Ju lutem prisni që administratori ta shqyrtojë.");
                        return Page();
                    }
                    else if (kerkesaEkzistuese.Statusi == StatusiKerkeses.Refuzuar)
                    {
                        ModelState.AddModelError(string.Empty, $"Kërkesa juaj është refuzuar më parë. Arsyeja: {kerkesaEkzistuese.ArsetimiRefuzimit}");
                        return Page();
                    }
                }

                // Verifiko në Active Directory - përdor password dummy për verifikim
                var (success, userInfo) = await _adSherbimi.AutontifikoNeActiveDirectory(Input.Email, "DummyPasswordForVerification");
                
                // Nëse AD është i çaktivizuar, success do të jetë true me userInfo të gjeneruar automatikisht
                if (userInfo == null)
                {
                    ModelState.AddModelError(string.Empty, "Nuk u arrit të komunikohet me Active Directory. Ju lutem kontaktoni administratorin.");
                    _logger.LogWarning("Tentativë regjistrimi dështoi: {Email} - problem me AD", Input.Email);
                    return Page();
                }

                // Krijo kërkesën për regjistrim
                var kerkese = new KerkeseRegjistrim
                {
                    Email = userInfo.Email ?? Input.Email,
                    EmriPlote = userInfo.DisplayName,
                    Departamenti = userInfo.Department,
                    Pozicioni = userInfo.Title,
                    ADUsername = userInfo.SamAccountName,
                    EshteVerifikuarNeAD = true,
                    Statusi = StatusiKerkeses.NePritje,
                    DataKerkeses = DateTime.UtcNow
                };

                await _depojaKerkesave.Shto(kerkese);

                _logger.LogInformation("Kërkesë e re regjistrimi nga: {Email}, Email AD: {ADEmail}", Input.Email, userInfo.Email);

                TempData["Sukses"] = "Kërkesa juaj për regjistrim është dërguar me sukses! Administratori do ta shqyrtojë dhe ju do të njoftoheni.";
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
