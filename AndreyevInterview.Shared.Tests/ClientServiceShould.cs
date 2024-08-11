namespace AndreyevInterview.Shared.Tests;

public class ClienServiceShould
{
    private readonly AIDbContext _context;
    private readonly ClientService _service;

    public ClienServiceShould()
    {
        var options = new DbContextOptionsBuilder<AIDbContext>()
         .UseInMemoryDatabase(databaseName: "TestDb")
         .Options;

        _context = new AIDbContext(options);

        _service = new ClientService(_context);
    }

    /// <summary>
    /// This is a unit test that checks the service's ability to return a list of clients.
    /// It first populates the clients list and then calls the GetClientsAsync method.
    /// It then asserts that at least two clients are returned as expected.
    /// </summary>
    [Fact]
    public async Task ReturnListOfClients()
    {
        // Arrange
        var testClients = new List<ClientEntity>
        {
            new ClientEntity { Name = "Conrad", Email = "conradvdyk@gmail.com", Address = "14 Retief road, Despatch" },
            new ClientEntity { Name = "Jean", Email = "jean@gmail.com", Address = "15 Retief road, Despatch" }
            // Add more clients if desired
        };

        _context.Clients.AddRange(testClients);
        _context.SaveChanges();

        // Act
        var clients = await _service.GetClientsAsync();

        // Assert
        Assert.True(clients.Count() >= 2);
    }

    /// <summary>
    /// This is a unit test that checks the service's ability to create a new client.
    /// It first creates a new client and then calls the CreateClientAsync method.
    /// It then asserts that the new client is indeed created as expected.
    /// </summary>
    [Fact]
    public async Task CreateClient()
    {
        // Arrange
        var client = new ClientEntity { Name = "TestClient", Email = "testclient@gmail.com", Address = "16 Retief road, Despatch" };

        // Act
        await _service.CreateClientAsync(client);

        // Assert
        Assert.NotEmpty(_context.Clients.Where(c => c.Name == "TestClient"));
    }

    // <summary>
    /// This is a unit test that checks the service's ability to update an existing client's details.
    /// It first creates a new client, and then updates the name of this client.
    /// It then asserts that the client's name is updated as expected.
    /// </summary>
    [Fact]
    public async Task UpdateClient()
    {
        // Arrange
        var client = new ClientEntity { Name = "TestClient", Email = "testclient@gmail.com", Address = "16 Retief road, Despatch" };
        await _service.CreateClientAsync(client);
        client.Name = "UpdateClient";

        // Act
        await _service.UpdateClientAsync(client);

        // Assert
        Assert.NotEmpty(_context.Clients.Where(c => c.Name == "UpdateClient"));
    }

    /// <summary>
    /// This is a unit test that checks the service's ability to delete a client.
    /// It first adds a client to the list, and then deletes it.
    /// It then asserts that the deleted client no longer exists in the list.
    /// </summary>
    [Fact]
    public async Task DeleteClient()
    {
        // Arrange
        var client = new ClientEntity { Name = "TestClient", Email = "testclient@gmail.com", Address = "16 Retief road, Despatch" };
        _context.Clients.AddRange(client);
        _context.SaveChanges();

        var clientToDelete = _context.Clients.FirstOrDefault(c => c.Name == "TestClient");

        // Act
        if (clientToDelete != null)
        {
            await _service.DeleteClientAsync(clientToDelete.Id);
        }

        // Assert
        Assert.DoesNotContain(_context.Clients, c => c.Name == "TestClient");
    }
}
