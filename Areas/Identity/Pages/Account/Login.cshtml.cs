using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmartJobRecommender.Areas.Identity.Pages.Account
{
    // The [AllowAnonymous] attribute ensures this page can be accessed by anyone
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        // Private fields initialized via dependency injection
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // Constructor: ASP.NET Core automatically provides these services
        public LoginModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        // Input Model: Binds the data from the HTML form (Email, Password, RememberMe)
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        // Input Model Class: Defines the fields the user needs to enter
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // Handles GET requests (initial page load)
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // Handles POST requests (form submission) - The core login logic
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // We use returnUrl ??= Url.Content("~/") here, but the custom redirect below overrides it
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This checks the user's credentials against the database.
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    // --- FIX APPLIED: Use RedirectToRoute for MVC Controller action ---
                    // This redirects to the "Analyze" action in the "Home" controller.
                    return RedirectToRoute(new { controller = "Home", action = "Analyze" });
                    // --- END FIX ---
                }

                // Handle various failure states (e.g., Two-Factor Authentication, Lockout)
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // Generic error message for security
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed (validation error), redisplay form
            return Page();
        }
    }
}