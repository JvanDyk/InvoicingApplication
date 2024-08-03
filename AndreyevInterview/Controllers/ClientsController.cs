namespace AndreyevInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AIDbContext _context;
    private readonly IMapper _mapper;

    public ClientsController(AIDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }

    [HttpGet]
    public async Task<ActionResult<ClientDTO>> GetClients()
    {
        var listOfClients = new List<ClientDTO>();
        var clients = _context.Clients.ToList();

        foreach (ClientEntity client in clients)
        {
            var clientDto = _mapper.Map<ClientDTO>(client);
            listOfClients.Add(clientDto);
        }

        return Ok(listOfClients);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDTO>> CreateClientAsync(ClientDTO clientDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var client = _mapper.Map<ClientEntity>(clientDTO);

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateClientAsync(int id, ClientDTO client)
    {
        if (id != client.Id)
        {
            return BadRequest();
        }

        _context.Entry(client).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClientAsync([FromRoute]int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
