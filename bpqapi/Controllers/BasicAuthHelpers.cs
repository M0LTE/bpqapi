using System.Text;

namespace bpqapi.Controllers;

internal static class BasicAuthHelpers
{
    public static (string User, string Password)? ParseBasicAuthHeader(this HttpContext context)
    {
        context.Request.Headers.TryGetValue("Authorization", out var authHeader);

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return null;
        }

        var parts = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.ToString().Split(' ')[1])).Split(':');
        var user = parts[0];
        var password = parts[1];

        return (user, password);
    }
}