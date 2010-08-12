/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LaStoriaInGiallo
{
	public delegate void UpdateStatusDelegate(float percentage);

	class ExternalSoftware
	{
		private delegate float ExtractPercentageDelegate(string text);
		
		private string mplayer;
		private string lame;
		
		private string bigWavFile;
		
		private Regex regexMPlayer = new Regex(@"A:\s*(\d+\.\d+)\s*\(.*?\)\s*of\s*(\d+\.\d+)");
		private Regex regexLame = new Regex(@"\s*\d+/\d+\s+\(\s*(\d+)%\)");
		
		private Process process;
		
		private bool mplayerEndsWithError;
		
		public ExternalSoftware(bool mplayerEndsWithError)
		{
			if(System.Environment.OSVersion.VersionString.StartsWith("Unix"))
			{
				mplayer = "mplayer";
				lame = "lame";
			}
			else
			{
				var exeDirectory = Path.GetDirectoryName(Application.ExecutablePath);
				mplayer = Path.Combine(Path.Combine(exeDirectory, "apps"), "mplayer.exe");
				lame = Path.Combine(Path.Combine(exeDirectory, "apps"), "lame.exe");
			}
		
			this.mplayerEndsWithError = mplayerEndsWithError;
			process = null;
		}

		private void RunAndUpdatePercentage(string program, string args, string workingDirectory, bool stdout, ExtractPercentageDelegate getPercentage, UpdateStatusDelegate update, string errorMessage, bool stopOnErrors)
		{
			var info = new ProcessStartInfo();			
			info.FileName = program;
			info.Arguments = args;
			info.WorkingDirectory = workingDirectory;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.CreateNoWindow = true;
						
			using(process = Process.Start(info))
			{
				using(var output = stdout ? process.StandardOutput : process.StandardError)
				{
					update(0);
					var line = "";
					while(!process.HasExited)
					{
						if((line = output.ReadLine()) != null)
						{
							var perc = getPercentage(line);
							if(perc > 0)
							{
								update(perc);
							}
						}
					}

					update(100);
					
					if(stopOnErrors && process.ExitCode != 0)
					{
						throw new Exception(errorMessage);
					}
				}
			}
			process = null;
		}
		
		public void Terminate()
		{
			if(process != null)
			{
				process.Kill();
				File.Delete(bigWavFile);
			}
		}
		
		private float ExtractPercentageMPlayer(string text)
		{
			var m = regexMPlayer.Match(text);
			if(m.Success)
			{
				var done = float.Parse(m.Groups[1].Captures[0].ToString());
				var tot = float.Parse(m.Groups[2].Captures[0].ToString());
				return done * 100 / tot;
			}
			else
			{
				return -1;
			}
		}
		
		private float ExtractPercentageLame(string text)
		{
			var m = regexLame.Match(text);
			if(m.Success)
			{
				return float.Parse(m.Groups[1].Captures[0].ToString());
			}
			else
			{
				return -1;
			}
		}
		
		public void DownloadRTSP(string rtspURL, string outFile, UpdateStatusDelegate update)
		{
			var f = new DirectoryInfo(outFile);
			// Without -nocache it creates two processes, one being used for caching
			var args = string.Format("\"{0}\" -ao pcm -ao pcm:file=\"{1}\" -vc dummy -vo null", rtspURL, f.Name);
			bigWavFile = outFile;
			RunAndUpdatePercentage(mplayer, args, f.Parent.FullName, true, ExtractPercentageMPlayer, update, "Si è verificato un errore durante lo scaricamento della puntata.", !mplayerEndsWithError);
		}
		
		public void ConvertToMP3(string inFile, string outFile, UpdateStatusDelegate update)
		{
			if(!File.Exists(inFile))
			{
				throw new FileNotFoundException("impossibile trovare " + inFile);
			}
			
			var args = string.Format("\"{0}\" \"{1}\"", inFile, outFile);
			RunAndUpdatePercentage(lame, args, ".", false, ExtractPercentageLame, update, "Si è verificato un errore durante la creazione del file MP3.", true);
		}
	}
}