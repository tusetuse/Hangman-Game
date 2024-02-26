using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hangman_app
{
    public partial class Form1 : Form
    {
        private string[] words;
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
            RetrieveWordsFromMongoDB();

            if (words != null && words.Length > 0)
            {
                Random rand = new Random();
                int index = rand.Next(words.Length);
                currentWord = words[index];

                displayWord = new string('_', currentWord.Length);
                label1.Text = String.Join(" ", displayWord.ToCharArray());
                incorrectGuesses = 0;
                UpdateHangmanImage(0);
            }
            else
            {
                MessageBox.Show("Error: No words retrieved from MongoDB.");
            }
        }

        private void RetrieveWordsFromMongoDB()
        {
            try
            {
                string connectionString = "mongodb://root:ivana@localhost:27017/?directConnection=true";
                MongoClient client = new MongoClient(connectionString);
                IMongoDatabase database = client.GetDatabase("HangManGame");
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("guess");
                var document = collection.Find(new BsonDocument()).FirstOrDefault();
                if (document != null && document.Contains("word") && document["word"].IsBsonArray)
                {
                    var wordsArray = document["word"].AsBsonArray;
                    words = wordsArray.Select(word => word.ToString()).ToArray();
                }
                else
                {
                    MessageBox.Show("Error: Invalid document structure or no words found.");
                    words = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving words from MongoDB: {ex.Message}");
                words = null;
            }
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
                    InitializeGame();
                }
            }
            else
            {
                incorrectGuesses++;
                UpdateHangmanImage(incorrectGuesses);
                if (incorrectGuesses >= 12)
                {
                    MessageBox.Show($"Sorry, you've lost! The word was: {currentWord}");
                    InitializeGame();
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
