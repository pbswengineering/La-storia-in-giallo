/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace LaStoriaInGiallo
{
	class Version
	{
		public const double CURRENT_VERSION = 2.0;
		public const string URL = "http://www.webalice.it/bernardi82/software/lstg_version.txt";
	
		public bool Check()
		{
			try
			{
                		var text = ""; 
	                        using(var web = new WebClient())
        	                {
        	                        web.Headers["User-Agent"] = Crawler.DEFAULT_USER_AGENT;
	                                text = web.DownloadString(URL).Trim();
                	        }
				// Now text is something like "20 http://pippo.com"
				// The version is multiplied by 10 to avoid localization problems
				// (e.g. in my Italian Windows the floating point numbers use commas,
				// in my English Linux systems they use the dot)
				var parts = text.Split('\n')[0].Split(' ');
				var lastVersion = Convert.ToDouble(parts[0]) / 10;
				var lastVersionUrl = parts[1];
				if (lastVersion > CURRENT_VERSION)
				{
					var message = "Stai usando la versione {0:0.0} del programma mentre quella";
					message += " piu' recente e' la {1:0.0}; premi OK per visualizzare la pagina web";
					message += " da cui scaricare la nuova versione del programma.";
					message = string.Format(message, CURRENT_VERSION, lastVersion);
					var result = MessageBox.Show(message, "Aggiornamento disponibile", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
					if(result == System.Windows.Forms.DialogResult.OK)	
					{
						Process.Start(lastVersionUrl);
						return true;
					}
				}
				return false;
			}
			catch(Exception)
			{
				// Oh well, no update for you!
				return false;
			}
		}
	}
}
