using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using KosovaDoganaModerne.Modelet.Entitetet;
using KosovaDoganaModerne.Sherbime;

namespace KosovaDoganaModerne.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Perdoruesi> _signInManager;
        private readonly UserManager<Perdoruesi> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly SherbimetActiveDirectory _adSherbimi;

        public LoginModel(
            SignInManager<Perdoruesi> signInManager,
            UserManager<Perdoruesi> userManager,
            ILogger<LoginModel> logger,
            SherbimetActiveDirectory adSherbimi)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _adSherbimi = adSherbimi;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username është i detyrueshëm.")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Fjalëkalimi është i detyrueshëm.")]
            [DataType(DataType.Password)]
            [Display(Name = "Fjalëkalimi")]
            public string Password { get; set; }

            [Display(Name = "Më mbaj mend")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // HAPI 1: Provo autentifikimin në Active Directory së pari
                var (adAuthSuccess, adUserInfo) = await _adSherbimi.AutontifikoNeActiveDirectory(Input.Username, Input.Password);

                if (!adAuthSuccess || adUserInfo == null)
                {
                    ModelState.AddModelError(string.Empty, "Username ose fjalëkalimi i Active Directory është i pasaktë.");
                    _logger.LogWarning("Tentativë hyrje dështoi: Kredencialet e AD janë të pasakta për {Username}", Input.Username);
                    return Page();
                }

                // HAPI 2: Autentifikimi në AD është i suksesshëm, tani kontrollo nëse përdoruesi ekziston në sistemin lokal
                var user = await _userManager.FindByNameAsync(Input.Username);

                // HAPI 3: Nëse përdoruesi nuk ekziston, krijon automatikisht bazuar në të dhënat e AD
                if (user == null)
                {
                    _logger.LogInformation("Përdoruesi {Username} nuk ekziston në sistem, po krijohet bazuar në të dhënat e AD...", Input.Username);
                    
                    // Krijo përdoruesin e ri bazuar në të dhënat e AD
                    user = new Perdoruesi
                    {
                        UserName = adUserInfo.SamAccountName ?? Input.Username,
                        Email = adUserInfo.Email ?? $"{Input.Username}@dogana.rks",
                        EmriPlote = adUserInfo.DisplayName ?? $"{adUserInfo.FirstName} {adUserInfo.LastName}",
                        Departamenti = adUserInfo.Department ?? "N/A",
                        Pozicioni = adUserInfo.Title ?? "N/A",
                        KodiZyrtarit = adUserInfo.SamAccountName ?? Input.Username,
                        EmailConfirmed = true,
                        EshteAktiv = true,
                        DataKrijimit = DateTime.UtcNow
                    };

                    // Krijon përdoruesin pa password (sepse AD menaxhon passwords)
                    var createResult = await _userManager.CreateAsync(user);
                    
                    if (createResult.Succeeded)
                    {
                        // Shto rolin default "Zyrtar"
                        await _userManager.AddToRoleAsync(user, RoletSistemit.Zyrtar);
                        _logger.LogInformation("Përdoruesi i ri u krijua automatikisht nga AD: {Username}", user.UserName);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Gabim gjatë krijimit të llogarisë. Ju lutem kontaktoni administratorin.");
                        _logger.LogError("Dështoi krijimi i përdoruesit nga AD: {Username}. Errors: {Errors}", 
                            Input.Username, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        return Page();
                    }
                }
                else
                {
                    // HAPI 3b: Nëse përdoruesi ekziston, përditëso të dhënat nga AD
                    bool needsUpdate = false;
                    
                    if (user.EmriPlote != adUserInfo.DisplayName && !string.IsNullOrEmpty(adUserInfo.DisplayName))
                    {
                        user.EmriPlote = adUserInfo.DisplayName;
                        needsUpdate = true;
                    }
                    
                    if (user.Departamenti != adUserInfo.Department && !string.IsNullOrEmpty(adUserInfo.Department))
                    {
                        user.Departamenti = adUserInfo.Department;
                        needsUpdate = true;
                    }
                    
                    if (user.Pozicioni != adUserInfo.Title && !string.IsNullOrEmpty(adUserInfo.Title))
                    {
                        user.Pozicioni = adUserInfo.Title;
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        await _userManager.UpdateAsync(user);
                        _logger.LogInformation("Të dhënat e përdoruesit u përditësuan nga AD: {Username}", user.UserName);
                    }
                }

                // HAPI 4: Kontrollo nëse llogaria është aktive
                if (!user.EshteAktiv)
                {
                    ModelState.AddModelError(string.Empty, "Llogaria juaj është çaktivizuar. Ju lutem kontaktoni administratorin.");
                    _logger.LogWarning("Tentativë hyrje dështoi: Llogaria {Username} është çaktivizuar", user.UserName);
                    return Page();
                }

                // HAPI 5: Hyn në sistem
                await _signInManager.SignInAsync(user, Input.RememberMe);
                
                // Përditëso datën e hyrjes së fundit
                user.HyrjaEFundit = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Përdoruesi {Username} hyri me sukses përmes Active Directory", user.UserName);
                return LocalRedirect(returnUrl);
            }

            return Page();
        }
    }
}
