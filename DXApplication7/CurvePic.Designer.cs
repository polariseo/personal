namespace DXApplication7
{
    partial class CurvePic
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
            this.CurveBox.Location = new System.Drawing.Point(1, -3);
            this.CurveBox.Name = "CurveBox";
            this.CurveBox.Size = new System.Drawing.Size(400, 361);
            this.CurveBox.TabIndex = 0;
            this.CurveBox.TabStop = false;
            this.CurveBox.Paint += new System.Windows.Forms.PaintEventHandler(this.CurveBox_Paint);
            // 
            // CurvePic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 355);
            this.Controls.Add(this.CurveBox);
            this.Name = "CurvePic";
            this.Text = "灰度曲线";
            this.Load += new System.EventHandler(this.CurvePic_Load);
            this.Shown += new System.EventHandler(this.CurvePic_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.CurveBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox CurveBox;
    }
}