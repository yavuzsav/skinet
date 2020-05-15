namespace Core.Entities
{
    public class Photo : BaseEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}