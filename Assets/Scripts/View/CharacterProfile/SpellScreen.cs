using System;
using System.Collections.Generic;
using SpellMastery.View.Util;
using UnityEngine;
using SpellMastery.Control;
using SpellMastery.Model.DnD;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public abstract class SpellScreen : ScrollViewScreen, IButtonHandler
	{
		protected const int cRegularSpell = 0;
		protected const int cExtraSpell = 1;
		protected const string cTwoPartPrefab = "Prefabs/TwoPartButton";
		protected const string cSimplePrefab = "Prefabs/SimpleButton";

		protected List<GameObject> mButtonList = new List<GameObject>();
		protected bool mShouldUpdate = true;
		protected int mSelectedClass = -1;
		protected int mSelectedRank = -1;
		protected int mSelectedSpell = -1;
		protected int mSelectedSpellSender = -1;
		protected GameObject mLastClicked;

		public GameObject PrevScreen;

		public List<GameObject> ButtonList
		{
			get
			{
				return mButtonList;
			}
			set
			{
				mButtonList = value;
			}
		}

		public void ButtonCLickHandler(int notification, int senderID, GameObject sender)
		{
			mLastClicked = sender;
			if (mSelectedClass < 0)
			{
				mSelectedClass = notification;
			}
			else
			{
				if (mSelectedRank < 0)
				{
					mSelectedRank = notification;
				}
				else
				{
					mSelectedSpell = notification;
					mSelectedSpellSender = senderID;
					Next();
				}
			}
			mShouldUpdate = true;
		}

		public void DeInitButtons()
		{
			foreach (GameObject go in mButtonList)
			{
				GameObject.Destroy(go);
			}
			mButtonList.Clear();
		}

		public void InitButtons()
		{
			DeInitButtons();
			Reset();
			mTotalHeight = 0;
			if (mSelectedRank >= 0)
			{
				InitSpellSelection();
			}
			else
			{
				if (mSelectedClass >= 0)
				{
					InitRankSelection();
				}
				else
				{
					if (AppStorage.Instance.CurrentCharacter.GetType() == typeof(DnDCharacter))
					{
						if (((DnDCharacter)AppStorage.Instance.CurrentCharacter).IsMultiClass)
						{
							InitClassSelection();
						}
						else
						{
							mSelectedClass = 0;
							InitRankSelection();
						}
					}
					else
					{
						throw new NotImplementedException();
					}
				}
			}
			UpdateNow = true;
		}

		public override void Back()
		{
			base.Back();
			if (mSelectedRank < 0)
			{
				if (mSelectedClass < 0)
				{
					ExitScreen();
				}
				else
				{
					if (AppStorage.Instance.CurrentCharacter.GetType() == typeof(DnDCharacter))
					{
						if (((DnDCharacter)AppStorage.Instance.CurrentCharacter).IsMultiClass)
						{
							mSelectedClass = -1;
						}
						else
						{
							ExitScreen();
						}
					}
				}
			}
			else
			{
				mSelectedRank = -1;
			}
			mShouldUpdate = true;
		}

		public override void Update()
		{
			base.Update();
			if (mShouldUpdate)
			{
				mShouldUpdate = false;
				InitButtons();
			}
		}

		protected void SetTopText(string main, string extra = "")
		{
			Text text = transform.FindChild("Top").FindChild("TextLeft").GetComponent<Text>();
			text.text = main;
			text = transform.FindChild("Top").FindChild("TextRight").GetComponent<Text>();
			text.text = extra;
		}

		private void ExitScreen()
		{
			if (PrevScreen != null)
			{
				mSelectedRank = -1;
				mSelectedClass = -1;
				PrevScreen.GetComponent<CharacterOptionsScreen>().ShouldUpdate = true;
				PrevScreen.SetActive(true);
				gameObject.SetActive(false);
			}
		}

		protected abstract void InitClassSelection();

		protected abstract void InitRankSelection();

		protected abstract void InitSpellSelection();
	}
}
