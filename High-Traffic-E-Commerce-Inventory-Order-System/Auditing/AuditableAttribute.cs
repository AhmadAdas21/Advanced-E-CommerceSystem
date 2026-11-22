namespace High_Traffic_E_Commerce_Inventory_Order_System.Auditing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AuditableAttribute:Attribute
    {
        public string Operation { get; }

        public AuditableAttribute(string operation)
        {
            Operation = operation;
        }
    }
}
