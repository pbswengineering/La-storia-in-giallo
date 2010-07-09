/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LaStoriaInGiallo
{
	class MainProgram: Form
	{
		private Label lbSearch;
		private Color txSearchBackground;
		private TextBox txSearch;
		private ListBox lsEpisodes;
		private Button btDownload;
		private Label lbDownload;
		private ProgressBar pgDownload;
		private Label lbConvert;
		private ProgressBar pgConvert;
		
		private EpisodeDB db;
		private Crawler web;
		private ExternalSoftware sw;
		
		private string tempDirectory;
		private string mp3Directory;
		private string configDirectory;
		private string episodesFile;
		private bool canExit;
		private bool isExiting;
		
		public MainProgram()
		{
			canExit = false;
			isExiting = false;
		
			web = new Crawler();
			
			if(!web.Internet())
			{
				MessageBox.Show("Per usare questo programma devi essere connesso ad Internet.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
				return;
			}
			
			sw = new ExternalSoftware();
			
			var desktop = Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);			
			tempDirectory = Path.Combine(desktop, "La storia in giallo");
			if (!Directory.Exists(tempDirectory)) Directory.CreateDirectory(tempDirectory);
			
			mp3Directory = tempDirectory;
			if (!Directory.Exists(mp3Directory)) Directory.CreateDirectory(mp3Directory);
			
			var appData = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
			configDirectory = Path.Combine(appData, "la_storia_in_giallo");
			if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);			
			
			episodesFile = Path.Combine(configDirectory, "episodes.txt");
			var exeDirectory = Path.GetDirectoryName(Application.ExecutablePath);
			var dataDirectory = Path.Combine(exeDirectory, "data");
			if (!File.Exists(episodesFile))
			{
				var originalEpisodesFile = Path.Combine(dataDirectory, "episodes.txt");
				File.Copy(originalEpisodesFile, episodesFile);
			}
			
			Console.WriteLine();
			
			Text = "La storia in giallo";
			StartPosition = FormStartPosition.CenterScreen;
			ClientSize = new Size(400, 600);
			Icon = new Icon(Path.Combine(dataDirectory, "icon.ico"));
			FormClosing += CheckClosing;
			
			// Top
			lbSearch = new Label();
			lbSearch.Text = "Titolo da cercare";
			Controls.Add(lbSearch);
			
			txSearch = new TextBox();
			txSearch.TextChanged += delegate { SearchEpisode(); };
			txSearchBackground = txSearch.BackColor;
			Controls.Add(txSearch);
			
			// Middle
			lsEpisodes = new ListBox();
			Controls.Add(lsEpisodes);
			
			Resize += delegate { ResizeControls(); };
			
			// Bottom
			btDownload = new Button();
			btDownload.Text = "Scarica in formato MP3";
			btDownload.Font = new Font("Times New Roman", 12, FontStyle.Bold | FontStyle.Italic);
			btDownload.Click += delegate { 
				new Thread(new ThreadStart(DownloadEpisode)).Start();
			};
			Controls.Add(btDownload);
			
			// Bottom (continues...)
			lbDownload = new Label();
			lbDownload.Text = "Scaricamento";
			Controls.Add(lbDownload);
			
			pgDownload = new ProgressBar();
			pgDownload.Style = ProgressBarStyle.Continuous;
			pgDownload.Minimum = 0;
			pgDownload.Maximum = 100;
			Controls.Add(pgDownload);
			
			lbConvert = new Label();
			lbConvert.Text = "Conversione in MP3";	
			Controls.Add(lbConvert);
			
			pgConvert = new ProgressBar();
			pgConvert.Style = ProgressBarStyle.Continuous;
			pgConvert.Minimum = 0;
			pgConvert.Maximum = 100;
			Controls.Add(pgConvert);
			
			ResizeControls();
			EnableControls(true);
		
			db = new EpisodeDB(episodesFile);
			lsEpisodes.Items.AddRange(db.GetRange());			
			// No need to update the episodes list because the transmission is over
			//new Thread(new ThreadStart(UpdateEpisodes)).Start();
		}
		
		/*
		private void UpdateEpisodes()
		{
			EnableControls(false);
			var previousTitle = Text;
			Text = "Aggiornamento lista episodi...";
			var count = 0;
			var page = 1;
			do
			{
				var newEpisodes = web.ExtractEpisodes(web.DownloadPage(page));
				count = db.AddRange(newEpisodes);
				page++;
			}
			while(count != 0);
			db.WriteTo(episodesFile);
			lsEpisodes.Items.Clear();
			lsEpisodes.Items.AddRange(db.GetRange());
			Text = previousTitle;
			EnableControls(true);
		}
		*/
		
		private void ResizeControls()
		{
			var border = 20;
			var labelWidth = 120;
			var fullWidth = this.Width - border * 2 - SystemInformation.Border3DSize.Width * 4;
	
			lbSearch.Location = new Point(border, border);
			lbSearch.Size = new Size(labelWidth - 30, 20);			

			txSearch.Location = new Point(labelWidth - 30 + border * 2, border);
			txSearch.Size = new Size(fullWidth - labelWidth + 30 - border, 15);

			btDownload.Location = new Point(border, this.Height - 70 - SystemInformation.Border3DSize.Height * 4);
			btDownload.Size = new Size(fullWidth, 30);
			
			pgConvert.Location = new Point(labelWidth + border * 2, btDownload.Location.Y - 30 - SystemInformation.Border3DSize.Height * 2);
			pgConvert.Size = new Size(fullWidth - labelWidth - border, 20);
			
			lbConvert.Location = new Point(border, pgConvert.Location.Y);
			lbConvert.Size = new Size(labelWidth, 20);

			pgDownload.Location = new Point(labelWidth + border * 2, pgConvert.Location.Y - 40 - SystemInformation.Border3DSize.Height * 2);
			pgDownload.Size = new Size(fullWidth - labelWidth - border, 20);

			lbDownload.Location = new Point(border, pgDownload.Location.Y);
			lbDownload.Size = new Size(labelWidth, 20);
			
			lsEpisodes.Location = new Point(border, txSearch.Location.Y + txSearch.Size.Height + SystemInformation.Border3DSize.Height * 4);
			lsEpisodes.Size = new Size(fullWidth, pgDownload.Location.Y - (txSearch.Location.Y + txSearch.Height) - 15);
		}
		
		private void CheckClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			/*
			if(!canExit)
			{
				var result = MessageBox.Show("Sei sicuro di voler uscire?", "Domanda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if(result == System.Windows.Forms.DialogResult.Yes)
				{
					sw.Terminate();
					e.Cancel = false;
					isExiting = true;
				}
				else
				{
					e.Cancel = true;
				}
			}*/
			e.Cancel = !canExit;
		}
		
		private void EnableControls(bool enabled)
		{
			canExit = enabled;
			txSearch.Enabled = enabled;
			btDownload.Enabled = enabled;
			lsEpisodes.Enabled = enabled;
		}
		
		private void SearchEpisode()
		{
			var query = txSearch.Text;
			if(query.Length > 0)
			{
				var index = db.Find(query);
				txSearch.BackColor = (index  == -1 ? Color.Wheat : txSearchBackground);
				lsEpisodes.SelectedIndex = index;
			}
			else
			{
				txSearch.BackColor = txSearchBackground;
			}
		}
		
		private void UpdateDownloadStatus(float percentage)
		{
			pgDownload.Value = (int)percentage;
		}
		
		private void UpdateConvertStatus(float percentage)
		{
			pgConvert.Value = (int)percentage;
		}

		private void DownloadEpisode()
		{
			try
			{
				Episode selection = (Episode)lsEpisodes.SelectedItem;
				var title = selection.Title;
				
				pgDownload.Value = 0;
				pgConvert.Value = 0;
				EnableControls(false);
				
				var rtspURL = web.GetRTSPURL(selection.StreamingURL);
				var wav = Path.Combine(tempDirectory, "audiodump.wav");
				var mp3 = Path.Combine(mp3Directory, title + ".mp3");
				sw.DownloadRTSP(rtspURL, wav, new UpdateStatusDelegate(UpdateDownloadStatus));
				sw.ConvertToMP3(wav, mp3, new UpdateStatusDelegate(UpdateConvertStatus));
				TagLib.File tag = TagLib.File.Create(mp3);
				tag.Tag.Title = title;
				tag.Tag.Performers = new string[] {"La storia in giallo"};
				tag.Save();
				
				File.Delete(wav);
				
				MessageBox.Show("File MP3 creato correttamente.", "Tutto fatto!", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
			catch(NullReferenceException)
			{
				// This happens everytime the button is clicked without any episode being selected
			}
			catch(Exception e)
			{
				if(!isExiting)
				{
					pgDownload.Value = 0;
					pgConvert.Value = 0;
					MessageBox.Show("Si è verificato un errore: " + e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			finally
			{
				EnableControls(true);
			}
		}
		
		public static void Main(string[] args)
		{			
			Application.Run(new MainProgram());
		}
	}
}
