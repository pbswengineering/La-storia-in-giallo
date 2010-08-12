/*
 * Copyright 2009 Paolo Bernardi <villa.lobos@tiscali.it>
 *
 * Questo programma è distribuito secondo i termini della licenza
 * MIT/X11. Il testo della licenza è nel file licenza.txt contenuto
 * nella directory doc/
 */

using System;
using System.Collections.Generic;

namespace LaStoriaInGiallo
{
	abstract class Transmission
	{
		public abstract string Name { get; }
		public abstract string Code { get; }
		public abstract Crawler Crawler { get; }
		public abstract bool IsOver { get; }
		public abstract bool MplayerEndsWithError { get; }
	}

	class TransmissionLstg : Transmission
	{
		public override string Name { get { return "La Storia in Giallo"; } }
		public override string Code { get { return "lstg"; } }
		public override Crawler Crawler { get { return new CrawlerLstg(); } }
		public override bool IsOver { get { return true; } }
		public override bool MplayerEndsWithError { get { return false; } }
	}

	class TransmissionCdtd : Transmission
	{
		public override string Name { get { return "Cuore di Tenebra - Dentro la storia"; } }
		public override string Code { get { return "cdtd"; } }
		public override Crawler Crawler { get { return new CrawlerCdtd(); } }
		public override bool IsOver { get { return false; } }
		public override bool MplayerEndsWithError { get { return true; } }
	}

	class TransmissionCdto : Transmission
	{
		public override string Name { get { return "Cuore di Tenebra - Oltre la storia"; } }
		public override string Code { get { return "cdto"; } }
		public override Crawler Crawler { get { return new CrawlerCdto(); } }
		public override bool IsOver { get { return false; } }
		public override bool MplayerEndsWithError { get { return true; } }
	}

	class TransmissionManager
	{
		private Dictionary<string, string> nameCode = new Dictionary<string, string>();
		private Dictionary<string, Transmission> codeTransmission = new Dictionary<string, Transmission>();
		private List<string> transmissionNames = new List<string>();
		private Transmission defaultTransmission;

		private void AddTransmission(Transmission t)
		{
			nameCode[t.Name] = t.Code;
			codeTransmission[t.Code] = t;
			transmissionNames.Add(t.Name);
		}

		public TransmissionManager()
		{
			defaultTransmission = new TransmissionLstg();
			AddTransmission(defaultTransmission);
			AddTransmission(new TransmissionCdtd());
			AddTransmission(new TransmissionCdto());
		}

		public Transmission DefaultTransmission { get { return defaultTransmission; } }

		public List<string> TransmissionNames { get { return transmissionNames; } }

		public string TransmissionCode(string name)
		{
			return nameCode[name];
		}

		public Transmission Transmission(string code)
		{
			return codeTransmission[code];
		}
	}
}