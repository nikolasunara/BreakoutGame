using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

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
		
		double ball_speed;		//trenutna brzina loptice
		double standard_ball_speed;  //uvijek standardna brzina
		int fast_slow_time;

		int lowest;				//prati poziciju najdonje kocke

		//Ove varijable sluze za pokretanje loptice. Pomicemo ju tako da
		//poziciji loptice dodamo ballX s lijeve, odnosno ballY s gornje strane.
		//Uvijek vrijedi ballX^2 + ballY^2 = ball_speed^2
		double ballX;
		double ballY;

		//PictureBox[] blockArray;
		List<PictureBox> blockList = new List<PictureBox>();

		//Lista posebnih efekata. Ideja je naslijediti PictureBox u klasama 
		//Block i Effect. Za sad samo postavljamo te klasekao Tag. Ako se pokaze kao
		//nepotrebno, kasnije cu maknuti/doraditi.
		//List<Effect> effectsList = new List<Effect>();
		List<PictureBox> effectList = new List<PictureBox>();

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
			fast_slow_time = 0;
			playerSpeed = 12;
			scoreText.Text = "Score: " + score;
			textBox1.Text = "Press SPACE to start the game";
			label2.Text = "00:00";

			//na pocetku je loptica nepomicna, tj. stoji na ploci
			ball_speed = 0;
			standard_ball_speed = 0;
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
					 * Purpurna cigla: kad se razbija dogodi se neki padajuci efekt - fast, slow, +50, +100... 
					 * Destroy cigla - unisti svoje susjede
					 * NewBall cigla - stvara neki broj novih loptica.
					 * 
					 * Uzimamo random broj izmedu 0 i 1. Ako je u intervalu [0, 0.30] onda stvaramo 
					 * zutu ciglu, ako je u <0.30, 0.6] onda crvenu, ako je u <0.6, 0.75] stvaramo 
					 * tamnozelenu, a inace (<0.75, 0.85]) crvenu. Za <0.85, 0.93] Destroy cigla, a
					 * <0.93, 1] NewBall cigla.
					 * Korigirat ove brojeve u testnoj fazi.
					 * 
					 * Ovi brojevi trenutno promijenjeni radi testiranja.
					 */

					double odluka_boje = rnd.NextDouble();
					bool is_effect = false;
					if(odluka_boje <= 0.10)
                    {
						block.BackgroundImage = Properties.Resources.yellowBrick;
						block.Tag = new Block { blockColor = "yellow" };
					}
					else if (odluka_boje <= 0.2) //0.6
                    {
						block.BackgroundImage = Properties.Resources.redBrick2;
						block.Tag = new Block { blockColor = "red" };
					}
					else if(odluka_boje <= 0.3) //0.75
                    {
						block.BackgroundImage = Properties.Resources.darkGreenBrick;
						block.Tag = new Block { blockColor = "darkGreen" };

					}
					else if(odluka_boje <= 0.85) // 0.85
                    {
						block.BackgroundImage = Properties.Resources.purpleBrick;
						block.Tag = new Block { blockColor = "purple" };

					}
					else if (odluka_boje <= 0.95)
                    {
						block.BackgroundImage = Properties.Resources.destroy;
						block.Tag = new Effect { Mobile = false, Description = "destroy" };
						is_effect = true;
					}
                    else
                    {
						block.BackgroundImage = Properties.Resources.newBall;
						block.Tag = new Effect { Mobile = false, Description = "newBall" };
						is_effect = true;
					}

					block.BackgroundImageLayout = ImageLayout.Stretch;
					
					//Malo nezgodna notacija "block". Ako je nastao efekt newBall ili desroy spremamo
					//u listu efekata.
					if (!is_effect)
						blockList.Add(block);
					else 
						effectList.Add(block);

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
			foreach(PictureBox x in blockList)
			{
				this.splitContainer1.Panel2.Controls.Remove(x);
			}
			blockList.Clear();
			foreach (PictureBox x in effectList)
			{
				this.splitContainer1.Panel2.Controls.Remove(x);
			}
			effectList.Clear();
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
			//Ako je lopta išla prema lijevo i udarila u lijevi rub, odbija se(isto za gore i desno),
			if ( (ball.Left < 0 && ballX < 0) || (ball.Right > splitContainer1.Panel2.Width && ballX > 0) )
			{
				ballX = -ballX;
			}
			if(ball.Top < 0 && ballY < 0)
			{
				ballY = -ballY;
			}

			//Pomicemo padajuce efekte. Dodati slucaj kada udari u igracevu plocu. 
			//Loptica samo prolazi kroz efekte.
			foreach (PictureBox ef in effectList.ToList())
			{
				Effect effectTag = (Effect)ef.Tag;
				//Imamo dvije vrste efekata. Staticni su oni koji stoje na pozicijama kao cigle (destroy i newBall).
				//Padajuci efekti su bonusi na bodove, te usporavanje i ubrzavanje loptice. Potrebno ih je realizirati.
				if (!effectTag.Mobile) continue;
				
				ef.Top += 10;
				if (ef.Top + 10 > player.Top)
				{
					this.splitContainer1.Panel2.Controls.Remove(ef);
					effectList.Remove(ef);
				}
				else if (ef.Bounds.IntersectsWith(player.Bounds))
				{
					// Igrac je pokupio efekt
					// Imamo 4 slučaja:
					if (effectTag.Description == "bonus50")
					{
						score += 50;
						this.splitContainer1.Panel2.Controls.Remove(ef);
						effectList.Remove(ef);
					}
					else if (effectTag.Description == "bonus100")
					{
						score += 100;
						this.splitContainer1.Panel2.Controls.Remove(ef);
						effectList.Remove(ef);
					}
					else if (effectTag.Description == "fast")
					{
						//postavi fast_slow_time tako da se pokrene timer1
						//treba namistiti ballX i ballY tako da daju ball_speed^2
						//ali tu treba paziti da kut ostane isti
						fast_slow_time = 1;
						ball_speed = (int)(1.4 * standard_ball_speed);
						this.splitContainer1.Panel2.Controls.Remove(ef);
						effectList.Remove(ef);
					}
					else if (effectTag.Description == "slow")
					{
						fast_slow_time = 1;
						ball_speed = (int)(0.7 * standard_ball_speed);
						this.splitContainer1.Panel2.Controls.Remove(ef);
						effectList.Remove(ef);
					}
				}
			}

			//Lopta udara u igraca
			if (ball.Bounds.IntersectsWith(player.Bounds))
			{
				//pozicija gdje loptica udara o plocu
				double pos = ball.Width / 2 + ball.Left;
	
				double sredina_ploce = player.Left + player.Width / 2;
				double omjer = 2 * (pos - sredina_ploce) / player.Width;

				//korigiranje rubnih slučajeva
				omjer = (omjer < -1) ? -1 : omjer;
				omjer = (omjer > 1) ? 1 : omjer;
				//mapiranje intervala [-1,1]->[PI/4,-PI/4]
				//koliko će mjesto sudaranja lopte i ploče utjecati na
				//kut odbijanja lopte
				double kut = omjer * (- Math.PI / 4);
				
				//kut_2 je "klasični kut odbijanja lopte od neke podloge"
				double kut_2 = 0;
				if (ballX < 0 && ballY > 0)
					kut_2 = Math.PI - Math.Atan(ballY / -ballX);
				else if (ballX > 0 && ballY > 0)
					kut_2 = Math.Atan(ballY / ballX);
				else if (ballX == 0)
					kut_2 = Math.PI / 2;
				
				//konačni kut odbijanja lopte od ploču je zbroj "klasičnog kuta"
				//i utjecaja mjesta gdje se lopta udarila s pločom
				kut = kut + kut_2;

				//nedaj kut manji od PI/7 ili veći od 6PI/7
				kut = Math.Abs(kut) < Math.PI / 7 ? Math.PI / 7 : kut;
				kut = Math.Abs(kut) > 6 * Math.PI / 7 ? 6 * Math.PI / 7 : kut;

				ballY = -Math.Sin(kut) * ball_speed;
				ballX = Math.Cos(kut) * ball_speed;

			}
			
			// Provjeri dodiruje li lopta neku ciglu
			foreach (Control x in this.splitContainer1.Panel2.Controls)
			{
                //Gledamo presjek lopte s ciglama.
                if (ball.Bounds.IntersectsWith(x.Bounds) && x is PictureBox)
				{
					//funkcija koja unistava ciglu x
					destroyBlock(x);
					//preusmjeri lopticu tj. promijeni ballX ili ballY
					// jos uvijek se ne odbija savrseno ali bar je puno bolje nego prije
					if (x.Tag is Block)
                    {
						//provjeravanje s koje strane lopta dolazi
						//trazimo centar lopte te usporedujemo
						int ball_center_X = ball.Left + (int)(ball.Width / 2);
						int ball_center_Y = ball.Top + (int)(ball.Height / 2);

						if (ball_center_Y > x.Top + x.Height || ball_center_Y < x.Top)
							//dolazi s gornje ili donje strane
							ballY = -ballY;
						else
							ballX = -ballX;
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
				foreach (var x in blockList)
				{
					//Rectangle rect = new Rectangle(x.Left, x.Top + , x.Width, 32);
					if (ball.Bounds.IntersectsWith(x.Bounds))
						return;

				}
				foreach (var x in effectList)
				{
					Effect effectTag = (Effect)x.Tag;
					//Rectangle rect = new Rectangle(x.Left, x.Top + , x.Width, 32);
					if ( !effectTag.Mobile &&  ball.Bounds.IntersectsWith(x.Bounds))
						return;

				}

				//ako je doslo do tu znaci da se moze pomaknuti
				foreach (var x in blockList)
				{
					x.Top += 33;
					lowest += 33;
					if (x.Top + 33 > player.Top)
						gameOver();
				}
				foreach (var x in effectList)
				{
					Effect effectTag = (Effect)x.Tag;
                    if (!effectTag.Mobile)
                    {
						x.Top += 33;
						lowest += 33;
						if (x.Top + 33 > player.Top)
							gameOver();
					}
					
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
				ball_speed = 12;
				standard_ball_speed = ball_speed;
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

			fast_slow_time++;

			// Ako je time_to_shift > 0 znaci da je pokupljen efekt za brzu ili sporu loptu
			if (fast_slow_time != 0)
				fast_slow_time++;
			if (fast_slow_time > 15)
			{   //ako je proslo 15 sekundi ugasi ga
				fast_slow_time = 0;
				ball_speed = standard_ball_speed;
			}
		}

		//unistava ciglu x te kreira padajuci efekt ako je potrebno
		private void destroyBlock(Control x)
        {
			if (x.Tag is Block)
			{
				Block blockTag = (Block)x.Tag;
				if (blockTag.blockColor == "darkGreen")
				{
					//Cigla je napukla.
					x.BackgroundImage = Properties.Resources.brokenDarkGreenBrick;
					x.Tag = new Block { blockColor = "brokenDarkGreen" };
				}
				else
				{
					//Razbili smo ciglu.
					if (blockTag.blockColor == "brokenDarkGreen")
					{
						//Promijeniti po zelji. Drugi udarac u ciglu, pa malo veca nagrada.
						score += 50;
					}
					else
						score += 10;


					//Mozda i maknuti iz liste cigli, ne samo iz kontrola?
					this.splitContainer1.Panel2.Controls.Remove(x);


					//Ako smo pogidili ciglu koja stvara padajuci efekt, ovdje ga stvaramo.
					//Od njega se loptica ne odbija vec samo prolazi preko njega. Cilj ga je 
					//skupiti igracom plocom.
					if (blockTag.blockColor == "purple")
					{
						//Sirina bloka.
						int width = (int)(splitContainer1.Panel2.Width - 14) / 10;
						//stvori blok i postavi mu svojstva
						var effect = new PictureBox();

						effect.Height = 32;
						effect.Width = width;
						effect.BackColor = Color.White;
						//Stvaramo padajuci efekt na mjestu razbijene cigle x.
						effect.Left = x.Left;
						effect.Top = x.Top;


						double effect_decision = rnd.NextDouble();
						if (effect_decision <= 0.3)
						{
							//Stvaramo efekt +50.
							effect.BackgroundImage = Properties.Resources.bonus50;
							effect.Tag = new Effect { Mobile = true, Description = "bonus50" };
						}
						else if (effect_decision <= 0.5)
						{
							//Stvaramo efekt +100.
							effect.BackgroundImage = Properties.Resources.bonus100;
							effect.Tag = new Effect { Mobile = true, Description = "bonus100" };
						}
						else if (effect_decision <= 0.75)
						{
							//Stvaramo efekt Slow - usporavanje loptice za neki (odrediti) koeficijent.

							effect.BackgroundImage = Properties.Resources.slow;
							effect.Tag = new Effect { Mobile = true, Description = "slow" };
						}
						else
						{
							//Stvaramo efekt Fast - ubrzanje loptice za neki (odrediti) koeficijent.
							effect.BackgroundImage = Properties.Resources.fast;
							effect.Tag = new Effect { Mobile = true, Description = "fast" };
						}

						effect.BackgroundImageLayout = ImageLayout.Stretch;
						effectList.Add(effect);
						this.splitContainer1.Panel2.Controls.Add(effect);

					}
				}
			}
			else if (x.Tag is Effect)
            {
				//Ovdje dolazi imeplementacija efekata i njihovo unistavanje
				//Padajuce efekte dodir s lopticom ne smeta, samo prolazi kroz nju. Oni se skupljaju
				//pomocu igrace ploce.
				//Staticke efekte skupljamo kad ih loptica pogodi. Tu za sad ide samo implementacija da oni
				//nestanu kad ih se pogodi, no potrebna je jos realizacija.
				Effect effectTag = (Effect)x.Tag;
				if (!effectTag.Mobile)
				{
					//Dodati jos brisanje iz liste efekata, kao sto treba i za cigle.
					if (effectTag.Description == "destroy")
					{
						this.splitContainer1.Panel2.Controls.Remove(x);
						destroySurroundingBlocks(x); //unistava okolne cigle
					}
					else if (effectTag.Description == "newBall")
					{
						//dodati stvaranje novih loptice
						this.splitContainer1.Panel2.Controls.Remove(x);
					}
				}
			}
		}

		//Funkcija uzima ciglu x i unistava cigle koje je okruzuju
		private void destroySurroundingBlocks(Control x)
		{
			
			var rect = new Rectangle(x.Left - 2, x.Top - 2 ,x.Width + 5, x.Height + 5);
			foreach (Control c in this.splitContainer1.Panel2.Controls)
			{
				if (c is PictureBox && (c.Tag is Block || c.Tag is Effect))
				{
					var c_rect = new Rectangle(c.Left, c.Top, c.Width, c.Height);
					if (c_rect.IntersectsWith(rect))
						destroyBlock(c);

				}
			}
		}

	}
}
