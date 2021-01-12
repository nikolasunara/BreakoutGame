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
            ((System.ComponentModel.ISupportInitialize)(this.ball)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player)).BeginInit();
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
            this.scoreText.Location = new System.Drawing.Point(12, 9);
            this.scoreText.Name = "scoreText";
            this.scoreText.Size = new System.Drawing.Size(70, 25);
            this.scoreText.TabIndex = 3;
            this.scoreText.Text = "label1";
            // 
            // ball
            // 
            this.ball.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ball.Image = global::BreakoutGame.Properties.Resources.circle_cropped;
            this.ball.Location = new System.Drawing.Point(335, 268);
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
            this.player.Location = new System.Drawing.Point(277, 481);
            this.player.Name = "player";
            this.player.Size = new System.Drawing.Size(142, 20);
            this.player.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.player.TabIndex = 1;
            this.player.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(738, 513);
            this.Controls.Add(this.scoreText);
            this.Controls.Add(this.ball);
            this.Controls.Add(this.player);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Blackout ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyisdown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyisup);
            ((System.ComponentModel.ISupportInitialize)(this.ball)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.PictureBox player;
		private System.Windows.Forms.PictureBox ball;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Label scoreText;
    }
}

