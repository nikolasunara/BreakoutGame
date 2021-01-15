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

		double ball_speed;      //trenutna brzina loptice
		double standard_ball_speed;  //uvijek standardna brzina
		int fast_slow_time;

		int lowest;             //prati poziciju najdonje kocke

		//Ove varijable sluze za pokretanje loptice. Pomicemo ju tako da
		//poziciji loptice dodamo ballX s lijeve, odnosno ballY s gornje strane.
		//Uvijek vrijedi ballX^2 + ballY^2 = ball_speed^2
		
		//Postavljamo stvari za situaciju s vise loptica u igri. Umjesto jedne loptice, imat cemo niz loptica
		//(najcesce ce u nizu biti samo jedna loptica, osim kada pokupimo efekt za vise njih).
		List<double> ballXList = new List<double>();
		List<double> ballYList = new List<double>();
		List<PictureBox> ballList = new List<PictureBox>();

		//PictureBox[] blockArray;
		List<PictureBox> blockList = new List<PictureBox>();

		//Lista posebnih efekata. Ideja je naslijediti PictureBox u klasama 
		//Block i Effect. Za sad samo postavljamo te klase kao Tag. Ako se pokaze kao
		//nepotrebno, kasnije cu maknuti/doraditi.
		//List<Effect> effectsList = new List<Effect>();
		List<PictureBox> effectList = new List<PictureBox>();

		Random rnd = new Random();


		//zvukovi
		//SoundPlayer moze pustati samo jedan zvuk istovremeno i to radi
		// u zasebnoj dretvi tako da se javlja bug kad se razbije vise cigli odjednom
		System.Media.SoundPlayer explosionSound = new System.Media.SoundPlayer(Properties.Resources.explosion);
		//System.Media.SoundPlayer brickSound = new System.Media.SoundPlayer(Properties.Resources.brick_shot);
		//System.Media.SoundPlayer newballsSound = new System.Media.SoundPlayer(Properties.Resources.newball1);
		//System.Media.SoundPlayer brokenSound = new System.Media.SoundPlayer(Properties.Resources.broken);

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
			textBox1.Text = "CLICK where you want to send the ball";
			label2.Text = "00:00";


			//namjesti plocu na sredinu
			player.Left = (int)(splitContainer1.Panel2.Width / 2 - player.Width / 2);

			//na pocetku je loptica nepomicna, tj. stoji na ploci
			ball_speed = 0;
			standard_ball_speed = 0;
			
			//Sve loptce ce imati istu brzinu u svakom trenutku.
			//Stvaramo pocetnu lopticu.
			var ballFirst = new PictureBox();
			ballFirst.Height = 26;
			ballFirst.Width = 26;
			ballFirst.BackColor = SystemColors.ControlDarkDark;
			//Ako zelimo smjestiti lopticu na sredinu ploce 
			//ballFirst.Left = (int)(splitContainer1.Panel2.Width) / 2  - ballFirst.Width / 2;
			//ballFirst.Top = (int)(splitContainer1.Panel2.Height) / 2  - ballFirst.Height / 2;
			ballFirst.Left = player.Left + player.Width / 2 - ballFirst.Width / 2;
			ballFirst.Top = player.Top - ballFirst.Height;
			ballFirst.BackgroundImage = Properties.Resources.circle_cropped;
			ballFirst.BackgroundImageLayout = ImageLayout.Stretch;
			ballList.Add(ballFirst);
			this.splitContainer1.Panel2.Controls.Add(ballFirst);
			ballXList.Add(0);
			ballYList.Add(0);


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
					else if(odluka_boje <= 0.75) // 0.85
                    {
						block.BackgroundImage = Properties.Resources.purpleBrick;
						block.Tag = new Block { blockColor = "purple" };

					}
					else if (odluka_boje <= 0.85) //0.95
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
                {
					//Ovakav kod zna dati bug ako se ubaci naknadno loptica u ballList
					/*
					foreach(PictureBox mBall in ballList)
                    {
						mBall.Left -= playerSpeed;
                    }
					*/
					for (int i = 0; i < ballList.Count; ++i)
						ballList[i].Left -= playerSpeed;
				}
			}
			if (goRight == true && player.Right < splitContainer1.Panel2.Width - 7) 
			{
				player.Left += playerSpeed;
				if (ball_speed == 0.0)
                {
					for (int i = 0; i < ballList.Count; ++i)
						ballList[i].Left += playerSpeed;
				}
					
			}

			//Pomicemo sve loptice na ploci
            for(int tmpCounter = 0; tmpCounter < ballList.Count; ++tmpCounter)
            {
				PictureBox mBall = ballList[tmpCounter];

				mBall.Left += (int)ballXList[tmpCounter];
				mBall.Top += (int)ballYList[tmpCounter];
            }

			//Lopta udara u rub prozora
			provjeriUdaranjeOdRub();

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
						ball_speed = (int)(1.3 * standard_ball_speed);
						this.splitContainer1.Panel2.Controls.Remove(ef);
						effectList.Remove(ef);

						//promijeni ballX i ballY
						for (int i = 0; i < ballXList.Count(); i++)
                        {
							double kut = Math.Atan2(ballYList[i], ballXList[i]);
							ballXList[i] = ball_speed * Math.Cos(kut);
							ballYList[i] = ball_speed * Math.Sin(kut);
						}
					}
					else if (effectTag.Description == "slow")
					{
						fast_slow_time = 1;
						ball_speed = (int)(0.8 * standard_ball_speed);
						this.splitContainer1.Panel2.Controls.Remove(ef);
						effectList.Remove(ef);

						//promijeni ballX i ballY
						for (int i = 0; i < ballXList.Count(); i++)
						{
							double kut = Math.Atan2(ballYList[i], ballXList[i]);
							ballXList[i] = ball_speed * Math.Cos(kut);
							ballYList[i] = ball_speed * Math.Sin(kut);
						}
					}
				}
			}

			//Lopta udara u igraca
			provjeriUdaranjeOdIgraca();

			// Provjeri dodiruje li lopta neku ciglu
			provjeriUdarenjeOdCiglu();

			//Provjera je li neka od loptica izasla iz granica polja.
			//Moramo ici u obrnutom smjeru zbog brisanja na odgovarajucim indeksima u listama ballXList i ballYList.
			//Inace bi obrisali manji indeks pa u iducem brisanju utjecali na pogresan element.
			for(int i = ballList.Count - 1; i >= 0; --i)
            {
				PictureBox mBall = ballList[i];
				if (mBall.Top > player.Top)
				{
					//Izbacujemo neku od loptica.
					ballList.Remove(mBall);
					this.splitContainer1.Panel2.Controls.Remove(mBall);
					ballXList.RemoveAt(i);
					ballYList.RemoveAt(i);
				}
			}
			//Ako su sve loptice izasle onda je kraj igre.
			if(ballList.Count == 0)
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
					for(int i = 0; i < ballList.Count; ++i)
						if (ballList[i].Bounds.IntersectsWith(x.Bounds))
							return;

				}
				foreach (var x in effectList)
				{
					Effect effectTag = (Effect)x.Tag;
					//Rectangle rect = new Rectangle(x.Left, x.Top + , x.Width, 32);
					for(int i = 0; i < ballList.Count; ++i)
						if ( !effectTag.Mobile &&  ballList[i].Bounds.IntersectsWith(x.Bounds))
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

		private void provjeriUdarenjeOdCiglu()
        {

			for(int tmpCounter = 0; tmpCounter < ballList.Count; ++tmpCounter)
            {
				PictureBox mBall = ballList[tmpCounter];

				foreach (Control x in this.splitContainer1.Panel2.Controls)
				{
					//Gledamo presjek lopte s ciglama.
					if (mBall.Bounds.IntersectsWith(x.Bounds) && x is PictureBox)
					{
						//preusmjeri lopticu tj. promijeni ballX ili ballY
						// jos uvijek se ne odbija savrseno ali bar je puno bolje nego prije
						//Loptica se obija i od statickih efekata jednako kao i od cigli.
						if (x.Tag is Block || (x.Tag is Effect && !((Effect)x.Tag).Mobile))
						{
							//provjeravanje s koje strane lopta dolazi
							//trazimo centar lopte te usporedujemo
							int ball_center_X = mBall.Left + (int)(mBall.Width / 2);
							int ball_center_Y = mBall.Top + (int)(mBall.Height / 2);

							if ((ball_center_X >= x.Left && ball_center_X <= x.Right && mBall.Top <= x.Bottom) ||
									(ball_center_X >= x.Left && ball_center_X <= x.Right && mBall.Bottom >= x.Top))
								//dolazi s gornje ili donje strane
								ballYList[tmpCounter] = -ballYList[tmpCounter];
							else if ((ball_center_Y <= x.Bottom && ball_center_Y >= x.Top && mBall.Right >= x.Left) ||
									(ball_center_Y <= x.Bottom && ball_center_Y >= x.Top && mBall.Left <= x.Right))
								//dolazi s lijeva ili desna
								ballXList[tmpCounter] = -ballXList[tmpCounter];
							else if ((ball_center_X < x.Left && mBall.Top <= x.Bottom) ||
										(ball_center_Y > x.Bottom && mBall.Right >= x.Left))
							//udara u lijevi donji rub
							{
								ballYList[tmpCounter] = Math.Abs(ballYList[tmpCounter]);
								ballXList[tmpCounter] = -Math.Abs(ballXList[tmpCounter]);
							}
							else if ((ball_center_X > x.Right && mBall.Top <= x.Bottom) ||
										(ball_center_Y > x.Bottom && mBall.Left <= x.Right))
							//udara u desni donji rub
							{
								ballYList[tmpCounter] = Math.Abs(ballYList[tmpCounter]);
								ballXList[tmpCounter] = Math.Abs(ballXList[tmpCounter]);
							}
							else if ((ball_center_X < x.Left && mBall.Bottom >= x.Top) ||
										(ball_center_Y < x.Top && mBall.Right >= x.Left))
							//udara u gornji lijevi rub
							{
								ballYList[tmpCounter] = -Math.Abs(ballYList[tmpCounter]);
								ballXList[tmpCounter] = -Math.Abs(ballXList[tmpCounter]);
							}
							else if ((ball_center_X > x.Right && mBall.Bottom >= x.Top) ||
										(ball_center_Y < x.Top && mBall.Left <= x.Right))
							//udara i gornji desni rub
							{
								ballYList[tmpCounter] = -Math.Abs(ballYList[tmpCounter]);
								ballXList[tmpCounter] = Math.Abs(ballXList[tmpCounter]);
							}
						}
						//funkcija koja unistava ciglu ili staticki efekt x
						destroyBlock(x);
					}
				}
            }
		}

		private void provjeriUdaranjeOdRub()
        {

			for(int tmpCounter = 0; tmpCounter < ballList.Count; ++tmpCounter)
            {
				PictureBox mBall = ballList[tmpCounter];

				//Ako je lopta išla prema lijevo i udarila u lijevi rub, odbija se(isto za gore i desno),
				if ((mBall.Left < 0 && ballXList[tmpCounter] < 0) || (mBall.Right > splitContainer1.Panel2.Width && ballXList[tmpCounter] > 0))
				{
					ballXList[tmpCounter] = -ballXList[tmpCounter];
				}
				if (mBall.Top < 0 && ballYList[tmpCounter] < 0)
				{
					ballYList[tmpCounter] = -ballYList[tmpCounter];
				}
			}
		}

		private void provjeriUdaranjeOdIgraca()
        {

			for (int tmpCounter = 0; tmpCounter < ballList.Count; ++tmpCounter)
			{
				PictureBox mBall = ballList[tmpCounter];

				if (mBall.Bounds.IntersectsWith(player.Bounds))
				{
					//pozicija gdje loptica udara o plocu
					double pos = mBall.Width / 2 + mBall.Left;

					double sredina_ploce = player.Left + player.Width / 2;
					double omjer = 2 * (pos - sredina_ploce) / player.Width;

					//korigiranje rubnih slučajeva
					omjer = (omjer < -1) ? -1 : omjer;
					omjer = (omjer > 1) ? 1 : omjer;
					//mapiranje intervala [-1,1]->[PI,0]
					//koliko će mjesto sudaranja lopte i ploče utjecati na
					//kut odbijanja lopte
					double kut = Math.PI / 2 + omjer * (-Math.PI / 2);

					//kut_2 je "klasični kut odbijanja lopte od neke podloge
					/*
					double kut_2 = 0;
					if (ballX < 0 && ballY > 0)
						kut_2 = Math.PI - Math.Atan(ballY / -ballX);
					else if (ballX > 0 && ballY > 0)
						kut_2 = Math.Atan(ballY / ballX);
					else if (ballX == 0)
						kut_2 = Math.PI / 2;
					*/

					//nedaj kut manji od PI/7 ili veći od 6PI/7
					kut = Math.Abs(kut) < Math.PI / 7 ? Math.PI / 7 : kut;
					kut = Math.Abs(kut) > 6 * Math.PI / 7 ? 6 * Math.PI / 7 : kut;

					ballYList[tmpCounter] = -Math.Sin(kut) * ball_speed;
					ballXList[tmpCounter] = Math.Cos(kut) * ball_speed;

				}
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

			//fast_slow_time++;
			time_to_shift++;

			// Ako je time_to_shift > 0 znaci da je pokupljen efekt za brzu ili sporu loptu
			if (fast_slow_time != 0)
				fast_slow_time++;
			if (fast_slow_time > 10)
			{   //ako je proslo 10 sekundi ugasi ga
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
					//brokenSound.Play();
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

					//brickSound.Play(); 

					//Mozda i maknuti iz liste cigli, ne samo iz kontrola?
					this.splitContainer1.Panel2.Controls.Remove(x);
					//Brisanje iz liste cigli i azuiranje lowest
					lowest = 0;
					foreach (PictureBox p in blockList.ToList())
						if (p.Top == x.Top && p.Left == x.Left)
							blockList.Remove(p);
                        else if (p.Top > lowest) 
							lowest = p.Top;
                        

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
						explosionSound.Play();
						this.splitContainer1.Panel2.Controls.Remove(x);

						//brisanje iz liste efekata
						lowest = 0;
						foreach (PictureBox p in effectList.ToList())
							if (p.Top == x.Top && p.Left == x.Left)
								effectList.Remove(p);
							else if (p.Top > lowest)
							{
								Effect ef = (Effect)p.Tag;
								if (!ef.Mobile)
									lowest = p.Top;
							}

						destroySurroundingBlocks(x); //unistava okolne cigle
					}
					else if (effectTag.Description == "newBall")
					{
						//dodati stvaranje novih loptice
						//newballsSound.Play();
						this.splitContainer1.Panel2.Controls.Remove(x);
						lowest = 0;
						foreach (PictureBox p in effectList.ToList())
							if (p.Top == x.Top && p.Left == x.Left)
								effectList.Remove(p);
							else if (p.Top > lowest)
                            {
								Effect ef = (Effect)p.Tag;
								if (!ef.Mobile)
									lowest = p.Top;
							} 
								
						createNewBalls(x);
					}
				}
			}
		}

		private void createNewBalls(Control x)
        {
			for(int i = 0; i < 2; ++i)
            {
				var newBall = new PictureBox();
				newBall.Height = 26;
				newBall.Width = 26;
				newBall.BackColor = SystemColors.ControlDarkDark;
				newBall.Left = x.Left + x.Width / 2 - newBall.Width / 2;
				newBall.Top = x.Top - newBall.Height;
				newBall.BackgroundImage = Properties.Resources.circle_cropped;
				newBall.BackgroundImageLayout = ImageLayout.Stretch;
				ballList.Add(newBall);
				this.splitContainer1.Panel2.Controls.Add(newBall);

				//Kod stvaranja novih loptica ranumao random kuteve pod kojim krecu.
				//Ovo bi se moglo jos doradit da tocno biramo kojim se kutom odbijaju, tj. nastaju.
				//double kut = 0.3 + rnd.NextDouble() * (2.8 - 0.3);
				//double kut = -0.5 * Math.PI + Math.Pow(-1, i) * i * 0.1 * Math.PI;
				//ballXList.Add( Math.Cos(kut) * ball_speed);
				//ballYList.Add( -Math.Sin(kut) * ball_speed);
				ballXList.Add(Math.Cos(0.5 * Math.PI + 0.05 * i * Math.PI) * ball_speed);
				ballYList.Add(-Math.Sin(0.5 * Math.PI + 0.05 * i * Math.PI) * ball_speed);

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

        private void splitContainer1_Panel2_MouseDown(object sender, MouseEventArgs e)
        {
			if (ball_speed == 0)
            {
				//pokretanje igre
				//prvo pronadi gdje treba usmjeriti lopticu
				//ako je klik bio prenisko, zanemari
				if (e.Y > player.Top - 5)
					return;
				
				int sredina_lopte_X = ballList[0].Left + (int)ballList[0].Width / 2;
				int sredina_lopte_Y = ballList[0].Top;
				int a = Math.Abs((e.X - splitContainer1.Panel1.Width) - sredina_lopte_X);
				int b = Math.Abs(e.Y - sredina_lopte_Y);
			
				double kut = Math.Atan2(b,a);
				
				//ako je klik bio s lijeve strane translatiraj ga
				if ((e.X - splitContainer1.Panel1.Width) < sredina_lopte_X)
					kut = Math.PI - kut;

				ball_speed = 11;
				standard_ball_speed = ball_speed;
				textBox1.Text = "";

				//ball_speed moze biti 0 samo kad imamo jednu lopticu, tj prije pocetka igre
				ballXList[0] = Math.Cos(kut) * ball_speed;
				ballYList[0] = -Math.Sin(kut) * ball_speed;

				//zapocni timer u za igru u sekundama
				timer1.Start();
			}
        }
    }
}
