using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Boomlagoon.JSON;

namespace SpellMastery.Model.DnD
{
public class SpellList
{
	private const string cOnlinePath = "http://impdevelopment.eu/SpellMastery/list.data";
	private const string cFilename = "/list.data";

	private string mPath;
	private string mVersion;
	private List<string> mMessage = new List<string>();
	private List<Spell> mSpells = new List<Spell>();
	
	public List<Spell> Spells
	{
		get { return mSpells; }
	}

	public string Version
	{
		get { return mVersion; }
	}

	public List<string> Message
	{
		get { return mMessage; }
	}

	public SpellList()
	{
		mPath = Application.persistentDataPath;
	}

	public List<Spell> GetSpellsOfClassAndRank(DnDCharClass cclass, int rank)
	{
		ListCheck();
		List<Spell> result = mSpells.FindAll(
			delegate(Spell temp)
			{
				return temp.Contains(cclass, rank);
			});
		return result;
	}

	public List<Spell> GetWizardSchoolSpecializationSpells(DnDMagicSchool school, int rank)
	{
		ListCheck();
		List<Spell> result = mSpells.FindAll(
			delegate(Spell temp)
			{
				return temp.Contains(school, DnDCharClass.Wizard, rank);
			});
		return result;
	}

	public List<Spell> Get2DomainSpellsWithRank(DnDClericDomain domain_1, DnDClericDomain domain_2, int rank)
	{
		ListCheck();
		List<Spell> result = GetDomainAndRankSpells(domain_1, rank);
		result.AddRange(GetDomainAndRankSpells(domain_2, rank));
		return result;
	}

	public IEnumerator GetOnlineList()
	{
		WWW www = new WWW(cOnlinePath);
		yield return www;

		if (string.IsNullOrEmpty(www.error))
		{
			string data = www.text;
			if (!string.IsNullOrEmpty(data))
			{
				DeserializeData(data);
			}
		}
		else
		{
			GetOfflineList();
		}
	}

	private List<Spell> GetDomainSpells(DnDClericDomain domain)
	{
		ListCheck();
		List<Spell> result = mSpells.FindAll(
			delegate(Spell temp)
			{
				return temp.Contains(domain);
			});
		return result;
	}

	private List<Spell> GetDomainAndRankSpells(DnDClericDomain domain, int rank)
	{
		ListCheck();
		List<Spell> result = mSpells.FindAll(
			delegate(Spell temp)
			{
				return temp.Contains(domain, rank);
			});
		return result;
	}

	private void WriteList()
	{
		string data = "{\"spells\":[" + "\n";
		mSpells.Sort();
		for (int i = 0; i < mSpells.Count; ++i)
		{
			data += mSpells[i].Serialize().ToString();
			if (i != mSpells.Count - 1)
			{ // last items does not want the comma.
				data += ",\n";
			}
		}
		data += "]}";
		Control.IOManager.WriteData(mPath + cFilename, data);
	}

	private void GetOfflineList()
	{
		string data = Control.IOManager.ReadData(mPath + cFilename);
		if (!string.IsNullOrEmpty(data))
		{
			DeserializeData(data);
		}
	}

	private void DeserializeData(string data)
	{
		mSpells.Clear();
		JSONObject obj = JSONObject.Parse(data);
		if (obj.ContainsKey("version"))
		{
			mVersion = obj.GetString("version");
		}
		if (obj.ContainsKey("message"))
		{
			mMessage = new List<string>();
			JSONArray jsonMessage = obj.GetArray("message");
			foreach (var val in jsonMessage)
			{
				mMessage.Add(val.Str);
			}
		}
		JSONArray spells = obj.GetArray("spells");
		foreach (var val in spells)
		{
			Spell spell = new Spell();
			spell.InitiateSpell(val.Obj);
			mSpells.Add(spell);
		}
		WriteList();
	}

	private void ListCheck()
	{
		if (mSpells.Count == 0) // this is means something went bad... probably
		{
			GetOfflineList();
		}
	}
}
}
