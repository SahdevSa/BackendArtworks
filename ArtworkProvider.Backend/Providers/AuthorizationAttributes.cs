//file = "AuthorizationAttributes.cs"

using System;
using System.Linq;
using System.Net;
using System.Net.Http; // for actionContext.Request.CreateResponse
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;  // for HttpActionContext
using System.Web.Http.Filters;  // for AuthorizationFilterAttribute
using System.Web.Http; // for AuthorizeAttribute
using System.Threading;
using System.Diagnostics;

namespace ArtworkProvider.Backend.Providers
{
    // https://www.asp.net/web-api/overview/security/authentication-filters
    // No authentication was attempted (for this authentication method). ==> Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
    // Authentication was attempted but failed. ==> Set ErrorResult to indicate an error.
    // Authentication was attempted and succeeded. ==> Set Principal to the authenticated user.    

    /// <summary>
    /// </summary>
    public class CustomUserTokenAsApiKeyAuthAttribute : Attribute, IAuthenticationFilter
    {
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            const string tokenHeader = "X-API-KEY";
            if (!request.Headers.Contains(tokenHeader))
            {
                // No authentication was attempted
                Debug.WriteLine($"$$$ ??? Header not found. {request.RequestUri}");
                return;
            }
            else
            {
                var token = request.Headers.GetValues(tokenHeader).FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                    Debug.WriteLine($"$$$ --- Header found, but empty. {request.RequestUri}");
                    context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                    return;
                }

                if (!Guid.TryParse(token, out _))
                {
                    // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                    Debug.WriteLine($"$$$ --- Header found, but not guid. {request.RequestUri}");
                    context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                    return;
                }

                // check whether token is a valid user token
                //var appSettings = new AppSettings();
                //var userStorage = new UserStorage(appSettings); // TBD singleton?
                //bool isValid = userStorage.IsValidToken(token);
                //if (!isValid)
                {
                    // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                    Debug.WriteLine($"$$$ --- Header found, but invalid token! {request.RequestUri}");
                    context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                    return;
                }

                Debug.WriteLine($"$$$ +++ Header found, valid token. {request.RequestUri}");

                // Reset Timer for every call.
                //userStorage.UpdateClientPing(token);

                // create identity
                var identity = new ClaimsIdentity("CustomApiKeyAuth");
                identity.AddClaim(new Claim(MyClaims.USER_TOKEN, token));
                //var group = await userStorage.GetGroup(token);
                //if (group == "admin")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, MyRoles.ADMIN));
                }

                // Authentication was attempted and succeeded. Set Principal to the authenticated user.
                context.Principal = new ClaimsPrincipal(new[] { identity });
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public virtual bool AllowMultiple
        {
            get { return false; }
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ReasonPhrase);

            return response;
        }
    }

    public static class MyClaims
    {
        public const string USER_TOKEN = "Token";
    }

    public static class MyRoles
    {
        public const string ADMIN = "Administrator";
    }

    /// <summary>
    /// Authorize users that have admin rights.
    /// </summary>
    public class AuthorizationAdminAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool succ = DoAuthorize(actionContext);
            if (!succ)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        private bool DoAuthorize(HttpActionContext actionContext)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (!principal.Identity.IsAuthenticated)
            {
                return false;
            }
            return principal.IsInRole(MyRoles.ADMIN);
        }
    }

    /// <summary>
    /// Helper Methods to access the claims inside of an api controller
    /// </summary>
    public static class AuthorizationHelpers
    {
        public static string GetClaimToken(System.Security.Principal.IPrincipal user)
        {
            var gid = GetFirstClaimValue(user, MyClaims.USER_TOKEN);
            return gid;
        }

        public static string GetFirstClaimValue(System.Security.Principal.IPrincipal user, string type)
        {
            var ci = user.Identity as ClaimsIdentity;
            string value = null;
            var theclaim = ci.FindFirst(x => x.Type == type);
            if (theclaim != null)
            {
                value = theclaim.Value;
            }
            return value;
        }

        public static ClaimsPrincipal CreateTestPrincipal(string token)
        {
            var identity = new ClaimsIdentity("CustomApiKeyAuth"); //TBD
            identity.AddClaim(new Claim(MyClaims.USER_TOKEN, token));
            return new ClaimsPrincipal(new[] { identity });
        }

        public static bool RemoveAllClaims(System.Security.Principal.IPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            var claims = identity.Claims.ToList();
            for (var i = claims.Count() - 1; i >= 0; i--)
            {
                identity.RemoveClaim(claims[i]);
            }
            return true;
        }
    }
}