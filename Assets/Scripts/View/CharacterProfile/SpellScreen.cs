using System;
using System.Collections.Generic;
using SpellMastery.View.Util;
using UnityEngine;
using SpellMastery.Control;
using SpellMastery.Model.DnD;

namespace SpellMastery.View
{
	public abstract class SpellScreen : ScrollViewScreen, IButtonHandler
	{
		protected List<GameObject> mButtonList = new List<GameObject>();
		protected bool mShouldUpdate = true;
		protected int mSelectedClass = -1;
		protected int mSelectedRank = -1;
		protected int mSelectedSpell = -1;
		protected int mSelectedSpellSender = -1;

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

		public void ButtonCLickHandler(int notification, int senderID)
		{
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

		private void ExitScreen()
		{
			if (PrevScreen != null)
			{
				mSelectedRank = -1;
				mSelectedClass = -1;
				PrevScreen.SetActive(true);
				gameObject.SetActive(false);
			}
		}

		private void InitClassSelection()
		{
			throw new NotImplementedException();
		}

		private void InitRankSelection()
		{
			throw new NotImplementedException();
		}

		protected abstract void InitSpellSelection();
	}
}
