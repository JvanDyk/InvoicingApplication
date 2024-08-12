public class InvoicesService : IInvoicesService
{
    private readonly AIDbContext _context;

    public InvoicesService(AIDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceModel> GetInvoicesAsync()
    {
        List<Invoices> Invoices = new List<Invoices>();
        var invoices = await _context.Invoices.ToListAsync();

        foreach (InvoiceEntity _invoice in invoices)
        {
            var LineItemEntitys = _context.LineItemEntitys.AsEnumerable().Where(x => x.InvoiceId == _invoice.Id);
            Invoices invoice = new Invoices
            {
                Id = _invoice.Id,
                Description = _invoice.Description,
                Discount = _invoice.Discount,
                TotalBillableValue = LineItemEntitys.Count() > 0 ? LineItemEntitys.Where(x => x.isBillable).Sum(x => x.Cost) : 0,
                TotalNumberLineItems = LineItemEntitys.Count(),
                TotalValue = LineItemEntitys.Count() > 0 ? LineItemEntitys.Sum(x => x.Cost) : 0
            };

            Invoices.Add(invoice);
        }

        return new InvoiceModel
        {
            Invoices = Invoices
        };
    }

    public async Task<InvoiceHistory> GetInvoiceHistoryAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            throw new NotFoundException($"Invoice History Entity Record not found for {id}");
        }

        var LineItemEntitys = (await _context.LineItemEntitys.ToListAsync()).Where(x => x.InvoiceId == invoice.Id);
        var invoiceHistory = new InvoiceHistory
        {
            Id = invoice.Id,
            Description = invoice.Description,
            TotalBillableValue = LineItemEntitys.Count() > 0 ? LineItemEntitys.Where(x => x.isBillable).Sum(x => x.Cost) : 0,
            TotalNumberLineItems = LineItemEntitys.Count(),
            TotalValue = LineItemEntitys.Count() > 0 ? LineItemEntitys.Sum(x => x.Cost) : 0,
            Discount = invoice.Discount,
            LogMessages = _context.InvoicesHistory.AsEnumerable().Where(iHistory => iHistory.InvoiceId == id).Select(i => new InvoiceHistoryMessage { Message = i.LogMessage, CreatedOn = i.CreatedOn })
        };

        return invoiceHistory;
    }

    public async Task<LineItemModel> GetInvoiceLineItemsAsync(int id)
    {
        var billableInvoices = (await _context.LineItemEntitys.ToListAsync())
            .Where(x => x.InvoiceId == id && x.isBillable)
            .GroupBy(r => r.InvoiceId)
            .Select(a => new
            {
                TotalBillableValue = a.Sum(r => r.Cost)
            }).FirstOrDefault();

        var totalInvoices = (await _context.LineItemEntitys.ToListAsync())
            .Where(x => x.InvoiceId == id)
            .GroupBy(r => r.InvoiceId)
            .Select(a => new
            {
                GrandTotal = a.Sum(r => r.Cost)
            }).FirstOrDefault();

        var discount = (await _context.Invoices.FindAsync(id)).Discount;
        LineItemModel LineItemEntityModel = new LineItemModel
        {
            LineItem = await _context.LineItemEntitys.Where(x => x.InvoiceId == id).ToListAsync(),
            GrandTotal = totalInvoices == null ? 0 : totalInvoices.GrandTotal,
            Discount = discount,
            TotalBillableValue = billableInvoices == null ? 0 : billableInvoices.TotalBillableValue
        };

        return LineItemEntityModel;
    }

    public async Task<InvoiceEntity> CreateInvoiceAsync(InvoiceInput input)
    {
        var invoice = new InvoiceEntity
        {
            Description = input.Description,
            ClientId = input.ClienId
        };
        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            throw new NotFoundException($"Invoice Entity Record not found for {id}");
        }

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<InvoiceEntity> AddDiscountAsync(int invoiceId, int discount)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null)
        {
            throw new NotFoundException($"Record not found for {invoiceId}");
        }

        invoice.Discount = discount;
        await AddInvoiceHistoryAsync(invoiceId, $"Discount: {discount}% has been added to the invoice");
        _context.Update(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    public async Task<LineItemEntity> AddLineItemEntityToInvoiceAsync(int id, LineItemInput input)
    {
        var LineItemEntity = new LineItemEntity
        {
            InvoiceId = id,
            Description = input.Description,
            Quantity = input.Quantity,
            Cost = input.Cost,
            isBillable = input.isBillable
        };

        if (input.Id == 0)
        {
            await AddLineItemEntityAsync(LineItemEntity);
        }
        else
        {
            await UpdateLineItemEntityAsync(LineItemEntity);
        }

        return LineItemEntity;
    }

    public async Task<bool> UpdateBillableAsync(LineItemBillable lineItemEntity)
    {
        return await UpdateLineItemEntityAsync(new LineItemEntity
        {
            InvoiceId = lineItemEntity.InvoiceId,
            isBillable = lineItemEntity.isBillable,
            Id = lineItemEntity.LineItemId
        });
    }

    public async Task<bool> DeleteLineItemEntityAsync(int id)
    {
        var lineItemEntity = await _context.LineItemEntitys.FindAsync(id);
        if (lineItemEntity == null)
        {
            throw new NotFoundException($"Line Item Entity Record not found for {id}");
        }

        _context.Remove(lineItemEntity);
        await _context.SaveChangesAsync();
        await AddInvoiceHistoryAsync(lineItemEntity.InvoiceId, $"{lineItemEntity.Description} has been removed from the invoice");

        return true;
    }

    private async Task<bool> UpdateLineItemEntityAsync(LineItemEntity lineItemEntity)
    {
        bool done = false;

        await Task.Run(async () =>
        {
            LineItemEntity? dbLineItemEntity = await _context.LineItemEntitys.FindAsync(lineItemEntity.Id);

            if (dbLineItemEntity != null)
            {
                LineItemEntity lt = dbLineItemEntity;
                lt.isBillable = lineItemEntity.isBillable;
                _context.Entry(dbLineItemEntity).CurrentValues.SetValues(lt);

                if (_context.SaveChanges() == 1)
                {
                    done = true;
                    await AddInvoiceHistoryAsync(lineItemEntity.InvoiceId, $"{lineItemEntity.Description} has been updated in the invoice");
                }
            }
        });

        return done;
    }

    private async Task<bool> AddLineItemEntityAsync(LineItemEntity lineItemEntity)
    {
        bool done = false;
        await _context.LineItemEntitys.AddAsync(lineItemEntity);

        if ((await _context.SaveChangesAsync()) == 1)
        {
            done = true;
            await AddInvoiceHistoryAsync(lineItemEntity.InvoiceId, $"{lineItemEntity.Description} has been added in the invoice");
        }
        return done;
    }

    private async Task AddInvoiceHistoryAsync(int invoiceId, string logMessage)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null)
        {
            throw new Exception($"Invoice with Id: {invoiceId} not found");
        }
        var invoiceHistoryEntity = new InvoiceHistoryEntity
        {
            Invoice = invoice,
            LogMessage = logMessage,
            InvoiceId = invoice.Id
        };

        await _context.AddAsync(invoiceHistoryEntity);
        await _context.SaveChangesAsync();
    }
}