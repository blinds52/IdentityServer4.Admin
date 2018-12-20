using System;

namespace IdentityServer4.Admin.Infrastructure
{
    public static class AdminConsts
    {
        public const string AdminName = "admin";

        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = true;
        public static bool EnableOfflineAccess = true;
        public static string OfflineAccessDisplayName = "Offline Access";
        public static bool AllowLocalLogin = true;
        // if user uses windows auth, should we load the groups from windows
        public static bool IncludeWindowsGroups = false;
        public static string OfflineAccessDescription =
            "Access to your applications and resources, even when you are offline";

        public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";
        public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
        public static string InvalidCredentialsErrorMessage = "用户名或密码错误";
        // specify the Windows authentication scheme being used
        public static readonly string WindowsAuthenticationSchemeName = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
    }
}