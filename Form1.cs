using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging; // Required for ColorMatrix and ImageAttributes
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Versioning;
using System.IO; // Required for Path

namespace RPG_Sim
{
    [SupportedOSPlatform("windows7.0")]
    public partial class Form1 : Form
    {
        // UI Controls
        private TextBox? txtPlayer1Name;
        private ComboBox? cmbPlayer1Type;
        private Label? lblPlayer1Health;
        private TextBox? txtPlayer2Name;
        private ComboBox? cmbPlayer2Type;
        private Label? lblPlayer2Health;
        private Button? btnStartBattle;
        private ListBox? lstBattleLog;
        private Label? lblWinner;
        private PictureBox? picPlayer1Character;
        private PictureBox? picPlayer2Character;

        private List<Type> characterTypes = new List<Type>();
        private ClassFighter? player1;
        private ClassFighter? player2;
        private Random battleRandom = new Random();

        // Theme Colors and Fonts 
        private Color formBackColor = Color.FromArgb(240, 248, 255); 
        private Color controlBackColor = Color.FromArgb(220, 235, 250); 
        private Color labelBackColor = Color.FromArgb(240, 248, 255); 
        private Color textColor = Color.FromArgb(45, 45, 70);
        private Color accentColor1 = Color.FromArgb(70, 130, 180);
        private Color accentColor2 = Color.FromArgb(0, 150, 136);
        private Color accentHoverColor = Color.FromArgb(0, 170, 156);
        private Color accentClickColor = Color.FromArgb(0, 130, 116);
        private Color winnerColor = Color.FromArgb(255, 140, 0);
        private Color healthGoodColor = Color.FromArgb(34, 177, 76);
        private Color healthLowColor = Color.FromArgb(255, 127, 39);
        private Color healthCriticalColor = Color.FromArgb(237, 28, 36);
        private Color healthDefeatedColor = Color.Gray;

        private Font regularFont = new Font("Segoe UI", 9.5F);
        private Font boldFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        private Font titleFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private Font logFont;
        private Font buttonFont = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        private Font winnerFont = new Font("Segoe UI", 14F, FontStyle.Bold);

        public Form1()
        {
            try { logFont = new Font("Consolas", 9F); }
            catch { logFont = new Font("Segoe UI", 9F); }

            InitializeComponent();
            InitializeCustomComponents();
            PopulateCharacterTypes();
            ResetCharacterDisplays();
        }

        private Bitmap SetImageOpacity(Image originalImage, float opacity)
        {
            if (originalImage == null) throw new ArgumentNullException(nameof(originalImage));
            if (opacity < 0.0f || opacity > 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(opacity), "Opacity must be between 0.0 and 1.0.");
            }

