namespace FincaFenix.Entities.DTOs.Login
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
