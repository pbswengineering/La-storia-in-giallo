/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace LaStoriaInGiallo
{
	class EpisodeDB: System.Collections.IEnumerable
	{
		private List<Episode> episodes = new List<Episode>();
		
		public EpisodeDB()
		{
		}
		
		public EpisodeDB(string fileName)
		{
			ReadFrom(fileName);
		}
		
		private void ReadFrom(string fileName)
		{
			episodes.Clear();
			try
			{
				using(var reader = new StreamReader(fileName))
				{
					var line = "";
					while((line = reader.ReadLine()) != null)
					{
						try
						{
							episodes.Add(new Episode(line));
						}
						catch
						{
							// No problems with malformed lines...
						}
					}
				}
			}
			catch
			{
				// No problems with unexistant files and such....
			}
		}
		
		public void WriteTo(string fileName)
		{
			using(var writer = new StreamWriter(fileName))
			{
				foreach(Episode e in episodes)
				{
					writer.WriteLine(e.SerializeToString());
				}
			}			
		}
		
		public int Count
		{
			get
			{
				return episodes.Count;
			}
		}
				
		public int AddRange(List<Episode> newEpisodes)
		{
			int count = 0;
			foreach(Episode e in newEpisodes)
			{
				if(!episodes.Contains(e))
				{
					episodes.Insert(count, e);
					count++;
				}
			}
			return count;
		}
		
		public Episode[] GetRange()
		{
			return episodes.ToArray();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return episodes.GetEnumerator();
		}
		
		public int Find(string query)
		{
			var result = -1;
			var lquery = query.ToLower();
			for(var n = 0; n < episodes.Count; ++n)
			{
				if(episodes[n].Title.ToLower().Contains(lquery))
				{
					return n;
				}
			}
			return result;
		}
	}
}