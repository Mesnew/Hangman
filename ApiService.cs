using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

public class ApiService
{
    private static readonly string BaseUrl = "https://hangman.api.qoyri.fr";

    public async Task<bool> Register(string username, string password, string confirmPassword)
    {
        using (HttpClient client = new HttpClient())
        {
            var url = $"{BaseUrl}/api/Auth/register";
            var requestData = new
            {
                username = username,
                password = password,
                confirmPassword = confirmPassword
            };
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erreur lors de l'enregistrement : {errorMessage}");
                return false;
            }

            return true;
        }
    }

    public async Task<string> Login(string username, string password)
    {
        using (HttpClient client = new HttpClient())
        {
            var url = $"{BaseUrl}/api/Auth/login";
            var requestData = new { username = username, password = password };
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erreur lors de la connexion : {errorMessage}");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            // Stocker le token dans AuthService
            AuthService.Token = tokenResponse.Token;
            return tokenResponse.Token;
        }
    }

    public async Task<int?> SuccessfulAction(string guessedWord)
    {
        using (HttpClient client = new HttpClient())
        {
            var url = $"{BaseUrl}/api/Game/successful-action";
            var requestData = new { guessedWord = guessedWord };
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Utiliser le token stocké
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthService.Token);

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erreur lors de l'action réussie :\n{errorMessage}");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var scoreResponse = JsonConvert.DeserializeObject<ScoreResponse>(responseContent);
            return scoreResponse.Score;
        }
    }


    public class TokenResponse
    {
        public string Token { get; set; }
    }

    public class ScoreResponse
    {
        public int Score { get; set; }
    }
}
