namespace DynamoLocalDemo.Settings
{
    public interface IDynamoSettings
    {
        string AuthorsTable { get; set; }
        string TablePrefix { get; set; }
        bool LocalMode { get; set; }
        string LocalServiceUrl { get; set; }
    }
}