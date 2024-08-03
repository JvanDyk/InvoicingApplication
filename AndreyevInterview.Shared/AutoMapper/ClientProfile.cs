namespace AndreyevInterview.Shared.AutoMapper;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<ClientEntity, ClientDTO>()
            .ReverseMap();
    }
}
