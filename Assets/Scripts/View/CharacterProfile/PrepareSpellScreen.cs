using SpellMastery.Control;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public class PrepareSpellScreen : SpellScreen
	{
		private const string cDualPrefab = "Prefabs/DualButton";

		private List<Spell> mCurrentSpells;

		public override void Next()
		{
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (character.Classes.Count < mSelectedClass)
				return;
			DnDClassSoul soul = character.Classes[mSelectedClass];
			if (soul.KnownSpells.Count < mSelectedRank)
				return;
			List<Spell> rank = soul.KnownSpells[mSelectedRank];
			if (rank.Count < mSelectedSpell)
				return;
			Spell spell = rank[mSelectedSpell];
			if (mSelectedSpellSender == cRegularSpell)
			{
				soul.PrepareMainSpell(spell, mSelectedRank);
			}
			else if (mSelectedSpellSender == cExtraSpell)
			{
				soul.PrepareExtraSpell(spell, mSelectedRank);
			}
		}

		protected override void InitClassSelection()
		{
			SetTopText("Select a class");
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			GameObject buttonPref = Resources.Load(cTwoPartPrefab) as GameObject;
			int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
			int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
			buttonHeight += spacing;
			for (int i = 0; i < character.Classes.Count; ++i)
			{
				if (character.Classes[i].NeedsToPrepareSpells)
				{
					GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
					button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					ViewUtility.ChangeTwoPartButtonText(character.Classes[i].CharacterClass.ToString(), "level  " + character.Classes[i].ClassLevel, button);
					button.GetComponent<IntButtonHandler>().NotificationInt = i;
					button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
					mTotalHeight += buttonHeight;
					mButtonList.Add(button);
				}
			}
			if (mViewHeight < mTotalHeight)
			{
				mTotalHeight -= spacing;
			}
			if (mButtonList.Count == 1)
			{
				int selected = mButtonList[0].GetComponent<IntButtonHandler>().NotificationInt;
				int sender = mButtonList[0].GetComponent<IntButtonHandler>().ButtonID;
				ButtonCLickHandler(selected, sender, gameObject);
			}
			UpdateNow = true;
		}

		protected override void InitRankSelection()
		{
			SetTopText("Select the spell Rank");
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (mSelectedClass > character.Classes.Count)
				return;
			DnDClassSoul soul = character.Classes[mSelectedClass];

			GameObject buttonPref = Resources.Load(cSimplePrefab) as GameObject;
			int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
			int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
			buttonHeight += spacing;
			for (int i = 0; i < soul.KnownSpells.Count; ++i)
			{
				GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
				button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeSimpleButtonText("rank  " + i, button);
				button.GetComponent<IntButtonHandler>().NotificationInt = i;
				button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
				mTotalHeight += buttonHeight;
				ViewUtility.EnableSimpleButton(button, soul.KnownSpells[i].Count > 0);
				mButtonList.Add(button);
			}
			if (mViewHeight < mTotalHeight)
			{
				mTotalHeight -= spacing;
			}
			UpdateNow = true;
		}

		protected override void InitSpellSelection()
		{
			SetTopText("Prepare spells", GetExtraTopText());
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (character.Classes.Count < mSelectedClass)
				return;
			DnDClassSoul soul = character.Classes[mSelectedClass];
			if (soul.KnownSpells.Count < mSelectedRank)
				return;

			mCurrentSpells = soul.KnownSpells[mSelectedRank];
			mCurrentSpells.Sort();
			if (soul.CanCastExtraSpell)
			{ // double buttons
				InitDualButtons();
			}
			else
			{ // single buttons
				InitSimpleButtons();
			}
		}

		private void InitDualButtons()
		{
			GameObject buttonPref = Resources.Load(cDualPrefab) as GameObject;
			int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
			int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
			buttonHeight += spacing;
			for (int i = 0; i < mCurrentSpells.Count; ++i)
			{
				GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
				button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeDualButtonText(button, mCurrentSpells[i].ToShortString());
				button.transform.FindChild("ButtonLeft").GetComponent<IntButtonHandler>().NotificationInt = i;
				button.transform.FindChild("ButtonLeft").GetComponent<IntButtonHandler>().NotificationCatcher = this;
				button.transform.FindChild("ButtonRight").GetComponent<IntButtonHandler>().NotificationInt = i;
				button.transform.FindChild("ButtonRight").GetComponent<IntButtonHandler>().NotificationCatcher = this;
				mTotalHeight += buttonHeight;
				mButtonList.Add(button);
			}
			if (mViewHeight < mTotalHeight)
			{
				mTotalHeight -= spacing;
			}
			UpdateNow = true;
		}

		private void InitSimpleButtons()
		{
			GameObject buttonPref = Resources.Load(cSimplePrefab) as GameObject;
			int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
			int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
			buttonHeight += spacing;
			for (int i = 0; i < mCurrentSpells.Count; ++i)
			{
				GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
				button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeSimpleButtonText(mCurrentSpells[i].ToShortString(), button);
				button.GetComponent<IntButtonHandler>().NotificationInt = i;
				button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
				mTotalHeight += buttonHeight;
				mButtonList.Add(button);
			}
			if (mViewHeight < mTotalHeight)
			{
				mTotalHeight -= spacing;
			}
			UpdateNow = true;
		}

		private string GetExtraTopText()
		{
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (character.Classes.Count < mSelectedClass)
				return "";
			DnDClassSoul soul = character.Classes[mSelectedClass];
			if (soul.KnownSpells.Count < mSelectedRank)
				return "";

			string result = "p / u";
			if (soul.CanCastExtraSpell)
			{
				result += " / +";
			}
			int prepared = soul.NumberOfPreparedMainSpells(mSelectedRank);
			int unused = soul.SpellsPerDay()[mSelectedRank] - prepared;
			result += "\n" + prepared + "  /  " + unused;
			if (soul.CanCastExtraSpell)
			{
				int extra = soul.NumberOfPreparedExtraSpells(mSelectedRank);
				result += "  /  " + extra;
			}
			return result;
		}
	}
}
