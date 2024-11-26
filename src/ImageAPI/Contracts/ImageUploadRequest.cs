namespace ImageAPI.Contracts
{
    public record ImageUploadRequest
    {
        public string FileName { get; set; }    
        public string? Description { get; set; }
        public IFormFile Image { get; set; }
        public bool AllowImageOverwrite { get; set; }
    }
}
