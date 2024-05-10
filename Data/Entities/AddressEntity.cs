using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class AddressEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string AddressType { get; set; } = null!;

        public string AddressLine { get; set; } = null!;

        public string? AddressLine_2 { get; set; }

        public string PostalCode { get; set; } = null!;

        public string City { get; set; } = null!;
    }
}
