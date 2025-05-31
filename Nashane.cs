using System;
using System.Drawing; // Required for Image
using System.IO;    // Required for Path and File

namespace RPG_Sim
{
    public class Nashane : ClassFighter
    {
        private Random random = new Random();

        public Nashane(string name) : base(name)
        {
            MaxHealth = 110;
            Health = MaxHealth;
            LoadCharacterImage("Nashane.png"); 
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
                    Console.WriteLine($"Warning: Image not found for Nashane: {imagePath}");
                    CharacterSprite = null; // Ensure it's null if not found
                }
            }
            catch (Exception ex)
            {
                // Log the error and ensure CharacterSprite is null
                Console.WriteLine($"Error loading image for Nashane ({imageName}): {ex.Message}");
                CharacterSprite = null;
            }
        }

        public override int Attack()
        {
            int damage = random.Next(15, 25);
            string[] attacks = { "Code Comet", "Syntax Strike", "Bug Buster", "Firewall Fury" };
            string attackName = attacks[random.Next(attacks.Length)];
            LastAttackName = attackName;
            return damage;
        }

        public string DebuggingProwess()
        {
            return $"{Name} analyzes the weakness!";
        }
    }
}