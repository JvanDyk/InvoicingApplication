namespace AndreyevInterview.Shared.Entities
{
    public class InvoiceHistoryEntity
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string LogMessage {  get; set; }
        public DateTime CreatedOn { get; set; }

        public InvoiceEntity Invoice { get; set; }
    }
}
