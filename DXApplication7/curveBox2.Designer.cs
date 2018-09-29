namespace DXApplication7
{
    partial class curveBox2
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
            this.CurveBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CurveBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CurveBox
            // 
            this.CurveBox.Location = new System.Drawing.Point(-1, 2);
            this.CurveBox.Name = "CurveBox";
            this.CurveBox.Size = new System.Drawing.Size(408, 333);
            this.CurveBox.TabIndex = 0;
            this.CurveBox.TabStop = false;
            this.CurveBox.Paint += new System.Windows.Forms.PaintEventHandler(this.CurveBox_Paint);
            // 
            // curveBox2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 334);
            this.Controls.Add(this.CurveBox);
            this.Name = "curveBox2";
            this.Text = "curveBox2";
            this.Load += new System.EventHandler(this.curveBox2_Load);
            this.Shown += new System.EventHandler(this.curveBox2_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.curveBox2_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.CurveBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox CurveBox;
    }
}