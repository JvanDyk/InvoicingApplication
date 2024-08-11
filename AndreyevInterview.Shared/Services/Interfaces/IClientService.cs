namespace AndreyevInterview.Shared.Services.Interfaces;

public interface IClientService
{
    /// <summary>
    /// Gets a collection of clients
    /// </summary>
    /// <returns>Returns a collection of clients.</returns>
    Task<IEnumerable<ClientEntity>> GetClientsAsync();

    /// <summary>
    /// Creates a client
    /// </summary>
    /// <param name="clientEntity">Client entity to be created</param>
    Task CreateClientAsync(ClientEntity clientEntity);

    /// <summary>
    /// Updates a client
    /// </summary>
    /// <param name="clientEntity">Entity of the client to be updated</param>
    Task UpdateClientAsync(ClientEntity clientEntity);

    /// <summary>
    /// Deletes a client
    /// </summary>
    /// <param name="id">Id of the client to be deleted</param>
    Task DeleteClientAsync(int id);
}
