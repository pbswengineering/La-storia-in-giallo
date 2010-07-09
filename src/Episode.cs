/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;

namespace LaStoriaInGiallo
{
	class Episode
	{
		private string title;
		public string Title
		{ 
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}
		
		private string streamingURL;
		public string StreamingURL
		{ 
			get
			{
				return streamingURL;
			}
			set
			{
				streamingURL = value;
			}
		}
		
		public Episode(string title, string streamingURL)
		{
			Title = title;
			StreamingURL = streamingURL;
		}

		public Episode(string stringRepresentation)
		{
			var v = stringRepresentation.Split('|');
			Title = v[0].Trim();
			StreamingURL = v[1].Trim();
		}
		
		public string SerializeToString()
		{
			return Title + "|" + StreamingURL;
		}
		
		public override string ToString()
		{
			return Title;
		}
		
		public override bool Equals(object o)
		{
			Episode e = (Episode)o;
			return e.Title == Title;
		}
		
		public override int GetHashCode()
		{
			return (Title + StreamingURL).GetHashCode();
		}
		
		public static bool operator ==(Episode a, Episode b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Episode a, Episode b)
		{
			return !(a == b);
		}

	}
}