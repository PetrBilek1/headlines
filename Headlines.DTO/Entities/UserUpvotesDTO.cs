namespace Headlines.DTO.Entities
{
    public sealed class UserUpvotesDto
    {
        public long Id { get; set; }
        public string UserToken { get; set; }
        public string Json { get; set; }
    }
}