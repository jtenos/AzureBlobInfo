namespace AzureBlobInfo.ApplicationConfiguration
{
    public class StorageAccount
    {
        public string ConnectionString { get; set; } = default!;
        public Container[] Containers { get; set; } = default!;
    }
}
