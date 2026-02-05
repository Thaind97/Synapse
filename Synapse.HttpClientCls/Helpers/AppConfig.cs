using System.Reflection;

namespace Synapse.HttpClientCls.Helpers
{
    public static class AppConfig
    {
        public static string? LogOutputDirectory { get; set; }
        public static DateTime ApplicationStartTime { get; set; }
        public static int DefaultWaitTimeoutPeriod { get; set; }
        public static int NumberOfScriptExecuteParallel { get; set; }
        public static bool UseTotalCpuCoreAsNumberOfParallelScriptsExecution { get; set; }
        public static string? ExecutionEnvironment { get; set; }
        public static string? HostName { get; set; }
        public static string? Protocol { get; set; }
        public static string? GlobalTenantName { get; set; }
        public static string? GlobalTenantUrl => $"{Protocol}://{GlobalTenantName}.{HostName}";
        public static string GetSubdomainUrl(string subdomainName) => $"{Protocol}://{subdomainName}.{HostName}";
        public static string ExecutingFolder { get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); } }
        public static string DataFolder { get { return Path.Combine(AppConfig.ExecutingFolder, "Data"); } }
        public static string PreConditionFolder { get { return Path.Combine(AppConfig.ExecutingFolder, "Preconditions"); } }
        public static bool InDebugMode { get { return System.Diagnostics.Debugger.IsAttached; } }
        public static string EmailSMTPServer { get { return "jcore-mailqa.cloudapp.net"; } }
        public static int EmailSMTPPort { get { return 25; } }
        public static string EmailFromAddress { get { return "admin@nomail.com"; } }
        public static string[] EmailToAddresses { get; set; } = new[] { "admin@nomail.com" };
        public static string DBConnectionString { get; set; } = $"data source=({DBServer});initial catalog=postgres;integrated security=false";
        public static string DBServer { get; set; } = "10.151.1.8";
        public static string ReportServer { get; set; } = "localhost:8080";
        public static bool HasDbLogging { get; set; } = true;
        public static string RunId { get; set; }
        public static string DownloadFolder
        {
            get
            {
                if (!Directory.Exists("DownloadedFiles"))
                {
                    Directory.CreateDirectory("DownloadedFiles");
                }

                return "DownloadedFiles";
            }
        }
        public static bool IsPfEnabledForGlobalTenant { get; set; }
        public static bool IsPfEnabledForLocalTenant { get; set; }
        public static bool IsPfEnabledOnAnyTenant => IsPfEnabledForGlobalTenant || IsPfEnabledForLocalTenant;

        public enum BrowserTypes
        {
            Chrome,
            Firefox,
            InternetExplorer,
            Edge,
            HeadLess
        }

        public static BrowserTypes BrowserType { get; set; }
        public static string ClsVersion { get; set; }
        public static Assembly AppAssembly { get { return Assembly.GetExecutingAssembly(); } }
        public static class CaptureUrlConfiguration
        {
            public static string CaptureDomain { get; set; }
            public static bool IgnoreResource { get; set; }
            public static string[] ExtensionFilterExclusions { get; set; }
        }
    }
}
