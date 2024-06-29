
using CRET.Repository.Interface;

namespace CRET.Web.Middleware
{
    // Assume you have a UserService that interacts with your database
    // This is a simple example; you should implement appropriate services and error handling

    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private IUserRepository _userRepository;


        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepository, ILogger<AuthorizationMiddleware> logger, IConfiguration configuration)
        {
            // Check if the request URL contains "api"
            if (!context.Request.Path.ToString().Contains("api"))
            {
                // If the URL doesn't contain "api", proceed with the request
                await _next(context);
                return;
            }
            _userRepository = userRepository;

            List<string> superAdmins = configuration.GetSection("SuperAdmins").Get<List<string>>();

            // Extract User Email from the request header
            if (!context.Request.Headers.TryGetValue("user-email", out var userEmailHeader))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            string userEmail = userEmailHeader.ToString();
            // Check if the User Email exists in the configured SuperAdmins
            if (superAdmins.Contains(userEmail))
            {
                // Continue processing the request
                await _next(context);
                return;
            }

            // If the email is not found in SuperAdmins, check for User ID
            if (!context.Request.Headers.TryGetValue("user-id", out var userIdHeader))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            string userId = userIdHeader.ToString();

            // Check if the UserId exists in the database
            var res = _userRepository.GetUserIdList();
            if (res == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            bool userExists = CheckIfIdExists(res.UserIdList, userId);

            if (!userExists)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // Continue processing the request
            await _next(context);
        }

        public bool CheckIfIdExists(string idString, string searchId)
        {
            string[] ids = idString?.Split(',');

            foreach (string id in ids)
            {
                if (id.Trim() == searchId)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // Extension method to add the middleware to the HTTP request pipeline
    public static class AuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }



}
