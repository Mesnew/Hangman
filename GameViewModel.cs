using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

public class GameViewModel : INotifyPropertyChanged
{
    private GameModel _gameModel;
    private ApiService _apiService = new ApiService();  // Service API

    public string DisplayWord { get; private set; } = string.Empty;

    public int AttemptsLeft
    {
        get => _gameModel.AttemptsLeft;
        set => throw new NotImplementedException();
    }

    public Dictionary<char, bool> ClickedLetters { get; private set; } = new Dictionary<char, bool>();
    public ICommand GuessCommand { get; }
    public ICommand NewGameCommand { get; }
    public ICommand NextWordCommand { get; }

    private int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            OnPropertyChanged(nameof(Score));
        }
    }

    private string _message;
    public string Message
    {
        get { return _message; }
        set
        {
            _message = value;
            OnPropertyChanged(nameof(Message));
        }
    }

    private bool _isVictory;
    public bool IsVictory
    {
        get { return _isVictory; }
        set
        {
            _isVictory = value;
            OnPropertyChanged(nameof(IsVictory));
        }
    }

    private bool _showVictoryMessage;
    public bool ShowVictoryMessage
    {
        get { return _showVictoryMessage; }
        set
        {
            _showVictoryMessage = value;
            OnPropertyChanged(nameof(ShowVictoryMessage));
        }
    }

    private bool _isGameOver;
    public bool IsGameOver
    {
        get { return _isGameOver; }
        set
        {
            _isGameOver = value;
            OnPropertyChanged(nameof(IsGameOver));
        }
    }

    public GameViewModel(string difficulty)
    {
        _gameModel = new GameModel();
        _gameModel.LoadRandomWord();
        GuessCommand = new RelayCommand(OnLetterClicked);
        NewGameCommand = new RelayCommand(param => NewGame(false, false));
        NextWordCommand = new RelayCommand(OnNextWordClicked);

        UpdateDisplayWord();
    }

    public void NewGame(bool isVictory, bool isRetry)
    {
        IsVictory = false;
        ShowVictoryMessage = false;

        if (isRetry)
        {
            if (Score > 0) Score--; 
            Message = "Nouveau mot, -1 point"; 
        }

        IsGameOver = !isVictory;
        _gameModel.LoadRandomWord();
        _gameModel.ResetGame();
        ClickedLetters.Clear();
        IsGameOver = false;
        UpdateDisplayWord();
        OnPropertyChanged(nameof(AttemptsLeft));
        OnPropertyChanged(nameof(ClickedLetters));
        OnPropertyChanged(nameof(IsGameOver));
        OnPropertyChanged(nameof(IsVictory));
        OnPropertyChanged(nameof(Score)); 
    }

    private void OnNextWordClicked(object param)
    {
        NewGame(true, false);
    }

    public async void GuessLetter(char letter)
    {
        if (IsGameOver) return;

        if (!ClickedLetters.ContainsKey(letter))
        {
            bool isCorrect = _gameModel.CheckLetter(letter);
            ClickedLetters[letter] = isCorrect;

            if (!isCorrect)
            {
                OnPropertyChanged(nameof(AttemptsLeft));
            }

            UpdateDisplayWord();
            OnPropertyChanged(nameof(ClickedLetters));

            if (_gameModel.IsWordGuessed())
            {
                IsVictory = true;

                var score = await _apiService.SuccessfulAction(_gameModel.CurrentWord);
                if (score.HasValue)
                {
                    Score = score.Value;
                }
                ShowVictoryMessage = true;
                IsGameOver = false;
                OnPropertyChanged(nameof(Score));
                OnPropertyChanged(nameof(IsVictory));
                OnPropertyChanged(nameof(ShowVictoryMessage));
                OnPropertyChanged(nameof(IsGameOver));
            }
            else if (AttemptsLeft <= 0)
            {
                IsGameOver = true;
                IsVictory = false;
                OnPropertyChanged(nameof(IsGameOver));
            }
        }
    }

    private void OnLetterClicked(object parameter)
    {
        if (parameter is string letterStr && letterStr.Length == 1)
        {
            char letter = letterStr[0];
            GuessLetter(letter);
        }
    }

    private void UpdateDisplayWord()
    {
        DisplayWord = new string(_gameModel.CurrentWord
            .Select(c => _gameModel.GuessedLetters.Contains(c) ? c : '*')
            .ToArray());
        OnPropertyChanged(nameof(DisplayWord));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
