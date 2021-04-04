namespace DynamoLocalDemo.Settings
{
    public class DynamoSettings : IDynamoSettings
    {
        public string AuthorsTable { get; set; }
        public string TablePrefix { get; set; }
        public bool LocalMode { get; set; }
        public string LocalServiceUrl { get; set; }
    }
}
