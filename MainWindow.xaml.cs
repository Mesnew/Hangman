using System.Windows;

namespace Hangman
{
    public partial class MainWindow : Window
    {
        private ApiService _apiService = new ApiService();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Afficher les boutons de difficulté après connexion réussie
        private void ShowDifficultyOptions()
        {
            LoginRegisterPanel.Visibility = Visibility.Collapsed;  // Masquer les boutons login/register
            DifficultyPanel.Visibility = Visibility.Visible;       // Afficher les options de difficulté
        }

        // Vérifier les champs vides pour l'enregistrement
        private bool ValidateRegistrationInputs(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Veuillez entrer un nom d'utilisateur.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez entrer un mot de passe.");
                return false;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Le mot de passe et la confirmation ne correspondent pas.");
                return false;
            }

            return true;
        }

        // Vérifier les champs vides pour la connexion
        private bool ValidateLoginInputs(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Veuillez entrer un nom d'utilisateur.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez entrer un mot de passe.");
                return false;
            }

            return true;
        }

        private async void RegisterUser(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text;
            string password = PasswordInput.Password;
            string confirmPassword = ConfirmPasswordInput.Password;

            // Validation des champs
            if (!ValidateRegistrationInputs(username, password, confirmPassword))
            {
                return;
            }

            bool result = await _apiService.Register(username, password, confirmPassword);
            if (result)
            {
                MessageBox.Show("Enregistrement réussi");
                ShowDifficultyOptions();  // Afficher les boutons de difficulté après l'enregistrement
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement. Veuillez vérifier les informations.");
            }
        }

        private async void LoginUser(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text;
            string password = PasswordInput.Password;

            // Validation des champs
            if (!ValidateLoginInputs(username, password))
            {
                return;
            }

            string token = await _apiService.Login(username, password);
            if (token != null)
            {
                MessageBox.Show("Connexion réussie. Token : " + token);
                ShowDifficultyOptions();  // Afficher les boutons de difficulté après connexion
            }
            else
            {
                MessageBox.Show("Erreur lors de la connexion. Veuillez vérifier vos identifiants.");
            }
        }

        private void StartGameEasy(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow("Easy");
            gameWindow.Show();
            this.Close();
        }

        private void StartGameMedium(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow("Medium");
            gameWindow.Show();
            this.Close();
        }

        private void StartGameHard(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow("Hard");
            gameWindow.Show();
            this.Close();
        }
    }
}
