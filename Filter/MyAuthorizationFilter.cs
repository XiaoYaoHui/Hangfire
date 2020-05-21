using Hangfire.Dashboard;

namespace Core.Api.App
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContent = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContent.User.Identity.IsAuthenticated;
        }
    }
}
