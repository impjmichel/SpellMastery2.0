using System;
using System.Collections.Generic;
using System.Linq;
using SpellMastery.Control;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public class DnDWizardCreationSpellsSelectSpells : ScrollViewScreen, IButtonHandler
	{
		private const int cPrevScreen = 6;
		private const string cButtonPrefab = "Prefabs/SimpleButton";

		private List<Spell> mSpellsToDisplay = new List<Spell>();
		private List<GameObject> mButtons = new List<GameObject>();

		public CreationStorage Storage;
		public int SelectedRank = -1;

		public List<GameObject> ButtonList
		{
			get { return mButtons; }
			set { mButtons = value; }
		}

		public void ButtonCLickHandler(int notification, int senderID, GameObject sender)
		{
			DnDWizard wizard = (DnDWizard)((DnDCharacter)Storage.Character).Classes.Find(x => x.CharacterClass == DnDCharClass.Wizard);
			if (wizard != null && SelectedRank >= 0)
			{
				// add spell:
				wizard.AddSpellToKnownList(mSpellsToDisplay[notification], SelectedRank);
				// disable button:
				ViewUtility.EnableSimpleButton(mButtons[notification], false);
			}
		}

		public override void Next()
		{ // there shouldn't be any Next button.
			throw new NotImplementedException();
		}

		public override void Back()
		{
			base.Back();
			Storage.Screens[cPrevScreen].SetActive(true);
			mSpellsToDisplay.Clear();
			DeInitButtons();
			Reset();
			SelectedRank = -1;
			this.gameObject.SetActive(false);
		}

		// Update is called once per frame
		public override void Update()
		{
			base.Update();
			if (!mInitialized)
			{
				if (SelectedRank >= 0)
				{
					DnDWizard wizard = (DnDWizard)((DnDCharacter)Storage.Character).Classes.Find(x => x.CharacterClass == DnDCharClass.Wizard);
					if (wizard != null)
					{
						var schools = Enum.GetValues(typeof(DnDMagicSchool)).Cast<DnDMagicSchool>();
						List<DnDMagicSchool> castable = schools.ToList();
						castable.RemoveAll(x => wizard.ForbiddenSchools.Contains(x));
						mSpellsToDisplay = new List<Spell>();
						foreach (DnDMagicSchool school in castable)
						{
							mSpellsToDisplay.AddRange(AppStorage.Instance.SpellList.GetWizardSchoolSpecializationSpells(school, SelectedRank));
						}
						if (wizard.KnownSpells.Count > SelectedRank)
							mSpellsToDisplay.RemoveAll(x => wizard.KnownSpells[SelectedRank].Find(y => y.Equals(x)) != null);
						mSpellsToDisplay.Sort();
						InitButtons();
					}
				}
			}
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
				for (int i = 0; i < mSpellsToDisplay.Count; ++i)
				{
					GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
					button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					button.GetComponent<IntButtonHandler>().NotificationInt = i;
					button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
					string text = ViewUtility.MakeStringVivaldiViable(mSpellsToDisplay[i].ToShortString());
					ViewUtility.ChangeSimpleButtonText(text, button);
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
