using Hangfire.Dashboard;

namespace LibraryManagement.Api.Hangfire;

public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
