using AndreyevInterview.Tests.Helpers;

namespace AndreyevInterview.Tests;

public class ClientsControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly AIDbContext _dbContext;
    private bool IsDbSeeded = false;

    public ClientsControllerTests(CustomWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClientWithBaseAddress("http://localhost:5000");
        _dbContext = factory.Services.GetRequiredService<AIDbContext>();
        _dbContext = factory.Services.GetRequiredService<AIDbContext>();
        Task.Run(async () => await Setup());
    }

    private async Task Setup()
    {
        if (IsDbSeeded)
        {
            return;
        }

        await _dbContext.Clients.AddAsync(new ClientEntity { Name = "Conrad", Email = "conradvdyk@gmail.com", Address = "14 Retief road, Despatch" });
        await _dbContext.Clients.AddAsync(new ClientEntity { Name = "Jean", Email = "jean@gmail.com", Address = "15 Retief road, Despatch" });
        await _dbContext.Clients.AddAsync(new ClientEntity { Name = "Michelle", Email = "michelle@gmail.com", Address = "16 Retief road, Despatch" });
        await _dbContext.SaveChangesAsync();


        var listOfInvoices = new List<InvoiceEntity>
            {
                new InvoiceEntity { ClientId = 1, Discount = 1, Description = "Invoice 1" },
                new InvoiceEntity { ClientId = 2, Discount = 2, Description = "Invoice 2" },
                new InvoiceEntity { ClientId = 3, Discount = 3, Description = "Invoice 3" }
            };
        await _dbContext.Invoices.AddRangeAsync(listOfInvoices);
        await _dbContext.SaveChangesAsync();

        var listOfLineItems = new List<LineItemEntity>
            {
               new LineItemEntity { InvoiceId = 1, Quantity = 1,  Cost = 120.5M, Description = "Item 1", isBillable = true, Invoice = listOfInvoices[0] },
               new LineItemEntity { InvoiceId = 1, Quantity = 2,  Cost = 210.2M, Description = "Item 2", isBillable = true, Invoice = listOfInvoices[0] },

               new LineItemEntity { InvoiceId = 2, Quantity = 3,  Cost = 300.3M, Description = "Item 3", isBillable = false, Invoice = listOfInvoices[1] },
               new LineItemEntity { InvoiceId = 2, Quantity = 4,  Cost = 400.4M, Description = "Item 4", isBillable = false, Invoice = listOfInvoices[1] },

               new LineItemEntity { InvoiceId = 3, Quantity = 5,  Cost = 500.5M, Description = "Item 5", isBillable = true, Invoice = listOfInvoices[2] },
               new LineItemEntity { InvoiceId = 3, Quantity = 6,  Cost = 600.6M, Description = "Item 6", isBillable = true, Invoice = listOfInvoices[2] },
            };
        await _dbContext.LineItemEntitys.AddRangeAsync(listOfLineItems);
        await _dbContext.SaveChangesAsync();

        IsDbSeeded = true;
    }

    /// <summary>
    /// This test ensures that GET request to "/Clients" returns status code OK and a list of Client objects.
    /// </summary>
    [Fact]
    public async Task GetClients_ShouldReturnOkAndClients()
    {
        var response = await _client.GetAsync("/Clients");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var clients = await response.Content.ReadFromJsonAsync<List<ClientEntity>>();
        clients.Should().NotBeNull();
    }

    /// <summary>
    /// This test verifies that a correctly formed POST request to "/Clients" microservice correctly creates a client and returns status code OK.
    /// </summary>
    [Fact]
    public async Task CreateClient_ShouldReturnOk()
    {
        var newClient = new ClientEntity
        {
            Name = "Jeandre",
            Email = "jeandrevdyk@gmail.com"
        };

        var response = await _client.PostAsJsonAsync("/Clients", newClient);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// This test checks that an improperly formed POST request to "/Clients" returns a BadRequest status code and specific validation errors.
    /// </summary>
    [Fact]
    public async Task CreateClient_ShouldReturnBadRequest()
    {
        var newClient = new ClientEntity
        {
            Name = "J",
            Email = "testclient"
        };

        var response = await _client.PostAsJsonAsync("/Clients", newClient);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var validationErrorsResponse = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
        validationErrorsResponse?.Errors.Count.Should().Be(2);
        validationErrorsResponse?.Errors.Keys.Should().Contain("Name");
        validationErrorsResponse?.Errors.Keys.Should().Contain("Email");

    }

    /// <summary>
    /// This test ensures that a valid PUT request to update an existing client's information on "/Clients/{clientId}" returns OK status code.
    /// </summary>
    [Fact]
    public async Task UpdateClient_ShouldReturnOk()
    {
        var existingClient = new ClientEntity
        {
            Id = 1,
            Name = "Jeandre",
            Email = "testclient@gmail.com",
            Address = "39 Retief street, Retief, Despatch"
        };

        var response = await _client.PutAsJsonAsync($"/Clients/{existingClient.Id}", existingClient);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// This test verifies that sending a DELETE request to "/Clients/{clientId}" deletes the specified client and returns OK status code.
    /// </summary>
    [Fact]
    public async Task DeleteClient_ShouldReturnNoContent()
    {
        int clientIdToDelete = 1;

        var response = await _client.DeleteAsync($"/Clients/{clientIdToDelete}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
