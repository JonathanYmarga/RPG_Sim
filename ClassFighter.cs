using System;
using System.Drawing; // Required for Image

namespace RPG_Sim
{
    public abstract class ClassFighter : IDisposable // Implement IDisposable
    {
        public string Name { get; private set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public string LastAttackName { get; protected set; }

        public Image? CharacterSprite { get; protected set; } // Property to hold the character's image
        private bool disposedValue; // For IDisposable pattern

        public ClassFighter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Character name cannot be empty.");
            }
            Name = name;
            LastAttackName = "";
            CharacterSprite = null; // Initialize sprite to null
        }

        public abstract int Attack();

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
            }
        }

        // IDisposable Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    CharacterSprite?.Dispose(); // Dispose the image if it exists
                    CharacterSprite = null;
                }
                // Free unmanaged resources (if any) here.
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.  
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}