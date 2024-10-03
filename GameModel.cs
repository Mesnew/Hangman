using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class GameModel
{
    public string CurrentWord { get; private set; }
    public int AttemptsLeft { get; private set; } = 6;
    public List<char> GuessedLetters { get; private set; } = new List<char>();
    public void LoadRandomWord()
    {
        var words = LoadWordsFromJson();
        if (words == null || words.Length == 0)
        {
            throw new Exception("Aucun mot trouvé dans le fichier JSON.");
        }

        var random = new Random();
        CurrentWord = words[random.Next(words.Length)];
    }
    private string[] LoadWordsFromJson()
    {
        var json = File.ReadAllText("mots.json");
        var wordDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        // Get the values (words) from the dictionary
        return wordDict.Values.ToArray();
    }
    public bool CheckLetter(char letter)
    {
        if (CurrentWord.Contains(letter))
        {
            GuessedLetters.Add(letter);
            return true;
        }
        else
        {
            AttemptsLeft = Math.Max(0, AttemptsLeft - 1); // Ne descend pas en dessous de 0
            return false;
        }
    }
    
    public bool IsGameOver() => AttemptsLeft == 0 || IsWordGuessed();
    public bool IsWordGuessed()
    {
        return CurrentWord.All(c => GuessedLetters.Contains(c));
    }

    public class WordList
    {
        public string[] Words { get; set; }
    }
    public void ResetGame()
    {
        GuessedLetters.Clear(); // Réinitialiser la liste des lettres devinées
        AttemptsLeft = 6; // Réinitialiser le nombre d'essais
    }
}