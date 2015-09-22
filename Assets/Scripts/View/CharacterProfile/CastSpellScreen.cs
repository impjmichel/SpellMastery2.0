using SpellMastery.Control;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public class CastSpellScreen : SpellScreen
	{
		private const string cLabelPrefab = "Prefabs/SimpleLabel";

		private DnDClassSoul mSoul;

		public override void Update()
		{
			base.Update();
			if (mSoul != null)
			{
				if (!mSoul.NeedsToPrepareSpells && mSelectedClass >= 0 && mSelectedRank >= 0)
				{
					int spellsPerDay = mSoul.SpellsPerDay()[mSelectedRank];
					while (mSoul.MainSpells.Count < mSelectedRank)
					{
						mSoul.MainSpells.Add(new List<KeyValuePair<Spell, bool>>());
					}
					if (mSoul.MainSpells[mSelectedRank].Count == spellsPerDay)
					{
						foreach (var button in mButtonList)
						{
							ViewUtility.EnableSimpleButton(button, false);
						}
					}
				}
			}
		}

		public override void Next()
		{
			if (mSoul.NeedsToPrepareSpells)
			{
				if (mSoul.KnownSpells.Count < mSelectedRank)
					return;

				if (mSelectedSpellSender == cRegularSpell)
				{
					Spell spell = mSoul.MainSpells[mSelectedRank][mSelectedSpell].Key;
					mSoul.MainSpells[mSelectedRank][mSelectedSpell] = new KeyValuePair<Spell, bool>(spell, true);
				}
				else
				{
					Spell eSpell = mSoul.ExtraSpells[mSelectedRank].Key;
					mSoul.ExtraSpells[mSelectedRank] = new KeyValuePair<Spell, bool>(eSpell, true);
				}
				ViewUtility.EnableSimpleButton(mLastClicked, false);
			}
			else
			{
				while (mSoul.MainSpells.Count < mSelectedRank)
				{
					mSoul.MainSpells.Add(new List<KeyValuePair<Spell, bool>>());
				}
				mSoul.MainSpells[mSelectedRank].Add(new KeyValuePair<Spell, bool>());
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
				GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
				button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeTwoPartButtonText(character.Classes[i].CharacterClass.ToString(), "level  " + character.Classes[i].ClassLevel, button);
				button.GetComponent<IntButtonHandler>().NotificationInt = i;
				button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
				mTotalHeight += buttonHeight;
				mButtonList.Add(button);
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
			mSoul = character.Classes[mSelectedClass];

			GameObject buttonPref = Resources.Load(cSimplePrefab) as GameObject;
			int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
			int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
			buttonHeight += spacing;
			for (int i = 0; i < mSoul.KnownSpells.Count; ++i)
			{
				GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
				button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeSimpleButtonText("rank  " + i, button);
				button.GetComponent<IntButtonHandler>().NotificationInt = i;
				button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
				mTotalHeight += buttonHeight;
				ViewUtility.EnableSimpleButton(button, mSoul.KnownSpells[i].Count > 0);
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
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (character.Classes.Count < mSelectedClass)
				return;
			mSoul = character.Classes[mSelectedClass];

			if (mSoul.NeedsToPrepareSpells)
			{
				SetTopText("Cast spells", "");
				InitPreparedSpells();
			}
			else
			{
				SetTopText("Cast spells", GetExtraTopText());
				InitKnownSpells();
			}
			int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
			if (mViewHeight < mTotalHeight)
			{
				mTotalHeight -= spacing;
			}
		}

		/// <summary>
		/// for clerics, wizards and other hard learners
		/// </summary>
		private void InitPreparedSpells()
		{
			int mainSpells = 0;
			bool extraSpell = false;
			if (mSoul.MainSpells.Count > mSelectedRank)
			{
				mainSpells = mSoul.MainSpells[mSelectedRank].Count;
			}
			if (mSoul.CanCastExtraSpell && mSoul.ExtraSpells.Count > mSelectedRank)
			{
				if (mSoul.ExtraSpells[mSelectedRank].Key != null)
				{
					extraSpell = true;
				}
			}
			GameObject buttonPref = null;
			int buttonHeight = 0;
			if (mainSpells == 0 && !extraSpell)
			{
				buttonPref = Resources.Load(cLabelPrefab) as GameObject;
				buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				mTotalHeight += buttonHeight;
				// display label
				GameObject obj = GameObject.Instantiate(buttonPref) as GameObject;
				obj.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeSimpleButtonText("no spells prepared for this rank", obj);
				mButtonList.Add(obj);
			}
			if (mainSpells > 0)
			{
				buttonPref = Resources.Load(cSimplePrefab) as GameObject;
				buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				// for loop to add main spells
				for (int i = 0; i < mainSpells; ++i)
				{
					GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
					button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					ViewUtility.ChangeSimpleButtonText(mSoul.MainSpells[mSelectedRank][i].Key.ToShortString(), button);
					button.GetComponent<IntButtonHandler>().NotificationInt = i;
					button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
					mTotalHeight += buttonHeight;
					ViewUtility.EnableSimpleButton(button, !mSoul.MainSpells[mSelectedRank][i].Value);
					mButtonList.Add(button);
				}
			}
			if (extraSpell)
			{
				buttonPref = Resources.Load(cTwoPartPrefab) as GameObject;
				buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				// add extra spell button
				GameObject eButton = GameObject.Instantiate(buttonPref) as GameObject;
				eButton.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeTwoPartButtonText(mSoul.ExtraSpells[mSelectedRank].Key.ToShortString(), "(extra)" , eButton);
				eButton.GetComponent<IntButtonHandler>().NotificationInt = mSelectedRank;
				eButton.GetComponent<IntButtonHandler>().ButtonID = cExtraSpell;
				eButton.GetComponent<IntButtonHandler>().NotificationCatcher = this;
				mTotalHeight += buttonHeight;
				ViewUtility.EnableSimpleButton(eButton, !mSoul.ExtraSpells[mSelectedRank].Value);
				mButtonList.Add(eButton);
			}
		}

		/// <summary>
		/// for sorcerers and other swaggers
		/// </summary>
		private void InitKnownSpells()
		{
			int mainSpells = 0;
			if (mSoul.MainSpells.Count > mSelectedRank)
			{
				mainSpells = mSoul.KnownSpells[mSelectedRank].Count;
			}
			GameObject buttonPref = null;
			int buttonHeight = 0;
			if (mainSpells == 0)
			{
				buttonPref = Resources.Load(cLabelPrefab) as GameObject;
				buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				mTotalHeight += buttonHeight;
				// display label
				GameObject obj = GameObject.Instantiate(buttonPref) as GameObject;
				obj.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
				ViewUtility.ChangeSimpleButtonText("You know no spells of this rank", obj);
				mButtonList.Add(obj);
			}
			if (mainSpells > 0)
			{
				buttonPref = Resources.Load(cSimplePrefab) as GameObject;
				buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				// for loop to add main spells
				for (int i = 0; i < mainSpells; ++i)
				{
					GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
					button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					ViewUtility.ChangeSimpleButtonText(mSoul.KnownSpells[mSelectedRank][i].ToShortString(), button);
					button.GetComponent<IntButtonHandler>().NotificationInt = i;
					button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
					mTotalHeight += buttonHeight;
					mButtonList.Add(button);
				}
			}
		}

		private string GetExtraTopText()
		{
			string result = "";

			return result;
		}
	}
}
