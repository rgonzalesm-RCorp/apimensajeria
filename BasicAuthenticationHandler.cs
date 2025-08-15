using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    string usernameBA = "";
    string passwordBA = "";

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        usernameBA = AppSettings.GetSetting("BasicAuth:username");
        passwordBA = AppSettings.GetSetting("BasicAuth:password");
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        LogHelper.GuardarLogTransaccion(logTransaccionId.ToString(), "BasicAuthenticationHandler.cs", "Inicio BasicAuth: HandleAuthenticateAsync()", "");

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.NoResult();
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            if (username != usernameBA || password != passwordBA)
            {
                LogHelper.GuardarLogTransaccion(logTransaccionId.ToString(), "BasicAuthenticationHandler.cs", "Error Inicio de Login: HandleAuthenticateAsync()", "");
                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            LogHelper.GuardarLogTransaccion(logTransaccionId.ToString(), "BasicAuthenticationHandler.cs", "Error Inicio de Login: HandleAuthenticateAsync()", ex.Message.ToString());
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }

    // Este es el método que sobreescribe el 401 challenge para que el JSON se devuelva SIEMPRE
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        long logTransaccionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = JsonSerializer.Serialize(new
        {
            State = "003",
            Message = "Credenciales incorrectas",
            Data = new { Id = $@"E-{logTransaccionId.ToString()}" }
        });

        await Response.WriteAsync(result);
    }
}
