using System.Windows;
using System.Windows.Threading;

namespace Hangman
{
    public partial class GameWindow : Window
    {
        private GameViewModel _viewModel;
        private DispatcherTimer messageTimer;

        public GameWindow(string difficulty)
        {
            InitializeComponent();
            _viewModel = new GameViewModel(difficulty);
            this.DataContext = _viewModel;

            messageTimer = new DispatcherTimer();
            messageTimer.Interval = TimeSpan.FromSeconds(3);
            messageTimer.Tick += MessageTimer_Tick;
        }

        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            _viewModel.IsVictory = false;
            messageTimer.Stop();
        }

        public void OnWordFound()
        {
            _viewModel.IsVictory = true;
            messageTimer.Start();
        }

        private void OnRetryClicked(object param)
        {
            _viewModel.NewGame(false, true);
        }
    }
}