namespace High_Traffic_E_Commerce_Inventory_Order_System.Auditing
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }

        public string Operation { get; set; } = string.Empty;

        public DateTimeOffset TimestampUtc { get; set; }

        public string Outcome { get; set; } = string.Empty;

        public int HttpStatusCode { get; set; }
    }
}
