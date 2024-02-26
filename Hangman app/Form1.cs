using Microsoft.VisualBasic.ApplicationServices;
using System;

namespace Hangman_app
{
    public partial class Form1 : Form
    {
        private string[] words = new string[] { "example", "mystery", "hangman", "challenge", "programming", "code", "debug" };
        private string currentWord = "";
        private string displayWord = "";
        private int incorrectGuesses = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }
        private void InitializeGame()
        {
            Random rand = new Random();
            int index = rand.Next(words.Length);
            currentWord = words[index];

            displayWord = new string('_', currentWord.Length);
            label1.Text = String.Join(" ", displayWord.ToCharArray());
            incorrectGuesses = 0;
            UpdateHangmanImage(0); // Show initial hangman state
        }

        public void EnterGuess()
        {
            var guess = textBox1.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(guess) && guess.Length == 1)
            {
                ProcessGuess(guess[0]);
            }
            textBox1.Clear();
            textBox1.Focus();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            EnterGuess();
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                EnterGuess();
            }    
        }

        private void ProcessGuess(char guess)
        {
            bool correctGuess = false;
            for (int i = 0; i < currentWord.Length; i++)
            {
                if (currentWord[i] == guess)
                {
                    displayWord = displayWord.Remove(i, 1).Insert(i, guess.ToString());
                    correctGuess = true;
                }
            }

            if (correctGuess)
            {
                label1.Text = String.Join(" ", displayWord.ToCharArray());
                if (!displayWord.Contains("_"))
                {
                    MessageBox.Show("Congratulations, you've won!");
                    InitializeGame(); // Reset the game
                }
            }
            else
            {
                incorrectGuesses++;
                UpdateHangmanImage(incorrectGuesses);
                if (incorrectGuesses >= 12)
                {
                    MessageBox.Show($"Sorry, you've lost! The word was: {currentWord}");
                    InitializeGame();// Reset the game
                }
            }
        }

        private void UpdateHangmanImage(int incorrectGuesses)
        {
            string basePath = @"C:\Users\ivanr\Desktop\C sharp programs\Hangman app\Hangman app\";
            string imageName = $"H{incorrectGuesses}.png";
            string imagePath = System.IO.Path.Combine(basePath, imageName);

            try
            {
                pictureBox1.Image = Image.FromFile(imagePath);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Image file not found: {imagePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}