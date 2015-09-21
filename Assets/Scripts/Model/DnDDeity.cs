
using System;
using System.Collections.Generic;
using Boomlagoon.JSON;

namespace SpellMastery.Model.DnD
{
	public class DnDDeity : SerializableObject
	{
		private const string cAltNames = "altNames";
		private const string cRaces = "races";
		private const string cClasses = "classes";
		private const string cDomains = "domains";

		public static List<DnDDeity> GodsOfDnD = new List<DnDDeity>()
		{
			new DnDDeity(
				"Boccob", 
				new List<string>() { "the Uncaring", "Lord of All Magics", "Archmage of the Deities" },
				DnDAlignment.TrueNeutral, 
				new List<DnDRace>(), 
				new List<DnDCharClass>() { DnDCharClass.Cleric, DnDCharClass.Sorcerer, DnDCharClass.Wizard },
				new List<DnDClericDomain>() { DnDClericDomain.Knowledge, DnDClericDomain.Magic, DnDClericDomain.Trickery }
				),
			new DnDDeity(
				"Correllon Larethian", 
				new List<string>() { "Creator of the Elves", "the Protector", "Protector and Preserver of Life", "Ruler of All Elves" },
				DnDAlignment.ChaoticGood, 
				new List<DnDRace>() { DnDRace.Elf, DnDRace.HalfElf }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric },
				new List<DnDClericDomain>() { DnDClericDomain.Chaos, DnDClericDomain.Good, DnDClericDomain.Protection, DnDClericDomain.War }
				),
			new DnDDeity(
				"Ehlonna", 
				new List<string>() { "Ehlonna of the Forests" },
				DnDAlignment.NeutralGood, 
				new List<DnDRace>() { DnDRace.Elf, DnDRace.Gnome, DnDRace.HalfElf, DnDRace.Halfling }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric, DnDCharClass.Ranger },
				new List<DnDClericDomain>() { DnDClericDomain.Animal, DnDClericDomain.Good, DnDClericDomain.Plant, DnDClericDomain.Sun }
				),
			new DnDDeity(
				"Erythnul", 
				new List<string>() { "the Many" },
				DnDAlignment.ChaoticEvil, 
				new List<DnDRace>() { }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric, DnDCharClass.Barbarian, DnDCharClass.Fighter, DnDCharClass.Rogue },
				new List<DnDClericDomain>() { DnDClericDomain.Chaos, DnDClericDomain.Evil, DnDClericDomain.Trickery, DnDClericDomain.War }
				),
			new DnDDeity(
				"Fharlanghn", 
				new List<string>() { "Dweller on the Horizon" },
				DnDAlignment.TrueNeutral, 
				new List<DnDRace>() { }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric, DnDCharClass.Bard },
				new List<DnDClericDomain>() { DnDClericDomain.Luck, DnDClericDomain.Protection, DnDClericDomain.Travel }
				),
			new DnDDeity(
				"Garl Glittergold", 
				new List<string>() { "the Joker", "the Watchful Protector", "the Priceless Gem", "the Sparkling Wit" },
				DnDAlignment.NeutralGood, 
				new List<DnDRace>() { DnDRace.Gnome }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric },
				new List<DnDClericDomain>() { DnDClericDomain.Good, DnDClericDomain.Protection, DnDClericDomain.Trickery }
				),
			new DnDDeity(
				"Gruumsh", 
				new List<string>() { "One-Eye", "He-Who-Never-Sleeps" },
				DnDAlignment.ChaoticEvil, 
				new List<DnDRace>() { DnDRace.HalfOrc }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric },
				new List<DnDClericDomain>() { DnDClericDomain.Chaos, DnDClericDomain.Evil, DnDClericDomain.Strength, DnDClericDomain.War }
				),
			new DnDDeity(
				"Heironeous", 
				new List<string>() { "the Invincible" },
				DnDAlignment.LawfulGood, 
				new List<DnDRace>() { }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric, DnDCharClass.Fighter, DnDCharClass.Monk, DnDCharClass.Paladin },
				new List<DnDClericDomain>() { DnDClericDomain.Good, DnDClericDomain.Law, DnDClericDomain.War }
				),
			new DnDDeity(
				"Hextor", 
				new List<string>() { "Champion of Evil", "Herald of Hell", "Scourge of Battle" },
				DnDAlignment.LawfulEvil, 
				new List<DnDRace>() { }, 
				new List<DnDCharClass>() { DnDCharClass.Cleric, DnDCharClass.Fighter, DnDCharClass.Monk },
				new List<DnDClericDomain>() { DnDClericDomain.Destruction, DnDClericDomain.Evil, DnDClericDomain.Law, DnDClericDomain.War }
				)
		};

		public string Name { get; private set; }

		public List<string> AlternativeNames { get; private set; }

		public DnDAlignment Alignment { get; private set; }

		public List<DnDRace> WorshippingRaces { get; private set; }

		public List<DnDCharClass> WorshippingClasses { get; private set; }

		public List<DnDClericDomain> Domains { get; private set; }

		/// <summary>
		/// Only use this constructor for deserialization
		/// </summary>
		public DnDDeity()
		{
		}
		public DnDDeity(string name, List<string> altNames, DnDAlignment alignment, List<DnDRace> races, List<DnDCharClass> classes, List<DnDClericDomain> domains)
		{
			Name = name;
			AlternativeNames = altNames;
			Alignment = alignment;
			WorshippingRaces = races;
			WorshippingClasses = classes;
			Domains = domains;
		}

		public override JSONObject Serialize()
		{
			JSONObject obj = new JSONObject();
			obj.Add(NAME, Name);
			JSONArray tempArray = new JSONArray();
			foreach (string alt in AlternativeNames)
			{
				tempArray.Add(new JSONValue(alt));
			}
			obj.Add(cAltNames, tempArray);
			obj.Add(ALIGNMENT, (int)Alignment);
			tempArray = new JSONArray();
			foreach (DnDRace race in WorshippingRaces)
			{
				tempArray.Add(new JSONValue((int)race));
			}
			obj.Add(cRaces, tempArray);
			tempArray = new JSONArray();
			foreach (DnDCharClass charClass in WorshippingClasses)
			{
				tempArray.Add(new JSONValue((int)charClass));
			}
			obj.Add(cClasses, tempArray);
			tempArray = new JSONArray();
			foreach (DnDClericDomain domain in Domains)
			{
				tempArray.Add(new JSONValue((int)domain));
			}
			obj.Add(cDomains, tempArray);
			return obj;
		}

		public override void Deserialize(JSONObject obj)
		{
			Name = obj.GetString(NAME);
			AlternativeNames = new List<string>();
			JSONArray tempArray = obj.GetArray(cAltNames);
			foreach (var val in tempArray)
			{
				AlternativeNames.Add(val.Str);
			}
			Alignment = (DnDAlignment)(int)obj.GetNumber(ALIGNMENT);
			WorshippingRaces = new List<DnDRace>();
			tempArray = obj.GetArray(cRaces);
			foreach (var val in tempArray)
			{
				WorshippingRaces.Add((DnDRace)(int)val.Number);
			}
			WorshippingClasses = new List<DnDCharClass>();
			tempArray = obj.GetArray(cClasses);
			foreach (var val in tempArray)
			{
				WorshippingClasses.Add((DnDCharClass)(int)val.Number);
			}
			Domains = new List<DnDClericDomain>();
			tempArray = obj.GetArray(cDomains);
			foreach (var val in tempArray)
			{
				Domains.Add((DnDClericDomain)(int)val.Number);
			}
		}
	}
}
