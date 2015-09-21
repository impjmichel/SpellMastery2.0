using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;

namespace SpellMastery.Model.DnD
{
public class Spell : IComparable<Spell>
{
	private string mName;
	private DnDMagicSchool mSchool;
	private List<SpellAttribute> mClasses = new List<SpellAttribute>();
	private List<SpellAttribute> mDomains = new List<SpellAttribute>();

	public string name
	{
		get { return mName; }
	}
	public DnDMagicSchool school
	{
		get { return mSchool; }
	}
	public List<SpellAttribute> classes
	{
		get { return mClasses; }
	}
	public List<SpellAttribute> domains
	{
		get { return mDomains; }
	}

	public int CompareTo(Spell other)
	{
		return mName.CompareTo(other.name);
	}

	public void InitiateSpell(JSONObject spellObject)
	{
		mName = spellObject.GetString("name");
		mSchool = (DnDMagicSchool)spellObject.GetNumber("school");
		JSONArray classes = spellObject.GetArray("classes");
		foreach(var val in classes)
		{
			SpellAttribute attribute = new SpellAttribute();
			attribute.attribute = SpellAttributeType.Class;
			attribute.attributeValue = (int)val.Obj.GetNumber("class");
			attribute.rank = (int)val.Obj.GetNumber("rank");
			mClasses.Add(attribute);
		}
		JSONArray domains = spellObject.GetArray("domains");
		foreach (var val in domains)
		{
			SpellAttribute attribute = new SpellAttribute();
			attribute.attribute = SpellAttributeType.Domain;
			attribute.attributeValue = (int)val.Obj.GetNumber("domain");
			attribute.rank = (int)val.Obj.GetNumber("rank");
			mDomains.Add(attribute);
		}
	}

	/// <summary>
	/// Returns the spell's rank. cleric domain is checked before class,
	/// for best result only provide 1 (one) of these, never both.
	/// </summary>
	public int Rank(DnDClericDomain domain = DnDClericDomain.NONE, DnDCharClass charClass = DnDCharClass.NONE)
	{
		if (domain != DnDClericDomain.NONE)
		{
			for (int rank = 0; rank <= SpellAttribute.MAX_RANK; ++rank)
			{
				if (Contains(domain, rank))
					return rank;
			}
		}
		if (charClass != DnDCharClass.NONE)
		{
			for (int rank = 0; rank <= SpellAttribute.MAX_RANK; ++rank)
			{
				if (Contains(charClass, rank))
					return rank;
			}
		}
		return -1;
	}

	public bool Contains(DnDClericDomain domain)
	{
		foreach(SpellAttribute attribute in mDomains)
		{
			if (attribute.attributeValue == (int)domain)
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(DnDClericDomain domain, int rank)
	{
		foreach (SpellAttribute attribute in mDomains)
		{
			if (attribute.attributeValue == (int)domain && attribute.rank == rank)
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(DnDCharClass charClass)
	{
		foreach (SpellAttribute attribute in mClasses)
		{
			if (attribute.attributeValue == (int)charClass)
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(DnDCharClass charClass, int rank)
	{
		foreach (SpellAttribute attribute in mClasses)
		{
			if (attribute.attributeValue == (int)charClass && attribute.rank == rank)
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(DnDMagicSchool school)
	{
		return mSchool == school;
	}

	public bool Contains(DnDMagicSchool school, DnDCharClass cclass, int rank)
	{
		foreach (SpellAttribute attribute in mClasses)
		{
			if (attribute.attributeValue == (int)cclass && attribute.rank == rank)
			{
				return mSchool == school;
			}
		}
		return false;
	}

	public override string ToString()
	{
		string result = mName;
		result += " (" + mSchool.ToString() + ") ";
		return result;
	}

	public string ToShortString()
	{
		string result = "(" + mSchool.ToString().Substring(0,3).ToLower() + ") ";
		result += mName;
		return result;
	}

	public void Deserialize(JSONObject obj)
	{
		mName = obj.GetString("name");
		mSchool = (DnDMagicSchool)obj.GetNumber("school");
		mClasses = new List<SpellAttribute>();
		foreach(var value in obj.GetArray("classes"))
		{
			SpellAttribute tempAttribute = new SpellAttribute();
			tempAttribute.attribute = SpellAttributeType.Class;
			tempAttribute.attributeValue = (int)value.Obj.GetNumber("class");
			tempAttribute.rank = (int)value.Obj.GetNumber("rank");
			mClasses.Add(tempAttribute);
		}
		mDomains = new List<SpellAttribute>();
		foreach (var value in obj.GetArray("domains"))
		{
			SpellAttribute tempAttribute = new SpellAttribute();
			tempAttribute.attribute = SpellAttributeType.Domain;
			tempAttribute.attributeValue = (int)value.Obj.GetNumber("domain");
			tempAttribute.rank = (int)value.Obj.GetNumber("rank");
			mClasses.Add(tempAttribute);
		}
	}

	public JSONObject Serialize()
	{
		JSONObject obj = new JSONObject();
		obj.Add("name", new JSONValue(mName));
		obj.Add("school", new JSONValue((int)mSchool));
		JSONArray tempClasses = new JSONArray();
		foreach(SpellAttribute attribute in mClasses)
		{
			JSONObject temp = new JSONObject();
			temp.Add("class", attribute.attributeValue);
			temp.Add("rank", attribute.rank);
			tempClasses.Add(new JSONValue(temp));
		}
		obj.Add("classes", tempClasses);
		JSONArray tempDomains = new JSONArray();
		foreach (SpellAttribute attribute in mDomains)
		{
			JSONObject temp = new JSONObject();
			temp.Add("domain", attribute.attributeValue);
			temp.Add("rank", attribute.rank);
			tempDomains.Add(new JSONValue(temp));
		}
		obj.Add("domains", tempDomains);
		return obj;
	}
}
}
