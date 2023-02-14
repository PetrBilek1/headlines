namespace Headlines.DTO.Entities
{
    public sealed class ObjectDataDTO
    {
        public long Id { get; set; }

        public string Bucket { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
    }
}