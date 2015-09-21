using UnityEngine;
using System.Collections;

namespace SpellMastery.Model.DnD
{
public enum SpellAttributeType
{
	Class = 1,
	Domain = 2
}

public class SpellAttribute
{
	public const int MAX_RANK = 9;

	private int mRank;
	private SpellAttributeType mAttribute;
	private int mAttributeValue;

	public int rank
	{
		get { return mRank; }
		set { mRank = value; }
	}

	public SpellAttributeType attribute
	{
		get { return mAttribute; }
		set { mAttribute = value; }
	}

	public int attributeValue
	{
		get { return mAttributeValue; }
		set { mAttributeValue = value; }
	}
}
}
