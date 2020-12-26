using System.Threading.Tasks;

namespace CCA.Application.Services.Authentication
{
    public interface IOpenIDProvider
    {
        string GetLoginUrl();

        Task<IOpenIDUserProfile?> GetProfileAsync(string code);
    }
}
