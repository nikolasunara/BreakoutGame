using System;
using System.Collections.Generic;
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

		int counter;            //broji vrijeme igre
		int time_to_shift;      //broji vrijeme do pomaka kocki prema dolje	
		double ball_speed;

		int lowest;				//prati poziciju najdonje kocke

		//Ove varijable sluze za pokretanje loptice. Pomicemo ju tako da
		//poziciji loptice dodamo ballX s lijeve, odnosno ballY s gornje strane.
		//Uvijek vrijedi ballX^2 + ballY^2 = ball_speed^2
		double ballX;
		double ballY;

		//PictureBox[] blockArray;
		List<PictureBox> blockArray = new List<PictureBox>();

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
			counter = 0;
			lowest = 0;
			time_to_shift = 0;
			playerSpeed = 12;
			scoreText.Text = "Score: " + score;
			textBox1.Text = "Press SPACE to start the game";
			label2.Text = "00:00";

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
		}

		private void placeBlocks()
		{
			// za pocetak nacrtaj 3 reda
			draw_rows(3);
			//pripremi igru
			setupGame();
		}
		private void draw_rows(int n) //crta n redova kocki, u svakom redu uvijek 10 kocki
        {
			int top = 5;
			int left = 1;
			int width = (int)(splitContainer1.Panel2.Width - 14) / 10;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 10; j++)
                {
					//stvori blok i postavi mu svojstva
					var block = new PictureBox();
					block.Height = 32;
					block.Width = width;
					//block.Tag = "blocks";
					block.BackColor = Color.White;
					block.Left = left;
					block.Top = top;

					block.BackColor = Color.Gray;
					/*
					 * Crvena cigla: obicna cigla koja puca nakon prvog udarca. 
					 * Zuta cigla: obicna cigla koja puca nakon prvog udarca.
					 * Tamnozelena cigla: nakon prvog udarca napukne, nakog drugog skroz puca. Nosi vise bodova.
					 * Purpurna cigla: kad se razbija dogodi se neki efekt. 
					 * 
					 * 
					 * Uzimamo random broj izmedu 0 i 1. Ako je u intervalu [0, 0.35] onda stvaramo 
					 * zutu ciglu, ako je u <0.35, 0.7] onda crvenu, ako je u <0.7, 0.9] stvaramo 
					 * tamnozelenu, a inace (<0.9, 1]) crvenu.
					 * Korigirat ove brojeve u testnoj fazi.
					 */
					double odluka_boje = rnd.NextDouble();
					if(odluka_boje <= 0.35)
                    {
						block.BackgroundImage = Properties.Resources.yellowBrick;
						block.Tag = new Block { blockColor = "yellow" };
					}
					else if (odluka_boje <= 0.7)
                    {
						block.BackgroundImage = Properties.Resources.redBrick2;
						block.Tag = new Block { blockColor = "red" };
					}
					else if(odluka_boje <= 0.9)
                    {
						block.BackgroundImage = Properties.Resources.darkGreenBrick;
						block.Tag = new Block { blockColor = "darkGreen" };

					}
					else
                    {
						block.BackgroundImage = Properties.Resources.purpleBrick;
						block.Tag = new Block { blockColor = "purple" };

					}

					block.BackgroundImageLayout = ImageLayout.Stretch;

					blockArray.Add(block);
					this.splitContainer1.Panel2.Controls.Add(block);

					left += width;
				}
				left = 1;
				top += 33;
				if (top > lowest)
					lowest = top;
            }
		}

		private void removeBlocks()
		{
			foreach(PictureBox x in blockArray)
			{
				this.splitContainer1.Panel2.Controls.Remove(x);
			}
			blockArray.Clear();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void gameOver()
		{
			isGameOver = true;
			gameTimer.Stop();
			timer1.Stop();
			scoreText.Text = "Score: " + score;
			textBox1.Text = "Game over! Press Enter to play again.";
		}

		private void mainGameTimerEvent(object sender, EventArgs e)
		{
			scoreText.Text = "Score: " + score;

			//pomakni plocu ako je pritisnuta tipka za lijevo ili desno
			if (goLeft == true && player.Left > 0)
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
			
			//Lopta udara u rub prozora
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
			
			// Provjeri dodiruje li lopta neku ciglu
			foreach (Control x in this.splitContainer1.Panel2.Controls)
			{
				if (x is PictureBox && x.Tag is Block)
				{
					if(ball.Bounds.IntersectsWith(x.Bounds))
					{
						Block block = (Block)x.Tag;
                        if (block.blockColor == "darkGreen")
                        {
							//Cigla je napukla.
							x.BackgroundImage = Properties.Resources.brokenDarkGreenBrick;
							x.Tag = new Block { blockColor = "brokenDarkGreen" };
							//this.splitContainer1.Panel2.Controls.

							ballY = -ballY;
						}
                        else
                        {
							//Razbili smo ciglu.
							if(block.blockColor == "brokenDarkGreen")
                            {
								//Promijeniti po zelji. Drugi udarac u ciglu, pa malo veca nagrada.
								score += 3;
                            }
							else 
								score += 1;
							
							ballY = -ballY;

							this.splitContainer1.Panel2.Controls.Remove(x);

							// azuriraj lowest
							foreach (var t in blockArray)
								/*if (t.Top == x.Top && t.Left == x.Left)
									blockArray.Remove(t);
								else*/
								lowest = (t.Top > lowest) ? t.Top : lowest;
						}
					}
				}
			}

			if(ball.Top > player.Top)
			{
				gameOver();
			}

			//ako je proslo bar 5 sek od zadnjeg dodavanja probaj dodat novi red na vrh
			//poslije cemo staviti vece
			if (time_to_shift > 20)
			{
				//prvo provjeri moze li se pomaknuti 
				foreach (var x in blockArray)
				{
					//Rectangle rect = new Rectangle(x.Left, x.Top + , x.Width, 32);
					if (ball.Bounds.IntersectsWith(x.Bounds))
						return;

				}
				//ako je doslo do tu znaci da se moze pomaknuti
				foreach (var x in blockArray)
				{
					x.Top += 33;
					lowest += 33;
					if (x.Top + 33 > player.Top)
						gameOver();
				}
				draw_rows(1);
				time_to_shift = 0;
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
			else if (e.KeyCode == Keys.Space && ball_speed == 0)    //pokrece igru
            {
				ball_speed = 10;
				//odredi kut pod kojim ce loptica biti ispaljena
				double kut = 0.3 + rnd.NextDouble() * (2.8 - 0.3);
				ballY = -Math.Sin(kut) * ball_speed;
				ballX = Math.Cos(kut) * ball_speed;
				textBox1.Text = "";

				//zapocni timer u za igru u sekundama
				timer1.Start();
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
		//broji vrijeme u sekundama
        private void timer1_Tick(object sender, EventArgs e)
        {
			counter++;
			int seconds = counter % 60;
			int minutes = counter / 60;
			label2.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

			time_to_shift++;
			
		}
    }
}
