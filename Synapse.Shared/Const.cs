namespace Synapse.Shared
{
    public static class Const
    {
        public const string TENANT_CLAIM_TYPE = "tenant_id";
        public const string TENANT_MASTER = "1b7b8848-9c72-4c34-9926-398c7b475adc";
        public const string PERMISSION_CLAIM_TYPE = "permission";
        public const string USER_CLAIM_TYPE = "sub";

        public const string APPLICATION_FOLDER_NAME = "Synapse";
        public const string DATABASE_FILE_NAME = $"{APPLICATION_FOLDER_NAME}_Database.db3";
        public const string LOG_FOLDER_NAME = "logs";

        public const string LANGUAGE_EN = "en-us";
        public const string LANGUAGE_JP = "ja-jp";

        public const string GOOGLE_LANGUAGE_EN = "en";
        public const string GOOGLE_LANGUAGE_JP = "ja";

        public const string INSTALL_DATE_EMPTY = "-";
        public const string INSTALL_VERSION_EMPTY = "1.0.0.0";
    }
}
