namespace Headlines.DTO.Entities
{
    public sealed class UserUpvotesDTO
    {
        public long Id { get; set; }
        public string UserToken { get; set; }
        public string Json { get; set; }
    }
}