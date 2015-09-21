using System;
using System.Collections.Generic;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public class DnDWizardCreationSpellsRanks : ScrollViewScreen, IButtonHandler
	{
		private const string cButtonPrefab = "Prefabs/SimpleButton";
		private const int cNextScreen = 7;
		private const int cPrevScreen = 5;
		private const string cRankButtonText = "R ank {0}  [{1}]";

		private List<GameObject> mButtons = new List<GameObject>();
		private int mNumberOfButtons = 0;
		private DnDWizard mWizard = null;

		public CreationStorage Storage;

		public List<GameObject> ButtonList
		{
			get { return mButtons; }
			set { mButtons = value; }
		}

		public void ButtonCLickHandler(int notification, int senderID)
		{
			if (!ScrollView.IsDragged)
			{
				if (mButtons.Count > notification)
				{
					Storage.Screens[cNextScreen].GetComponent<DnDWizardCreationSpellsSelectSpells>().SelectedRank = notification + 1;
					Storage.Screens[cNextScreen].SetActive(true);
					DeInitButtons();
					Reset();
					this.gameObject.SetActive(false);
				}
			}
		}

		public override void Next()
		{ // there shouldn't be any button that calls this.
			throw new NotImplementedException();
		}

		public override void Back()
		{
			base.Back();
			Storage.Screens[cPrevScreen].SetActive(true);
			Reset();
			DeInitButtons();
			this.gameObject.SetActive(false);
		}

		public override void Update()
		{
			if (mWizard == null)
			{
				mWizard = (DnDWizard)((DnDCharacter)Storage.Character).Classes.Find(x => x.CharacterClass == DnDCharClass.Wizard);
			}
			if (!mInitialized)
			{
				if (mWizard != null)
				{
					mNumberOfButtons = mWizard.HighestCastableRank; // Rank 0 is not needed (otherwhise would be +1)
					InitButtons();
				}
			}
			base.Update();
		}

		public void InitButtons()
		{
			if (mViewHeight.HasValue)
			{
				int height = 0;
				GameObject buttonPref = Resources.Load(cButtonPrefab) as GameObject;
				int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
				buttonHeight += spacing;
				for (int i = 0; i < mNumberOfButtons; ++i)
				{
					GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
					button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					button.GetComponent<IntButtonHandler>().NotificationInt = i;
					button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
					int number = mWizard.NumberofUnknownSpells(i + 1);
					ViewUtility.ChangeSimpleButtonText(string.Format(cRankButtonText, i + 1, number), button);
					ViewUtility.EnableButton(button, number != 0);
					mButtons.Add(button);
					height += buttonHeight;
				}
				if (height > mViewHeight.Value)
				{ // removing the spacing from the last button for nicer view
					height -= spacing;
				}
				mTotalHeight = height;
			}
		}

		public void DeInitButtons()
		{
			foreach (var button in mButtons)
			{
				GameObject.Destroy(button);
			}
			mButtons.Clear();
		}
	}
}
