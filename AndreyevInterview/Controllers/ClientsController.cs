namespace AndreyevInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
    }

    /// <summary>
    /// Retrieves all clients.
    /// </summary>
    /// <returns>Returns a collection of Clients.</returns>
    [HttpGet]
    public async Task<ActionResult<List<ClientEntity>>> GetClients()
    {
        var clients = await _clientService.GetClientsAsync();
        return Ok(clients);
    }

    /// <summary>
    /// Retrieves a client.
    /// </summary>
    /// <returns>Returns a client.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<List<ClientEntity>>> GetClient([FromRoute]int id)
    {
        var client = await _clientService.GetClientAsync(id);
        return Ok(client);
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="clientEntity">Information about the client to create.</param>
    [HttpPost]
    public async Task<ActionResult> CreateClientAsync(ClientEntity clientEntity)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await _clientService.CreateClientAsync(clientEntity);
        return Ok();
    }

    /// <summary>
    /// Updates existent client's record.
    /// </summary>
    /// <param name="id">The identifier of the client to update.</param>
    /// <param name="clientEntity">Contain contains updated client's info.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateClientAsync(int id, ClientEntity clientEntity)
    {
        if (id != clientEntity.Id || String.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await _clientService.UpdateClientAsync(clientEntity);
        return Ok();
    }

    /// <summary>
    /// Deletes a client based on ID.
    /// </summary>
    /// <param name="id">The identifier of the client to be deleted.</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClientAsync(int id)
    {
        await _clientService.DeleteClientAsync(id);
        return Ok();
    }
}