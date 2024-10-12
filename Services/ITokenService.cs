using Blog.Models;

namespace Blog.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}