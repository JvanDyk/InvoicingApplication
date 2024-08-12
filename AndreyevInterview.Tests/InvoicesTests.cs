namespace AndreyevInterview.Tests;

public class InvoicesTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly AIDbContext _dbContext;
    private bool IsDbSeeded = false;

    public InvoicesTests(CustomWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClientWithBaseAddress("http://localhost:5000");
        _dbContext = factory.Services.GetRequiredService<AIDbContext>();
        Setup();
    }

    private void Setup()
    {
        if (IsDbSeeded)
        {
            return;
        }

        _dbContext.Clients.Add(new ClientEntity { Name = "Conrad", Email = "conradvdyk@gmail.com", Address = "14 Retief road, Despatch" });
         _dbContext.Clients.Add(new ClientEntity { Name = "Jean", Email = "jean@gmail.com", Address = "15 Retief road, Despatch" });
         _dbContext.Clients.Add(new ClientEntity { Name = "Michelle", Email = "michelle@gmail.com", Address = "16 Retief road, Despatch" });
         _dbContext.SaveChanges();


        var listOfInvoices = new List<InvoiceEntity>
        {
            new InvoiceEntity { ClientId = 1, Discount = 1, Description = "Invoice 1" },
            new InvoiceEntity { ClientId = 2, Discount = 2, Description = "Invoice 2" },
            new InvoiceEntity { ClientId = 3, Discount = 3, Description = "Invoice 3" }
        };
         _dbContext.Invoices.AddRange(listOfInvoices);
         _dbContext.SaveChanges();

        var listOfLineItems = new List<LineItemEntity>
        {
           new LineItemEntity { InvoiceId = 1, Quantity = 1,  Cost = 120.5M, Description = "Item 1", isBillable = true, Invoice = listOfInvoices[0] },
           new LineItemEntity { InvoiceId = 1, Quantity = 2,  Cost = 210.2M, Description = "Item 2", isBillable = true, Invoice = listOfInvoices[0] },

           new LineItemEntity { InvoiceId = 2, Quantity = 3,  Cost = 300.3M, Description = "Item 3", isBillable = false, Invoice = listOfInvoices[1] },
           new LineItemEntity { InvoiceId = 2, Quantity = 4,  Cost = 400.4M, Description = "Item 4", isBillable = false, Invoice = listOfInvoices[1] },

           new LineItemEntity { InvoiceId = 3, Quantity = 5,  Cost = 500.5M, Description = "Item 5", isBillable = true, Invoice = listOfInvoices[2] },
           new LineItemEntity { InvoiceId = 3, Quantity = 6,  Cost = 600.6M, Description = "Item 6", isBillable = true, Invoice = listOfInvoices[2] },
        };
         _dbContext.LineItemEntitys.AddRange(listOfLineItems);
         _dbContext.SaveChanges();

        IsDbSeeded = true;
    }

    [Fact]
    public async Task GetInvoicesAsync_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/invoices");

        var invoiceModel = await response.Content.ReadFromJsonAsync<InvoiceModel>();
        invoiceModel.Should().NotBeNull();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetInvoiceHistoryAsync_ShouldReturnOk_WhenInvoiceExists()
    {
        var validId = _dbContext.Invoices.FirstOrDefault()?.Id;
        var response = await _client.GetAsync($"/invoices/history/{validId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetInvoiceHistoryAsync_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
    {
        var response = await _client.GetAsync("/invoices/history/0");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetInvoiceLineItems_ShouldReturnOk_WhenInvoiceExists()
    {
        var validId = 1;

        var response = await _client.GetAsync($"/invoices/{validId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetInvoiceLineItems_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
    {
        var response = await _client.GetAsync("/invoices/0");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateInvoiceAsync_ShouldReturnOk()
    {
        var input = new InvoiceInput 
        { 
            ClienId = 1,
            Description = "Rental for De Zicht estate",
        };
        var response = await _client.PostAsJsonAsync("/invoices", input);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ShouldReturnOk_WhenInvoiceExists()
    {

        var validId = _dbContext.Invoices.FirstOrDefaultAsync().Id;

        var response = await _client.DeleteAsync($"/invoices/{validId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
    {
        var response = await _client.DeleteAsync("/invoices/0");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task AddDiscountAsync_ShouldReturnOk_WhenInvoiceExists()
    {
        var validId = _dbContext.Invoices.FirstOrDefaultAsync().Id;

        var response = await _client.PutAsync($"/invoices/discount/{validId}?discount=10", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddDiscountAsync_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
    {
        var response = await _client.PutAsync("/invoices/discount/0?discount=10", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task AddLineItemEntityToInvoiceAsync_ShouldReturnOk()
    {
        var input = new LineItemInput
        {
            Id = 1,
            Description = "Item 1",
            Quantity = 5,
            Cost = 100.5M,
            isBillable = true
        };
        var validId = 1;
        var response = await _client.PostAsJsonAsync($"/invoices/{validId}", input);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBillableAsync_ShouldReturnOk()
    {
        var input = new LineItemBillable
        {
            InvoiceId = 1,
            LineItemId = 1,
            isBillable = false
        };
        var response = await _client.PutAsJsonAsync("/invoices/Update", input);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLineItemEntityAsync_ShouldReturnNotFound_WhenLineItemEntityDoesNotExist()
    {
        var response = await _client.DeleteAsync("/invoices/LineItemEntity/0");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLineItemEntityAsync_ShouldReturnOk_WhenLineItemEntityExists()
    {
        var validId = _dbContext.LineItemEntitys.FirstOrDefault()?.Id;

        var response = await _client.DeleteAsync($"/invoices/LineItemEntity/{validId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
