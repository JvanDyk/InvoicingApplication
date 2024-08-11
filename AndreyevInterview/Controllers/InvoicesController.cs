namespace AndreyevInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoicesService _invoiceService;

    public InvoicesController(IInvoicesService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<InvoiceModel>> GetInvoicesAsync()
    {
        var result = await _invoiceService.GetInvoicesAsync();
        return Ok(result);
    }

    [HttpGet("history/{id}")]
    public async Task<ActionResult<InvoiceHistory>> GetInvoiceHistoryAsync([FromRoute] int id)
    {
        var result = await _invoiceService.GetInvoiceHistoryAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<LineItemModel> GetInvoiceLineItems(int id)
    {
        var result = _invoiceService.GetInvoiceLineItems(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceEntity>> CreateInvoiceAsync([FromBody] InvoiceInput input)
    {
        var result = await _invoiceService.CreateInvoiceAsync(input);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInvoiceAsync([FromRoute] int id)
    {
        await _invoiceService.DeleteInvoiceAsync(id);
        return Ok();
    }

    [HttpPut("discount/{invoiceId}")]
    public async Task<IActionResult> AddDiscountAsync([FromRoute] int invoiceId, [FromQuery] int discount)
    {
        var result = await _invoiceService.AddDiscountAsync(invoiceId, discount);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("{id}")]
    public async Task<ActionResult<LineItemEntity>> AddLineItemEntityToInvoiceAsync([FromRoute] int id, [FromBody] LineItemInput input)
    {
        var result = await _invoiceService.AddLineItemEntityToInvoiceAsync(id, input);
        return Ok(result);
    }

    [HttpPut("Update")]
    public async Task<ActionResult<bool>> UpdateBillableAsync([FromBody] LineItemBillable lineItemEntity)
    {
        var result = await _invoiceService.UpdateBillableAsync(lineItemEntity);
        return Ok(result);
    }

    [HttpDelete("LineItemEntity/{id}")]
    public async Task<ActionResult> DeleteLineItemEntityAsync([FromRoute] int id)
    {
        var result = await _invoiceService.DeleteLineItemEntityAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return Ok();
    }
}