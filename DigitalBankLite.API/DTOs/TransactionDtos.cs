namespace DigitalBankLite.API.DTOs
{
    public class TransferDto
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class CreateServiceRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

     public class UpdateServiceRequestStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}
