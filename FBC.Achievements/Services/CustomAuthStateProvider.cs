using FBC.Achievements.DBModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Radzen;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FBC.Achievements.Services
{
    public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public FakeAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Boş ve yetkisiz bir kullanıcı tanımı (anonim gibi)
            var identity = new ClaimsIdentity(); // boş bırakılıyor
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Context.Response.Redirect(C.NAV.Login);
            return base.HandleChallengeAsync(properties);
        }
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            //Eğer login sayfası için request gelmiş ise 201 döndür yoksa Login sayfasına yönlendir
            if (Context.Request.Path == C.NAV.Login)
            {
                Context.Response.StatusCode = 201;
                return Task.CompletedTask;
            }
            else
            {
                Context.Response.Redirect(C.NAV.Login);
                return Task.CompletedTask;
                //return base.HandleForbiddenAsync(properties);
            }

        }
    }

    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private DBUser? currentUser;
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            if (currentUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, currentUser.UserName),
                    new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, currentUser.UserType.ToString())
                };
                var identity = new ClaimsIdentity(claims, "CustomAuth");
                authState = new AuthenticationState(new ClaimsPrincipal(identity));
            }
            return Task.FromResult(authState);

        }
        private void NotifyUserAuthentication(DBUser user)
        {
            currentUser = user;
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            }, "CustomAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }
        private void NotifyUserLogout()
        {
            currentUser = null;
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public string Login(string username, string password)
        {
            var db = new DB();
            var user = db.Users.FirstOrDefault(u => C.User.UsersThatCanLogin.Contains(u.UserType) && u.UserName == username && u.Password == C.Tools.ToMD5(password));
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Password))
                {
                    NotifyUserAuthentication(user);
                    return String.Empty;
                }
                else
                {
                    return "Yöneticiniz sizin için şifre oluşturmadı. Lütfen yöneticinizle iletişime geçin.";
                }
            }
            else
            {
                return "Geçersiz kullanıcı adı veya şifre.";
            }
        }
        public void Logout()
        {
            NotifyUserLogout();
        }
    }
}
