﻿namespace AndreyevInterview.Shared.Models.Invoices;

public class LineItemInput
{
    public int Id { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal Cost { get; set; }
    public bool isBillable { get; set; }
}

public class LineItemBillable
{ 
    public int InvoiceId { get; set; }
    public int LineItemId { get; set; } 
    public bool isBillable { get; set; }
}
