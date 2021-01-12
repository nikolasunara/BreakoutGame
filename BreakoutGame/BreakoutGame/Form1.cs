using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BreakoutGame
{
	public partial class Form1 : Form
	{
		bool goLeft;
		bool goRight;
		bool isGameOver;

		int score;
		int ballX;
		int ballY;
		int playerSpeed;

		PictureBox[] blockArray;

		Random rnd = new Random();


		public Form1()
		{
			InitializeComponent();

			placeBlocks();
		}

		private void setupGame() 
		{
			isGameOver = false;
			score = 0;
			ballX = 5;
			ballY = -5;
			playerSpeed = 12;
			scoreText.Text = "Score: " + score;

			ball.Left = 345;
			ball.Top = 250;

			player.Left = 320;

			gameTimer.Start();

			foreach(Control x in this.splitContainer1.Panel2.Controls)
			{
				if(x is PictureBox && x.Tag is "blocks")
				{
					x.BackColor = Color.Gray;
					x.BackgroundImage = Properties.Resources.redBrick;
				}
			}
		}

		private void placeBlocks()
		{
			blockArray = new PictureBox[24];

			int a = 0;
			int top = 50;
			int left = 60;

			for(int i = 0; i < blockArray.Length; ++i)
			{
				blockArray[i] = new PictureBox();
				blockArray[i].Height = 32;
				blockArray[i].Width = 64;
				blockArray[i].Tag = "blocks";
				blockArray[i].BackColor = Color.White;

				if(a == 8)
				{
					top = top + 50;
					left = 60;
					a = 0;
				}
				if(a < 8)
				{
					a++;
					blockArray[i].Left = left;
					blockArray[i].Top = top;
					//this.Controls.Add(blockArray[i]);
					this.splitContainer1.Panel2.Controls.Add(blockArray[i]);
					
					left = left + 80; 
				}
			}

			setupGame();
		}

		private void removeBlocks()
		{
			foreach(PictureBox x in blockArray)
			{
				this.Controls.Remove(x);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void gameOver(String message)
		{
			isGameOver = true;
			gameTimer.Stop();
			scoreText.Text = "Score: " + score + " " + message;
		}

		private void mainGameTimerEvent(object sender, EventArgs e)
		{
			scoreText.Text = "Score: " + score;

			if(goLeft == true && player.Left > 0)
			{
				player.Left -= playerSpeed;
			}
			if (goRight == true && player.Right < splitContainer1.Panel2.Width - 7) 
			{
				player.Left += playerSpeed;
			}

			ball.Left += ballX;
			ball.Top += ballY;

			if(ball.Left < 0 || ball.Right > splitContainer1.Panel2.Width)
			{
				ballX = -ballX;
			}
			if(ball.Top < 0)
			{
				ballY = -ballY;
			}
			if(ball.Bounds.IntersectsWith(player.Bounds))
			{
				ballY = -ballY;
			}

			foreach (Control x in this.splitContainer1.Panel2.Controls)
			{
				if (x is PictureBox && x.Tag is "blocks")
				{
					if(ball.Bounds.IntersectsWith(x.Bounds))
					{
						score += 1;
						ballY = -ballY;

						this.splitContainer1.Panel2.Controls.Remove(x);
					}
				}
			}

			if(ball.Top > player.Top)
			{
				gameOver("Game over! Press Enter to play again.");
			}
		}

		private void keyisdown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Left)
			{
				goLeft = true;
			}
			if (e.KeyCode == Keys.Right)
			{
				goRight = true;
			}
		}

		private void keyisup(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left)
			{
				goLeft = false;
			}
			if (e.KeyCode == Keys.Right)
			{
				goRight = false;
			}
			if (e.KeyCode == Keys.Enter && isGameOver == true)
			{
				removeBlocks();
				placeBlocks();
			}
		}

        private void scoreText_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void player_Click(object sender, EventArgs e)
        {

        }
    }
}
