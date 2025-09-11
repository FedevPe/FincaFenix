using FincaFenix.Entities.DTOs.Login;
using System.Net.Http.Json;

namespace FincaFenix.ViewModels.ViewModels.Login
{
    public class LoginViewModel
    {
        private readonly HttpClient _httpClient;

        public LoginViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResult> Login(LoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/login/validatecredentials", loginDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResult>();
            }

            // Maneja el caso de error.
            return new LoginResult { IsSuccess = false };
        }
    }
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
    }
}
