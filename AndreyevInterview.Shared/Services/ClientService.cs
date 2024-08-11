namespace AndreyevInterview.Shared.Services;

public class ClientService: IClientService
{
    private readonly AIDbContext _context;

    public ClientService(AIDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ClientEntity>> GetClientsAsync()
    {
        var listOfClients = await _context.Clients.ToListAsync();
        return listOfClients;
    }

    public async Task CreateClientAsync(ClientEntity clientEntity)
    {
        await _context.Clients.AddAsync(clientEntity);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateClientAsync(ClientEntity clientEntity)
    {
        _context.Entry(clientEntity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            throw new NotFoundException($"Client Entity Record not found for {id}");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }
}