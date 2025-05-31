using System;
using System.Drawing; // Required for Image
using System.IO;    // Required for Path and File

namespace RPG_Sim
{
    public class Caius : ClassFighter
    {
        private Random random = new Random();

        public Caius(string name) : base(name)
        {
            MaxHealth = 90;
            Health = MaxHealth;
            LoadCharacterImage("Caius.png"); 
        }

        private void LoadCharacterImage(string imageName)
        {
            try
            {
                // Assumes an "Images" folder in the application's startup directory
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = Path.Combine(baseDirectory, "Assets", imageName);

                if (File.Exists(imagePath))
                {
                    // Load image in a way that doesn't lock the file
                    using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        CharacterSprite = new Bitmap(stream);
                    }
                }
                else
                {
                    // Optionally, log a warning or set a default placeholder image
                    Console.WriteLine($"Warning: Image not found for Caius: {imagePath}");
                    CharacterSprite = null; // Ensure it's null if not found
                }
            }
            catch (Exception ex)
            {
                // Log the error and ensure CharacterSprite is null
                Console.WriteLine($"Error loading image for Caius ({imageName}): {ex.Message}");
                CharacterSprite = null;
            }
        }

        public override int Attack()
        {
            int damage = random.Next(10, 30);
            string[] attacks = { "Trivia Tornado", "Knowledge Knockout", "Fact Flurry", "Intellect Impact" };
            string attackName = attacks[random.Next(attacks.Length)];
            LastAttackName = attackName;
            return damage;
        }

        public string AnnounceQuiz()
        {
            return $"{Name} poses a mind-bending question!";
        }
    }
}