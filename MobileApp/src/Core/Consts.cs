using System;
namespace Core
{
    public static class Consts
    {

#if STAGING
        public const string SiteUrl = "https://yarratrams.sharepoint.com/sites/uat_staffapp/";
        public const string AppUrl = "https://yarratrams.sharepoint.com/sites/uat_staffapp/mobile/Pages/app.aspx#";
        public const string TenancyId = "199b72ab-1cf2-4095-a988-6c4f2e9fbea2";
        public const string CustomSignInPage = null;//"https://sso.yarratrams.com.au/adfs/ls";
        public const string AppCentreSecret = "android=6c5fe3db-559c-4431-876e-8bef4a4cea9c;ios=559876ec-a902-4beb-a391-252c2ea20a55;";
        public const bool ForceClearCookiesAfterUnlock = false;//This setting will clear the cookies and reset the session after the unlock.  This is used for debug only. In production and staging this should be false.
        public const string RegistrationFunctionUrl = "https://ding-apps-tst-yt-fad-ase-funapp-registry.azurewebsites.net/api/ProcessRegistration?code=FGKDe1Erht98s1Jjxg6Pl5zKH66T2eNcDlM2gOpNoIutZI7PqJEjFA==";
        public const string AzureADClientId = "963d117a-dece-44cf-bebf-84421a04abb0";
        public const string AndroidAuthSignatureHash = "u1IJo8OmLPTgTqUE6rf/8ts/rdY="; // Set on App Registration in Azure AD

#elif PROD
        public const string SiteUrl = "https://yarratrams.sharepoint.com/sites/staffapp/";
        public const string AppUrl = "https://yarratrams.sharepoint.com/sites/staffapp/mobile/Pages/app.aspx#";
        public const string TenancyId = "199b72ab-1cf2-4095-a988-6c4f2e9fbea2";
        public const string CustomSignInPage = "https://sso.yarratrams.com.au/adfs/ls";
        public const string AppCentreSecret = "android=d95a6803-6622-4bec-8af7-f4fe753f80f4;ios=511586dc-fcc0-467a-ad6c-54af291137df;";
        public const bool ForceClearCookiesAfterUnlock = false;//This setting will clear the cookies and reset the session after the unlock.  This is used for debug only. In production and staging this should be false.
        public const string RegistrationFunctionUrl = "https://ding-apps-prd-yt-3s6-ase-funapp-registry.azurewebsites.net/api/ProcessRegistration?code=M3pHgyPd9YTBxQ9AnW12B6Y8Hd8gL3gj9MIMRjYqMVYWPhI1DWqCvQ==";
        public const string AzureADClientId = "18aac706-2a5c-486a-a8c6-bd27c5e47f6f";
        public const string AndroidAuthSignatureHash = "2jmj7l5rSw0yVb/vlWAYkK/YBwk="; // Set on App Registration in Azure AD

#elif DEV
        public const string SiteUrl = "https://jchmediagroup.sharepoint.com/sites/ytmobile-dev1/";
        public const string AppUrl = "https://jchmediagroup.sharepoint.com/sites/ytmobile-dev1/mobile/Pages/app.aspx#";
        public const string TenancyId = "e697e4dd-5b97-4ea2-aeb9-ebb3955084c2";
        public const string CustomSignInPage = null;
        public const string AppCentreSecret = "android=6c5fe3db-559c-4431-876e-8bef4a4cea9c;ios=559876ec-a902-4beb-a391-252c2ea20a55;";
        public const bool ForceClearCookiesAfterUnlock = false;//This setting will clear the cookies and reset the session after the unlock.  This is used for debug only.  In production and staging this should be false.
        public const string RegistrationFunctionUrl = "https://dingregistrationdev.azurewebsites.net/api/ProcessRegistration?code=4GHeHtysXfBpqCqnhS57KBNZE2ultM5jT/90wC/drBGeMmVpZUQoLA==";
        public const string AzureADClientId = "db0ef54c-4842-471d-9e99-d6f205b08319";
        public const string AndroidAuthSignatureHash = "xVxKrShBURn2GgE27JPTQLNrbv8="; // Set on App Registration in Azure AD

#endif


        public const string LoginNavigationFlow = "Login_Navigation_Flow";
        public const string MainPageNavigationFlow = "Main_Page_Navigation_Flow";
        public const string AppName = "YarraTramsMobileMauiBlazor";
        public const string RosterListName = "Tram Driver Roster";
        public const string RosterConfirmationListName = "TRAM Driver Roster Confirmations";
        public const string UserRoleTramDriver = "Driver - Tram";
        public const string RosterSubsite = "Roster";
        public const string IsInstalledFirstTimeKey = "IsInstalledFirstTime";
        public const string UserProfileKey = "UserProfile";
        public const string LastPasswordSetKey = "LastPasswordSet";
        public const string TempNewPasswordKey = "TempNewPassword";
        public const string RouteDataKey = "RouteData";
        public const string RosterDataKey = "RosterData";
        public const string ConfigDataKey = "MobileAppConfig";
        public const string RosterDataInProgressKey = "RosterDataInProgress";
        public const string DefaultDomain = "yarratrams.com.au";
        public const string PasswordResetUrl = "https://passwordreset.microsoftonline.com/";
        public const string PasswordChangeUrl = "https://account.activedirectory.windowsazure.com/ChangePassword.aspx?BrandContextID=O365&ruO365=";
        public const string SuccessfulPasswordChangeUrl = "https://portal.office.com/ChangePassword.aspx?ReturnCode=0&ruO365=";
        public const string HelpPageUrl = "Pages/Help.aspx?isTrimmedDisplay=1";
        public const string ToolsUrl = AppUrl + "/my-tools";
        public const string NewsUrl = AppUrl + "/news?web=1";
        public const string NewsSiteUrl = SiteUrl + "/news";
        public const string AlertsNotices = AppUrl + "/alerts/archive/page-next";
        public const string ArchiveNewsUrl = AppUrl + "/news/archive/page-next";
        public const string ArchiveWireUrl = AppUrl + "/wire/archive/page-next";
        public const string AlertsUrl = AppUrl + "/alerts/archive/page-next";
        public const string StaffBenefitsUrl = AppUrl + "/my-tools/page-next/staff-benefits";
        public const string PayRollBaseUrl = "https://performancemanager10.successfactors.com";
        public const string VacanciesBaseUrl = "http://jobs.yarratrams.com.au";
        public const string LockedAccountMessage = "Your account has been locked. Contact your support person to unlock it, then try again.";
        public const string InvalidUserNameOrPasswordMessage = "Your account or password is incorrect. If you don't remember your password you can reset it using the 'Forgot Login' link";
        public const string LastPasswordPromptCheck = "LastPasswordPromptCheck";



    }
}
