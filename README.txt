README.txt

== RPG Battle Simulator (Simplified) ==

This is a fun C# Windows Forms game that lets you simulate battles with characters inspired by classmates! It's built using important coding ideas called Object-Oriented Programming (OOP).

---
## Character Descriptions ##

Meet the fighters:

1.  **Nashane:**
    * **Theme:** A cool coder character.
    * **Special:** Uses "Code Comet" and "Debugging Prowess."

2.  **Caius:**
    * **Theme:** A smart quiz master.
    * **Special:** Attacks with "Trivia Tornado" and can "AnnounceQuiz."

Each character has their own picture in the game.

---
## How OOP Principles Were Used ##

These coding principles helped build the game:

1.  **Encapsulation:**
    * **What it is:** Keeping related data and actions bundled together and protected.
    * **Example:** A character's health and how they take damage are managed within their own code. The main game window (`Form1.cs`) also bundles its buttons and battle logic together.

2.  **Inheritance:**
    * **What it is:** Creating new things based on a blueprint.
    * **Example:** `Nashane` and `Caius` are specific fighters based on a general `ClassFighter` blueprint. They get common features like having a name and health from this blueprint.

3.  **Polymorphism:**
    * **What it is:** Letting one command have different results depending on who receives it.
    * **Example:** When the game says `Attack()`, `Nashane` attacks with her code style, and `Caius` attacks with his quiz style.

4.  **Abstraction:**
    * **What it is:** Hiding complex details and showing only what's necessary.
    * **Example:** The game works with a general idea of a "Fighter" (`ClassFighter`) that knows how to `Attack()`. It doesn't need to know the tiny details of *how* each specific character performs their attack.

5.  **Exception Handling:**
    * **What it is:** Dealing with errors gracefully so the program doesn't crash.
    * **Example:** If you forget to type a player's name or if a character's image file is missing, the game shows a friendly message instead of breaking, using `try-catch` blocks.

---
## Challenges Faced (And How They Were Solved) ##

Building this game had a few tricky parts:

1.  **Keeping the Game Smooth:**
    * **Challenge:** Making sure the game window didn't freeze during battle turns.
    * **Solution:** Used `async` code for the battle loop, so the game could pause between turns without stopping everything else.

2.  **Handling Character Pictures:**
    * **Challenge:** Loading character images correctly and making sure they didn't use up too much computer memory.
    * **Solution:** Implemented careful image loading (checking if files exist) and made sure to release image resources when they weren't needed anymore (using `IDisposable` and `Dispose()` methods).

3.  **Catching Mistakes Nicely:**
    * **Challenge:** If users made mistakes (like not entering names) or if files were missing, the game could crash.
    * **Solution:** Added `try-catch` blocks to catch these problems and show helpful error messages to the user.