namespace DXApplication7
{
    partial class ZernikeToInterfere
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Interfere = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Interfere)).BeginInit();
            this.SuspendLayout();
            // 
            // Interfere
            // 
            this.Interfere.Location = new System.Drawing.Point(16, 15);
            this.Interfere.Name = "Interfere";
            this.Interfere.Size = new System.Drawing.Size(252, 230);
            this.Interfere.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Interfere.TabIndex = 1;
            this.Interfere.TabStop = false;
            // 
            // ZernikeToInterfere
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.Interfere);
            this.Name = "ZernikeToInterfere";
            this.Text = "标准Zernike像差干涉图";
            this.Load += new System.EventHandler(this.ZernikeToInterfere_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Interfere)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Interfere;
    }
}