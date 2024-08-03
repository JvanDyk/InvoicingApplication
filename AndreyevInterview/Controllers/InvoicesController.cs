using AndreyevInterview.Shared.Entities;
using AndreyevInterview.Shared.Models;
using AndreyevInterview.Shared.Models.API;

namespace AndreyevInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    #region Constractor
    private readonly AIDbContext _context;

    public InvoicesController(AIDbContext context)
    {
        _context = context;
    }
    #endregion

    #region All Get API's
    [HttpGet]
    public ActionResult<InvoiceModel> GetInvoices()
    {
        List<Invoices> Invoices = new List<Invoices>();
        var invoices = _context.Invoices.ToList();

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

        return Ok(new InvoiceModel
        {
            Invoices = Invoices
        });

    }

    [HttpGet("history/{id}")]
    public async Task<ActionResult<InvoiceHistoryDTO>> GetInvoiceHistory([FromRoute]int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        var LineItemEntitys = _context.LineItemEntitys.AsEnumerable().Where(x => x.InvoiceId == invoice.Id);
        var invoiceHistory = new InvoiceHistoryDTO
        {
            Id = invoice.Id,
            Description = invoice.Description,
            TotalBillableValue = LineItemEntitys.Count() > 0 ? LineItemEntitys.Where(x => x.isBillable).Sum(x => x.Cost) : 0,
            TotalNumberLineItems = LineItemEntitys.Count(),
            TotalValue = LineItemEntitys.Count() > 0 ? LineItemEntitys.Sum(x => x.Cost) : 0,
            Discount = invoice.Discount,
            LogMessages = _context.InvoicesHistory.AsEnumerable().Where(iHistory => iHistory.InvoiceId == id).Select(i => i.LogMessage)
        };


        return Ok(invoiceHistory);
    }

    [HttpGet("{id}")]
    public LineItemModel GetInvoiceLineItemEntitys(int id)
    {
        var billableInvoices = _context.LineItemEntitys.AsEnumerable().Where(x => x.InvoiceId == id && x.isBillable)
              .GroupBy(r => r.InvoiceId)
              .Select(a => new
              {
                  TotalBillableValue = a.Sum(r => r.Cost)
              }).FirstOrDefault();

        var totalInvoices = _context.LineItemEntitys.AsEnumerable().Where(x => x.InvoiceId == id)
              .GroupBy(r => r.InvoiceId)
              .Select(a => new
              {
                  GrandTotal = a.Sum(r => r.Cost)
              }).FirstOrDefault();

        LineItemModel LineItemEntityModel = new LineItemModel
        {
            LineItem = _context.LineItemEntitys.Where(x => x.InvoiceId == id).ToList(),
            GrandTotal = totalInvoices == null ? 0 : totalInvoices.GrandTotal,
            TotalBillableValue = billableInvoices == null ? 0 : billableInvoices.TotalBillableValue
        };

        return LineItemEntityModel;
    }
    #endregion

    [HttpPut]
    public async Task<ActionResult<InvoiceEntity>> CreateInvoiceAsync([FromBody] InvoiceInput input)
    {
        var invoice = new InvoiceEntity();
        invoice.Description = input.Description;
        invoice.ClientId = input.ClienId;
        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();
        return Ok(invoice);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<InvoiceEntity>> DeleteInvoiceAsync([FromRoute] int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("discount/{invoiceId}")]
    public async Task<IActionResult> CreateInvoicDiscounteAsync([FromRoute] int invoiceId, [FromQuery] int discount)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null)
        {
            return NotFound();
        }

        invoice.Discount = discount;
        await AddInvoiceHistoryAsync(invoiceId, $"Discount: {discount}% has been added to the invoice");
        _context.Update(invoice);
        await _context.SaveChangesAsync();
        return Ok(invoice);
    }


    [HttpPost("{id}")]
    public async Task<ActionResult<LineItemEntity>> AddLineItemEntityToInvoice([FromRoute] int id, [FromBody] LineItemInput input)
    {
        var LineItemEntity = new LineItemEntity();

        LineItemEntity.InvoiceId = id;
        LineItemEntity.Description = input.Description;
        LineItemEntity.Quantity = input.Quantity;
        LineItemEntity.Cost = input.Cost;
        LineItemEntity.isBillable = input.isBillable;

        if (input.Id == 0)
        {
            await AddLineItemEntity(LineItemEntity);
        }
        else
        {
            await UpdateLineItemEntity(LineItemEntity);
        }

        await _context.AddAsync(LineItemEntity);
        await _context.SaveChangesAsync();
        return Ok(LineItemEntity);
    }

    [HttpPut("Update")]
    public async Task<bool> UpdateBillable(LineItemBillable lineItemEntity)
    {
        return await UpdateLineItemEntity(new LineItemEntity
        {
            InvoiceId = lineItemEntity.InvoiceId,
            isBillable = lineItemEntity.isBillable,
            Id = lineItemEntity.LineItemId
        });
    }

    [HttpDelete("LineItemEntity/{id}")]
    public async Task<IActionResult> DeleteLineItemEntityAsync([FromRoute] int id)
    {
        var lineItemEntity = await _context.LineItemEntitys.FindAsync(id);
        if (lineItemEntity == null)
        {
            return NotFound();
        }

        _context.Remove(lineItemEntity);
        await _context.SaveChangesAsync();
        await AddInvoiceHistoryAsync(lineItemEntity.InvoiceId, $"{lineItemEntity.Description} has been removed from the invoice");
        return Ok();
    }


    private async Task<bool> UpdateLineItemEntity(LineItemEntity lineItemEntity)
    {
        bool done = false;

        await Task.Run(async () =>
        {
            LineItemEntity dbLineItemEntity = _context.LineItemEntitys.Find(lineItemEntity.Id);

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

    private async Task<bool> AddLineItemEntity(LineItemEntity lineItemEntity)
    {
        bool done = false;
        await Task.Run(async () =>
        {
            _context.LineItemEntitys.Add(lineItemEntity);

            if ((await _context.SaveChangesAsync()) == 1)
            {
                done = true;
                await AddInvoiceHistoryAsync(lineItemEntity.InvoiceId, $"{lineItemEntity.Description} has been added in the invoice");
            }
        });
        return done;
    }

    private async Task AddInvoiceHistoryAsync(int invoiceId, string logMessage)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null)
        {
            throw new Exception($"Invoice with Id: {invoiceId} not found");
        }
        var invoiceHistoryEntity = new InvoiceHistoryEntity();
        invoiceHistoryEntity.Invoice = invoice;
        invoiceHistoryEntity.LogMessage = logMessage;
        invoiceHistoryEntity.InvoiceId = invoice.Id;

        await _context.AddAsync(invoiceHistoryEntity);
        await _context.SaveChangesAsync();
    }
}