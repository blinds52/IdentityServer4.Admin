using System;
using System.Collections.Generic;

namespace IdentityServer4.Admin.Infrastructure
{
    public class AdminOptions
    {
        public string ConnectionString { get; set; }
        public bool AllowLocalLogin { get; set; } = true;
        public bool AllowRememberLogin { get; set; } = true;

        public int RememberMeLoginDuration { get; set; } = 30;

        public bool ShowLogoutPrompt { get; set; } = true;
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        // specify the Windows authentication scheme being used
        public string WindowsAuthenticationSchemeName { get; set; } =
            Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;

        // if user uses windows auth, should we load the groups from windows
        public bool IncludeWindowsGroups { get; set; } = false;

        public string InvalidCredentialsErrorMessage { get; set; } = "用户名或密码错误";

        public bool EnableOfflineAccess { get; set; } = true;

        public string OfflineAccessDisplayName = "Offline Access";

        public string OfflineAccessDescription { get; set; } =
            "Access to your applications and resources, even when you are offline";

        public string MustChooseOneErrorMessage { get; set; } = "You must pick at least one permission";
        public string InvalidSelectionErrorMessage { get; set; } = "Invalid selection";

        public HashSet<string> Group { get; set; }
        public HashSet<string> Title { get; set; }
        public HashSet<string> Level { get; set; }
    }
}