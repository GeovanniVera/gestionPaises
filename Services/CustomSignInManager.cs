using gestionpaises.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace gestionpaises.Services
{
    public class CustomSignInManager : SignInManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomSignInManager(
            UserManager<ApplicationUser> userManager,
            Microsoft.AspNetCore.Http.IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor,
            Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>> logger,
            Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _userManager = userManager;
        }

        public override async Task<Microsoft.AspNetCore.Identity.SignInResult> PasswordSignInAsync(
            string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var resultado = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            if (resultado.Succeeded)
            {
                var usuario = await _userManager.FindByNameAsync(userName);
                if (usuario != null)
                {
                    // Generamos un nuevo identificador de sesión y lo guardamos en la base.
                    string nuevoSessionId = Guid.NewGuid().ToString();
                    usuario.SessionId = nuevoSessionId;
                    await _userManager.UpdateAsync(usuario);

                    // Lo agregamos también como claim dentro de la cookie actual.
                    var principal = await CreateUserPrincipalAsync(usuario);
                    var identity = (System.Security.Claims.ClaimsIdentity)principal.Identity!;
                    identity.AddClaim(new System.Security.Claims.Claim("SessionId", nuevoSessionId));

                    await Context.SignInAsync(
                        Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme,
                        principal,
                        new Microsoft.AspNetCore.Authentication.AuthenticationProperties { IsPersistent = isPersistent });
                }
            }

            return resultado;
        }
    }
}