            Bitmap transparentBitmap = new Bitmap(originalImage.Width, originalImage.Height);
            using (Graphics gfx = Graphics.FromImage(transparentBitmap))
            {
                ColorMatrix matrix = new ColorMatrix { Matrix33 = opacity };
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    gfx.DrawImage(originalImage,
                                  new Rectangle(0, 0, transparentBitmap.Width, transparentBitmap.Height),
                                  0, 0, originalImage.Width, originalImage.Height,
                                  GraphicsUnit.Pixel, attributes);
                }
            }
            return transparentBitmap;
        }

        private void LoadAndSetBackgroundImage()
        {
            // Set the form's base BackColor. 
            this.BackColor = formBackColor;

            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string assetsPath = Path.Combine(baseDirectory, "Assets");
                string backgroundImagePath = Path.Combine(assetsPath, "Classroom.jpg");

                if (File.Exists(backgroundImagePath))
                {
                    this.BackgroundImage?.Dispose(); 

                    using (Image originalBgImage = Image.FromFile(backgroundImagePath))
                    {
                        // Make it "much more transparent". For example, 15% opacity.
                        float desiredOpacity = 0.50f; // Lower value for more transparency
                        this.BackgroundImage = SetImageOpacity(originalBgImage, desiredOpacity);
                    }
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    MessageBox.Show($"Background image 'Classroom.jpg' not found in '{assetsPath}'. Using solid fallback color.", "Image Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // BackgroundImage will be null, so Form.BackColor (formBackColor) will be visible.
                    this.BackgroundImage = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading or processing background image: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BackgroundImage = null; // Ensure no broken image is set
            }
        }

        private void InitializeCustomComponents()
        {
            this.SuspendLayout();

            LoadAndSetBackgroundImage(); // Load, process for opacity, and set the background image

            this.Text = "Classroom Battle Simulator";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = regularFont;

            // --- Opaque Labels ---

            var lblP1Title = new Label() { Text = "PLAYER 1", Location = new Point(20, 15), Font = titleFont, ForeColor = accentColor1, AutoSize = true, BackColor = labelBackColor };
            var lblP1Name = new Label() { Text = "Name:", Location = new Point(20, 45), Font = regularFont, ForeColor = textColor, AutoSize = true, BackColor = labelBackColor };
            txtPlayer1Name = new TextBox() { Location = new Point(150, 43), Width = 150, Font = regularFont, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            var lblP1Type = new Label() { Text = "Character Type:", Location = new Point(20, 75), Font = regularFont, ForeColor = textColor, AutoSize = true, BackColor = labelBackColor };
            cmbPlayer1Type = new ComboBox() { Location = new Point(150, 73), Width = 150, Font = regularFont, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White };
            lblPlayer1Health = new Label() { Text = "P1 Health: --/--", Location = new Point(20, 105), AutoSize = true, Font = boldFont, ForeColor = healthGoodColor, BackColor = labelBackColor };

            var lblP2Title = new Label() { Text = "PLAYER 2", Location = new Point(350, 15), Font = titleFont, ForeColor = accentColor1, AutoSize = true, BackColor = labelBackColor };
            var lblP2Name = new Label() { Text = "Name:", Location = new Point(350, 45), Font = regularFont, ForeColor = textColor, AutoSize = true, BackColor = labelBackColor };
            txtPlayer2Name = new TextBox() { Location = new Point(480, 43), Width = 150, Font = regularFont, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            var lblP2Type = new Label() { Text = "Character Type:", Location = new Point(350, 75), Font = regularFont, ForeColor = textColor, AutoSize = true, BackColor = labelBackColor };
            cmbPlayer2Type = new ComboBox() { Location = new Point(480, 73), Width = 150, Font = regularFont, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White };
            lblPlayer2Health = new Label() { Text = "P2 Health: --/--", Location = new Point(350, 105), AutoSize = true, Font = boldFont, ForeColor = healthGoodColor, BackColor = labelBackColor };

            // --- Opaque PictureBoxes ---
            picPlayer1Character = new PictureBox
            {
                Location = new Point(20, 135), Size = new Size(150, 100), BorderStyle = BorderStyle.FixedSingle,
                BackColor = controlBackColor, // Opaque background from theme
                SizeMode = PictureBoxSizeMode.Zoom, Tag = "P1Display"
            };
            picPlayer2Character = new PictureBox
            {
                Location = new Point(350, 135), Size = new Size(150, 100), BorderStyle = BorderStyle.FixedSingle,
                BackColor = controlBackColor, // Opaque background from theme
                SizeMode = PictureBoxSizeMode.Zoom, Tag = "P2Display"
            };

            int battleLogStartY = 250;
            var lblBattleLogTitle = new Label() { Text = "BATTLE LOG", Location = new Point(20, battleLogStartY), Font = titleFont, ForeColor = accentColor1, AutoSize = true, BackColor = labelBackColor };
            lstBattleLog = new ListBox()
            {
                Location = new Point(20, battleLogStartY + 28), Width = 640, Height = 150, Font = logFont,
                BackColor = Color.WhiteSmoke, // Opaque
                ForeColor = textColor, BorderStyle = BorderStyle.FixedSingle, HorizontalScrollbar = true, ItemHeight = regularFont.Height + 2
            };

            int controlsBelowLogY = lstBattleLog.Location.Y + lstBattleLog.Height + 20;
            btnStartBattle = new Button()
            {
                Text = "Start Battle!", Location = new Point(280, controlsBelowLogY), Width = 140, Height = 45, Font = buttonFont,
                BackColor = accentColor2, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnStartBattle.FlatAppearance.BorderSize = 0;
            btnStartBattle.FlatAppearance.MouseOverBackColor = accentHoverColor;
            btnStartBattle.FlatAppearance.MouseDownBackColor = accentClickColor;
            btnStartBattle.Click += BtnStartBattle_Click;

            lblWinner = new Label()
            {
                Text = "", Location = new Point(20, controlsBelowLogY + btnStartBattle.Height + 15), Font = winnerFont, ForeColor = accentColor1,
                AutoSize = false, Size = new Size(640, 30), TextAlign = ContentAlignment.MiddleCenter,
                BackColor = labelBackColor // Opaque background
            };

            this.Controls.AddRange(new Control[] {
                lblP1Title, lblP1Name, txtPlayer1Name, lblP1Type, cmbPlayer1Type, lblPlayer1Health, picPlayer1Character,
                lblP2Title, lblP2Name, txtPlayer2Name, lblP2Type, cmbPlayer2Type, lblPlayer2Health, picPlayer2Character,
                lblBattleLogTitle, lstBattleLog, btnStartBattle, lblWinner
            });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ResetCharacterDisplays()
        {
            if (picPlayer1Character != null)
            {
                picPlayer1Character.Image?.Dispose();
                picPlayer1Character.Image = null;
                picPlayer1Character.BackColor = controlBackColor; // Opaque background
            }
            if (picPlayer2Character != null)
            {
                picPlayer2Character.Image?.Dispose();
                picPlayer2Character.Image = null;
                picPlayer2Character.BackColor = controlBackColor; // Opaque background
            }
            UpdateHealthDisplay();
        }

        private void PopulateCharacterTypes()
        {
            characterTypes.Add(typeof(Nashane));
            characterTypes.Add(typeof(Caius));

            if (cmbPlayer1Type == null || cmbPlayer2Type == null) return;
            cmbPlayer1Type.Items.Clear();
            cmbPlayer2Type.Items.Clear();
            foreach (var type in characterTypes)
            {
                cmbPlayer1Type.Items.Add(type.Name);
                cmbPlayer2Type.Items.Add(type.Name);
            }
            if (cmbPlayer1Type.Items.Count > 0) cmbPlayer1Type.SelectedIndex = 0;
            if (cmbPlayer2Type.Items.Count > 0) cmbPlayer2Type.SelectedIndex = 0;
        }

        private async void BtnStartBattle_Click(object? sender, EventArgs e)
        {
            if (lstBattleLog == null || lblWinner == null || txtPlayer1Name == null || cmbPlayer1Type == null ||
                txtPlayer2Name == null || cmbPlayer2Type == null || btnStartBattle == null ||
                picPlayer1Character == null || picPlayer2Character == null)
            {
                MessageBox.Show("UI components are not fully initialized.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lstBattleLog.Items.Clear();
            lblWinner.Text = "";
            lblWinner.ForeColor = accentColor1;

            player1?.Dispose();
            player2?.Dispose();
            player1 = null;
            player2 = null;

            ResetCharacterDisplays();

            try
            {
                if (string.IsNullOrWhiteSpace(txtPlayer1Name.Text)) throw new ArgumentException("Player 1 name cannot be empty!");
                if (string.IsNullOrWhiteSpace(txtPlayer2Name.Text)) throw new ArgumentException("Player 2 name cannot be empty!");
                if (cmbPlayer1Type.SelectedItem == null) throw new ArgumentException("Please select a character type for Player 1!");
                if (cmbPlayer2Type.SelectedItem == null) throw new ArgumentException("Please select a character type for Player 2!");

                string p1SelectedTypeName = cmbPlayer1Type.SelectedItem.ToString()!;
                Type? p1SelectedType = characterTypes.FirstOrDefault(t => t.Name == p1SelectedTypeName);
                if (p1SelectedType == null) throw new InvalidOperationException($"Character type '{p1SelectedTypeName}' not found.");
                player1 = (ClassFighter?)Activator.CreateInstance(p1SelectedType, txtPlayer1Name.Text);
                if (player1 == null) throw new InvalidOperationException($"Failed to create instance of {p1SelectedTypeName}.");

                string p2SelectedTypeName = cmbPlayer2Type.SelectedItem.ToString()!;
                Type? p2SelectedType = characterTypes.FirstOrDefault(t => t.Name == p2SelectedTypeName);
                if (p2SelectedType == null) throw new InvalidOperationException($"Character type '{p2SelectedTypeName}' not found.");
                player2 = (ClassFighter?)Activator.CreateInstance(p2SelectedType, txtPlayer2Name.Text);
                if (player2 == null) throw new InvalidOperationException($"Failed to create instance of {p2SelectedTypeName}.");

                UpdateCharacterVisual(picPlayer1Character, player1);
                UpdateCharacterVisual(picPlayer2Character, player2);

                LogBattleEvent($"Battle Begins: {player1.Name} ({player1.GetType().Name}) vs {player2.Name} ({player2.GetType().Name})");
                LogBattleEvent($"Initial HP - {player1.Name}: {player1.Health}/{player1.MaxHealth}, {player2.Name}: {player2.Health}/{player2.MaxHealth}");
                UpdateHealthDisplay();

                btnStartBattle.Enabled = false;
                lblWinner.Text = "Battle in progress...";

                await StartBattleLoopAsync();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogBattleEvent($"Error setting up battle: {argEx.Message}");
                if (btnStartBattle != null) btnStartBattle.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.ToString()}", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogBattleEvent($"CRITICAL Error: {ex.Message}");
                player1?.Dispose();
                player2?.Dispose();
                player1 = null;
                player2 = null;
                ResetCharacterDisplays();
                if (btnStartBattle != null) btnStartBattle.Enabled = true;
            }
        }

        private void UpdateCharacterVisual(PictureBox characterBox, ClassFighter fighter)
        {
            if (characterBox == null) return;
            characterBox.Image?.Dispose();

            if (fighter.CharacterSprite != null)
            {
                characterBox.Image = fighter.CharacterSprite;
            }
            else
            {
                characterBox.Image = null;
                // Ensure it has its opaque background if no image
                characterBox.BackColor = controlBackColor;
            }
        }

        private async Task StartBattleLoopAsync()
        {
            if (player1 == null || player2 == null || btnStartBattle == null || lblWinner == null)
            {
                LogBattleEvent("ERROR: One or both players (or UI elements) are not initialized for battle.");
                if (btnStartBattle != null) btnStartBattle.Enabled = true;
                if (lblWinner != null) lblWinner.Text = "Battle setup error.";
                return;
            }

            try
            {
                ClassFighter attacker = (battleRandom.Next(0, 2) == 0) ? player1 : player2;
                ClassFighter defender = (attacker == player1) ? player2 : player1;
                LogBattleEvent($"{attacker.Name} wins the coin toss and attacks first!");

                while (attacker.Health > 0 && defender.Health > 0)
                {
                    LogBattleEvent("----------------------------------");
                    int damageDealt = attacker.Attack();
                    LogBattleEvent($"{attacker.Name} uses {attacker.LastAttackName} on {defender.Name}!");
                    if (attacker is Nashane nashane) LogBattleEvent(nashane.DebuggingProwess());
                    else if (attacker is Caius caius) LogBattleEvent(caius.AnnounceQuiz());
                    defender.TakeDamage(damageDealt);
                    LogBattleEvent($"{defender.Name} takes {damageDealt} damage. Remaining HP: {defender.Health}/{defender.MaxHealth}");
                    UpdateHealthDisplay();

                    if (defender.Health <= 0)
                    {
                        LogBattleEvent("----------------------------------");
                        LogBattleEvent($"{defender.Name} has been defeated!");
                        lblWinner.Text = $"{attacker.Name} is the WINNER!";
                        lblWinner.ForeColor = winnerColor;
                        if (defender == player1 && picPlayer1Character != null) UpdateDefeatedVisual(picPlayer1Character);
                        else if (defender == player2 && picPlayer2Character != null) UpdateDefeatedVisual(picPlayer2Character);
                        break;
                    }
                    var temp = attacker; attacker = defender; defender = temp;
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during battle: {ex.Message}", "Battle Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogBattleEvent($"BATTLE ERROR: {ex.Message}");
            }
            finally
            {
                if (btnStartBattle != null) btnStartBattle.Enabled = true;
                if (lblWinner != null && (string.IsNullOrWhiteSpace(lblWinner.Text) || lblWinner.Text == "Battle in progress..."))
                {
                    if (player1 != null && player2 != null)
                    {
                        if (player1.Health <= 0 && player2.Health > 0) { lblWinner.Text = $"{player2.Name} is the WINNER!"; lblWinner.ForeColor = winnerColor; }
                        else if (player2.Health <= 0 && player1.Health > 0) { lblWinner.Text = $"{player1.Name} is the WINNER!"; lblWinner.ForeColor = winnerColor; }
                        else if (player1.Health <= 0 && player2.Health <= 0) { lblWinner.Text = "It's a DRAW!"; }
                        else { lblWinner.Text = "Battle Concluded."; }
                    } else { lblWinner.Text = "Battle ended."; }
                }
            }
        }

        private void UpdateDefeatedVisual(PictureBox characterBox)
        {
            if (characterBox == null) return;
        }

        private void LogBattleEvent(string message)
        {
            if (lstBattleLog == null) return;
            if (lstBattleLog.InvokeRequired)
            {
                lstBattleLog.Invoke(new Action(() => LogBattleEvent(message)));
            }
            else
            {
                lstBattleLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (lstBattleLog.Items.Count > 0) lstBattleLog.TopIndex = lstBattleLog.Items.Count - 1;
            }
        }

        private void UpdateHealthDisplay()
        {
            if (this.InvokeRequired) { this.Invoke(new Action(UpdateHealthDisplay)); return; }
            if (lblPlayer1Health == null || lblPlayer2Health == null) return;

            if (player1 != null)
            {
                lblPlayer1Health.Text = $"P1: {player1.Name} - {player1.Health}/{player1.MaxHealth} HP";
                if (player1.Health <= 0) lblPlayer1Health.ForeColor = healthDefeatedColor;
                else if (player1.Health < player1.MaxHealth * 0.25) lblPlayer1Health.ForeColor = healthCriticalColor;
                else if (player1.Health < player1.MaxHealth * 0.50) lblPlayer1Health.ForeColor = healthLowColor;
                else lblPlayer1Health.ForeColor = healthGoodColor;
            } else { lblPlayer1Health.Text = "P1 Health: --/--"; lblPlayer1Health.ForeColor = healthGoodColor; }

            if (player2 != null)
            {
                lblPlayer2Health.Text = $"P2: {player2.Name} - {player2.Health}/{player2.MaxHealth} HP";
                if (player2.Health <= 0) lblPlayer2Health.ForeColor = healthDefeatedColor;
                else if (player2.Health < player2.MaxHealth * 0.25) lblPlayer2Health.ForeColor = healthCriticalColor;
                else if (player2.Health < player2.MaxHealth * 0.50) lblPlayer2Health.ForeColor = healthLowColor;
                else lblPlayer2Health.ForeColor = healthGoodColor;
            } else { lblPlayer2Health.Text = "P2 Health: --/--"; lblPlayer2Health.ForeColor = healthGoodColor; }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            player1?.Dispose();
            player2?.Dispose();
            this.BackgroundImage?.Dispose();
            base.OnFormClosed(e);
        }
    }
}