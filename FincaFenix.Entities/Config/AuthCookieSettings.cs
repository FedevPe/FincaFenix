namespace FincaFenix.Entities.Config
{
    public class AuthCookieSettings
    {
        public string Name { get; set; } = "FincaFenix.Auth";
        public bool Secure { get; set; } = true;
        public string SameSite { get; set; } = "None";
        public string Domain { get; set; }
    }
}
