namespace RPG_Sim
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        private void InitializeComponent()
        {
            // This line is CRUCIAL
            this.components = new System.ComponentModel.Container();

            // These will likely be overridden by InitializeCustomComponents, which is fine.
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450); // Default, will be changed by InitializeCustomComponents
            this.Text = "Form1"; // Default, will be changed by InitializeCustomComponents
        }

        #endregion
    }
}