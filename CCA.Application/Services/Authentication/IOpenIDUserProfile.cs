namespace CCA.Application.Services.Authentication
{
    public interface IOpenIDUserProfile
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
