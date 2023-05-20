namespace DapperSharing.Models
{
    public class StoreEntity
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public List<ProductEntity> Products { get; set; }
    }
}
