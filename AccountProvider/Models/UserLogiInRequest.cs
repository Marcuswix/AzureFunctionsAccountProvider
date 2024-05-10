namespace AccountProvider.Models
{
    public class UserLogiInRequest
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool IsPersistnet { get; set; }
    }
}
