namespace WorkflowCore.Persistence.EntityFramework
{
    public class MigrationMetaInfo
    {
        public static string TableNamePrefix = "wfcps_";
        public static string MigrationTableName = TableNamePrefix + "_EFMigrationsHistory";

        public static string DbSchema;

        public MigrationMetaInfo(string schema)
        {
            DbSchema = schema;
        }
    }
}