using SpellMastery.Control;
using SpellMastery.Model;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
public class DnDWizardCreationSpellsMain : CreationMainScreen
{
	private const string cSpellBookExplanation = "Spellbook\n{0}";
	private const string cAddedZeroRankText = "All castable rank 0 spells have been added to your spellbook.";
	private const string cAddMoreText = "\nYou can add more spells";
	private const string cCantAddMore = "You have added all castable spells to your spellbook.";
	private const string cLowAbility = "Your character's {0} ability score isn't high enough to cast any spells.";
	private const string cShortRankText = "Rank {0} [{1}]";
	private const int cReturnScreen = 2;
	private const int cAddSpellsScreen = 6;
	private const int cPrevScreen = 4;
	private const int cMaxRankFirst = 4;
	private const int cMaxRankLast = 9;

	private bool mSetText = false;
	private DnDWizard mSoul;

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text TopText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text FirstRanksText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text LastRanksText;

	public override void Next()
	{
		mSetText = false;
		Storage.Screens[cReturnScreen].SetActive(true);
		this.gameObject.SetActive(false);
	}

	public override void Back()
	{
		Storage.Screens[cPrevScreen].SetActive(true);
		Reset();
		this.gameObject.SetActive(false);
	}

	public void OnClick_AddSpells()
	{
		Storage.Screens[cAddSpellsScreen].SetActive(true);
		mSetText = false;
		this.gameObject.SetActive(false);
	}

	// Update is called once per frame
	public override void Update()
	{
		base.Update();
		if (mSoul == null)
		{
			mSoul = ((DnDCharacter)(Storage.Character)).Classes.Find(x => x.CharacterClass == DnDCharClass.Wizard) as DnDWizard;
		}
		if (!mSetText)
		{
			mSetText = true;
			int numberOfUnknownSpells = 0;
			for (int i = 1; i <= mSoul.HighestCastableRank; ++i)
			{
				numberOfUnknownSpells += mSoul.NumberofUnknownSpells(i);
			}
			//disabling button if no more spells can be added:
			ViewUtility.EnableButton(transform.FindChild("Menu").FindChild("AddSpellsButton").gameObject, numberOfUnknownSpells != 0);
			// setting the top text:
			if (TopText != null)
			{
				if (mSoul.HighestCastableRank < 0)
				{
					TopText.text = string.Format(cSpellBookExplanation, string.Format(cLowAbility, mSoul.SpellModifier.ToString()));
				}
				else if (mSoul.KnownSpells.Count == 0)
				{
					AddZeroRankSpells();
					TopText.text = string.Format(cSpellBookExplanation, cAddedZeroRankText);
				}
				else if (numberOfUnknownSpells == 0)
				{
					TopText.text = cCantAddMore;
				}
				else
				{
					TopText.text = string.Format(cSpellBookExplanation, cAddMoreText);
				}
			}
			// setting the current known spell texts:
			if (FirstRanksText != null && LastRanksText != null)
			{
				int rank = -1;
				string textFirst = "";
				string textLast = "";
				foreach (var list in mSoul.KnownSpells)
				{
					rank++;
					if (rank > cMaxRankFirst)
					{
						textLast += string.Format(cShortRankText, rank, list.Count);
						if (rank < cMaxRankLast)
						{
							textLast += "\n";
						}
					}
					else
					{
						textFirst += string.Format(cShortRankText, rank, list.Count);
						if (rank < cMaxRankFirst)
						{
							textFirst += "\n";
						}
					}
				}
				FirstRanksText.text = textFirst;
				LastRanksText.text = textLast;
			}
		}
	}

	private void Reset()
	{
		mSetText = false;
		if (mSoul != null)
		{
			mSoul.KnownSpells.Clear();
			mSoul = null;
		}
	}

	private void AddZeroRankSpells()
	{
		const int zero = 0;
		List<Spell> rankZeroList = new List<Spell>();
		var schools = Enum.GetValues(typeof(DnDMagicSchool)).Cast<DnDMagicSchool>();
		foreach (DnDMagicSchool school in schools)
		{
			if (!mSoul.ForbiddenSchools.Contains(school))
			{
				List<Spell> tempList = AppStorage.Instance.SpellList.GetWizardSchoolSpecializationSpells(school, zero);
				rankZeroList.AddRange(tempList);
			}
		}
		if (mSoul.KnownSpells.Count > 0)
		{
			mSoul.KnownSpells[zero] = rankZeroList;
		}
		else
		{
			mSoul.KnownSpells.Add(rankZeroList);
		}
	}
}
}
