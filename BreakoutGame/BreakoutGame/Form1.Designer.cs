namespace BreakoutGame
{
	partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.scoreText = new System.Windows.Forms.Label();
            this.ball = new System.Windows.Forms.PictureBox();
            this.player = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.ball)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameTimer
            // 
            this.gameTimer.Interval = 20;
            this.gameTimer.Tick += new System.EventHandler(this.mainGameTimerEvent);
            // 
            // scoreText
            // 
            this.scoreText.AutoSize = true;
            this.scoreText.Location = new System.Drawing.Point(3, 12);
            this.scoreText.Name = "scoreText";
            this.scoreText.Size = new System.Drawing.Size(86, 31);
            this.scoreText.TabIndex = 3;
            this.scoreText.Text = "label1";
            this.scoreText.Click += new System.EventHandler(this.scoreText_Click);
            // 
            // ball
            // 
            this.ball.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ball.Image = global::BreakoutGame.Properties.Resources.circle_cropped;
            this.ball.Location = new System.Drawing.Point(3, 435);
            this.ball.Name = "ball";
            this.ball.Size = new System.Drawing.Size(22, 20);
            this.ball.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ball.TabIndex = 2;
            this.ball.TabStop = false;
            // 
            // player
            // 
            this.player.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.player.Image = global::BreakoutGame.Properties.Resources.panel1;
            this.player.Location = new System.Drawing.Point(301, 530);
            this.player.Name = "player";
            this.player.Size = new System.Drawing.Size(142, 20);
            this.player.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.player.TabIndex = 1;
            this.player.TabStop = false;
            this.player.Click += new System.EventHandler(this.player_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.AllowDrop = true;
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Enabled = false;
            this.splitContainer1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.splitContainer1.Location = new System.Drawing.Point(-1, -3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.scoreText);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitContainer1.Panel2.Controls.Add(this.ball);
            this.splitContainer1.Panel2.Controls.Add(this.player);
            this.splitContainer1.Panel2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(903, 597);
            this.splitContainer1.SplitterDistance = 165;
            this.splitContainer1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(901, 580);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximumSize = new System.Drawing.Size(919, 627);
            this.MinimumSize = new System.Drawing.Size(919, 627);
            this.Name = "Form1";
            this.Text = "Blackout ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyisdown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyisup);
            ((System.ComponentModel.ISupportInitialize)(this.ball)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.PictureBox player;
		private System.Windows.Forms.PictureBox ball;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Label scoreText;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

