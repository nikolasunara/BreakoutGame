using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreakoutGame
{
    public partial class Form1 : Form
	{
		bool goLeft;
		bool goRight;
		bool isGameOver;

		int score;
		int playerSpeed;
		double ball_speed;

		//Ove varijable sluze za pokretanje loptice. Pomicemo ju tako da
		//poziciji loptice dodamo ballX s lijeve, odnosno ballY s gornje strane.
		//Uvijek vrijedi ballX^2 + ballY^2 = ball_speed^2
		double ballX;
		double ballY;

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
			playerSpeed = 12;
			scoreText.Text = "Score: " + score;
			textBox1.Text = "Press SPACE to start the game";

			//na pocetku je loptica nepomicna, tj. stoji na ploci
			ball_speed = 0;
			ballX = 0;
			ballY = 0;

			//namjesti plocu na sredinu
			player.Left = (int)(splitContainer1.Panel2.Width / 2 - player.Width / 2);
			//namjesti lopticu na sredinu ploce
			ball.Left = player.Left + player.Width / 2 - ball.Width/2;
			ball.Top = player.Top - ball.Height;

			gameTimer.Start();

			foreach(Control x in this.splitContainer1.Panel2.Controls)
			{
				if(x is PictureBox && x.Tag is "blocks")
				{
					x.BackColor = Color.Gray;
					x.BackgroundImage = Properties.Resources.redBrick;
					x.BackgroundImageLayout = ImageLayout.Stretch;
				}
			}
		}

		private void placeBlocks()
		{
			//postavljanje 30 blokova u 3 reda
			blockArray = new PictureBox[30];

			int in_row = 0; 
			int top = 40;
			int left = 1;
			int width = (int)(splitContainer1.Panel2.Width - 14) / 10;

			for(int i = 0; i < blockArray.Length; ++i)
			{
				blockArray[i] = new PictureBox();
				blockArray[i].Height = 32;
				blockArray[i].Width = width;
				blockArray[i].Tag = "blocks";
				blockArray[i].BackColor = Color.White;

				if(in_row == 10)
				{
					top = top + 34;
					left = 1;
					in_row = 0;
				}
				if(in_row < 10)
				{
					in_row++;
					blockArray[i].Left = left;
					blockArray[i].Top = top;
					this.splitContainer1.Panel2.Controls.Add(blockArray[i]);
					
					left = left + width + 1; 
				}
			}

			setupGame();
		}

		private void removeBlocks()
		{
			foreach(PictureBox x in blockArray)
			{
				this.splitContainer1.Panel2.Controls.Remove(x);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void gameOver(String message)
		{
			isGameOver = true;
			gameTimer.Stop();
			scoreText.Text = "Score: " + score;
			textBox1.Text = message;
		}

		private void mainGameTimerEvent(object sender, EventArgs e)
		{
			scoreText.Text = "Score: " + score;

			if(goLeft == true && player.Left > 0)
			{
				player.Left -= playerSpeed;
				if (ball_speed == 0.0)
					ball.Left -= playerSpeed;
			}
			if (goRight == true && player.Right < splitContainer1.Panel2.Width - 7) 
			{
				player.Left += playerSpeed;
				if (ball_speed == 0.0)
					ball.Left += playerSpeed;
			}

			//pomakni lopticu
			ball.Left += (int)ballX;
			ball.Top += (int)ballY;
			
			if (ball.Left < 0 || ball.Right > splitContainer1.Panel2.Width)
			{
				ballX = -ballX;
			}
			if(ball.Top < 0)
			{
				ballY = -ballY;
			}

			//Lopta udara u igraca
			if(ball.Bounds.IntersectsWith(player.Bounds))
			{
				ballY = -ballY;
				//pozicija gdje loptica udara o plocu
				double pos = ball.Width / 2 + ball.Left;
	
				double sredina_ploce = player.Left + player.Width / 2;
				double omjer = 2 * (pos - sredina_ploce) / player.Width;

				//mapiranje intervala [-1,1]->[PI,0]
				double kut = Math.PI - (omjer + 1) * Math.PI / 2;
				//nedaj kut manji od PI/7
				kut = Math.Abs(kut) < Math.PI / 7 ? Math.PI / 7 : kut;

				ballY = -Math.Sin(kut) * ball_speed;
				ballX = Math.Cos(kut) * ball_speed;
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
			else if (e.KeyCode == Keys.Right)
			{
				goRight = true;
			}
			else if (e.KeyCode == Keys.Space && ball_speed == 0)
            {
				ball_speed = 9;
				//odredi kut pod kojim ce loptica biti ispaljena
				double kut = 0.3 + rnd.NextDouble() * (2.8 - 0.3);
				ballY = -Math.Sin(kut) * ball_speed;
				ballX = Math.Cos(kut) * ball_speed;
				textBox1.Text = "";
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

        private void Player_Click(object sender, EventArgs e)
        {

        }
    }
}
