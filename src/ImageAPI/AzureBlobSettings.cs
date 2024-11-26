namespace ImageAPI
{
    public record AzureBlobSettings
    {
        public string AzureBlobConnectionString { get; set; }
        public string AzureBlobContainerName { get; set; }
    }
}
