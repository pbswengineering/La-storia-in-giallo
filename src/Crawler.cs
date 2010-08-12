/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace LaStoriaInGiallo
{
	abstract class Crawler
	{
		public const string DEFAULT_USER_AGENT = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

		public bool Internet()
		{
			try
			{
				Dns.GetHostEntry("www.radio.rai.it");
				return true;
			}
			catch
			{
				return false;
			}
		}

		public string GetRTSPURL(string ramUrl)
		{
			var rtsp = "";
			using(var web = new WebClient())
			{
				web.Headers["User-Agent"] = DEFAULT_USER_AGENT;
				rtsp = web.DownloadString(ramUrl).Trim();
			}
			return rtsp;
		}

		public abstract string DownloadPage(int downloadPage);
		public abstract List<Episode> ExtractEpisodes(string html);
	}

	class CrawlerLstg : Crawler
	{
		const string BASE_URL = "http://www.radio.rai.it/radio3/lastoriaingiallo/";
		const string URL_TEMPLATE = BASE_URL + "puntate.cfm?first=$NUMBER&V_ARCHIVIO=&Q_DATA_IN=&Q_DATA_OUT=&Q_PROG_ID=347&Q_KEYS=";
		const string EPISODE_BOUND = "<div id=\"giallo_event\">";
		const int EPISODES_PER_PAGE = 10;
		
		public override string DownloadPage(int pageNumber)
		{
			var startFromEpisode = string.Format("{0}", (pageNumber - 1) * EPISODES_PER_PAGE + 1);
			var html = "";
			
			using(var web = new WebClient())
			{
				web.Headers["User-Agent"] = DEFAULT_USER_AGENT;
				html = web.DownloadString(URL_TEMPLATE.Replace("$NUMBER", startFromEpisode));
			}
			
			return html;
		}
		
		private List<string> ExtractEpisodesChunks(string html)
		{
			var a = 0;
			var b = 0;
			var result = new List<string>();
			while(a != -1)
			{
				a = html.IndexOf(EPISODE_BOUND, b);
				b = html.IndexOf(EPISODE_BOUND, a + 1);
				if(b == -1)
				{
					b = html.Length;
				}
				
				if (a != -1)
				{
					result.Add(html.Substring(a, b - a));
				}
			}
			return result;
		}
		
		public override List<Episode> ExtractEpisodes(string html)
		{
			html.Replace("\n", " ");
			var pattern = @"<a title=""[^""]*?"" href=""/radio3/lastoriaingiallo/view.cfm[^""]*?"" class=""link"">(.*?)</a>.*?<a href=""(archivio_[^""]*?\.ram)";
			var r = new Regex(pattern, RegexOptions.Singleline);
			var list = new List<Episode>();
			foreach(string chunk in ExtractEpisodesChunks(html))
			{
				var m = r.Match(chunk);
				while(m.Success)
				{
					var title = m.Groups[1].Captures[0].ToString().Replace("<br>", " - ");
					var link = BASE_URL + m.Groups[2].Captures[0].ToString();
					list.Insert(0, new Episode(title, link));
					m = m.NextMatch();
				}
			}
			list.Reverse();
			return list;
		}
	}

	abstract class SimpleCrawler : Crawler
	{	
		public abstract string GetBaseURL();

		public override string DownloadPage(int pageNumber)
		{
			if (pageNumber != 1)
			{
				return "";
			}
			
			var html = "";
			
			using (var web = new WebClient())
			{
				web.Headers["User-Agent"] = DEFAULT_USER_AGENT;
				html = web.DownloadString(GetBaseURL());
			}
			
			return html;
		}
		
		
		public override List<Episode> ExtractEpisodes(string html)
		{
			html.Replace("\n", " ");
			var pattern = @"<a href=""([^""]*?\.ram)"">([^<]*?)</a>";
			var r = new Regex(pattern, RegexOptions.Singleline);
			var list = new List<Episode>();
			var m = r.Match(html);
			while(m.Success)
			{
				var title = m.Groups[2].Captures[0].ToString().Replace("<br>", " - ");
				// Every title has a prefix like "26/06/2010 - "
				title = title.Substring(13);
				title = title.Replace("&#39;", "'");
				var link = m.Groups[1].Captures[0].ToString();
				list.Insert(0, new Episode(title, link));
				m = m.NextMatch();
			}
			list.Reverse();
			return list;
		}
	}

	class CrawlerCdtd : SimpleCrawler
	{
		public override string GetBaseURL()
		{
			return  "http://www.radio3.rai.it/dl/radio3/programmi/PublishingBlock-5aa4947c-8dbd-4b7c-9083-1090eed9b8b7.html";
		}
	}

	class CrawlerCdto : SimpleCrawler
	{
		public override string GetBaseURL()
		{
			return "http://www.radio3.rai.it/dl/radio3/programmi/PublishingBlock-a723c434-72c7-4971-87ed-1a325f37287a.html";
		}
	}
}
