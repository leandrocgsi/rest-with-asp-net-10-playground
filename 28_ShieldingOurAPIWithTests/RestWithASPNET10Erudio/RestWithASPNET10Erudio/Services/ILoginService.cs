using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNETErudio.Data.DTO;

namespace RestWithASPNETErudio.Services
{
    public interface ILoginService
    {
        TokenDTO? ValidateCredentials(UserDTO user);
        TokenDTO? ValidateCredentials(TokenDTO token);
        bool RevokeToken(string username);
        AccountCredentialsDTO Create(AccountCredentialsDTO user);
    }
}