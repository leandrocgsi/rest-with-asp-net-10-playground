using RestWithASPNET10Erudio.Model;

namespace RestWithASPNETErudio.Repository
{
    public interface IUserRepository
    {
        User? ValidateCredentials(string username, string password);
        User? ValidateCredentials(string username);
        User? RefreshUserInfo(User user);
        bool RevokeToken(string username);
    }
}
