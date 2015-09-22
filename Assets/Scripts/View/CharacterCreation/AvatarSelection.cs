using System;
using SpellMastery.View.Util;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpellMastery.Control;

namespace SpellMastery.View
{
	public class AvatarSelection : ScrollViewScreen, IButtonHandler
	{
		private const string cPrefabName = "Prefabs/HorizontalButtonGroup";
		private const string cImagePath = "img/avatars/{0}";
		private const string cLoadingText = "loading avatars...";
		private const string cTitleText = "Available images:";

		private List<string> mImageList = ViewUtility.AvatarList;

		private int mPreviousSelected = 0;
		private List<GameObject> mButtons = new List<GameObject>();

		public CreationStorage Storage;
		public Text Title;
		public GameObject LoadingSymbol;
		public CharacterOptionsScreen NextScreen;

		public List<GameObject> ButtonList
		{
			get { return mButtons; }
			set { mButtons = value; }
		}

		public void ButtonCLickHandler(int notification, int senderID, GameObject sender)
		{
			UpdateSelectionBorder(notification);
		}

		public override void Next()
		{
			if (NextScreen == null && Storage != null)
			{
				Storage.Character.Avatar = string.Format(cImagePath, mImageList[mPreviousSelected]);
				AppStorage.Instance.SaveNewCharacter(Storage.Character);
				SwitchScene(SCENE_SPELLMASTERY);
			}
			else if (NextScreen != null)
			{
				if (AppStorage.Instance.CurrentCharacter != null)
				{
					AppStorage.Instance.CurrentCharacter.Avatar = string.Format(cImagePath, mImageList[mPreviousSelected]);
					AppStorage.Instance.SaveCharacters();
					NextScreen.ShouldUpdate = true;
				}
				NextScreen.gameObject.SetActive(true);
				gameObject.SetActive(false);
			}
		}

		public override void Back()
		{
			Next();
		}

		public override void Update()
		{
			base.Update();
			if (!mInitialized)
			{
				InitButtons();
			}
		}

		private void UpdateSelectionBorder(int selected)
		{
			mButtons[mPreviousSelected].transform.FindChild("Border").gameObject.SetActive(false);
			mButtons[selected].transform.FindChild("Border").gameObject.SetActive(true);
			mPreviousSelected = selected;
		}

		private IEnumerator SetButtons()
		{
			if (mButtons.Count < mImageList.Count)
			{
				Debug.LogError("something went wrong when creating the buttons!");
			}
			if (AppStorage.Instance.CurrentCharacter == null)
			{
				UpdateSelectionBorder(0);
			}
			else
			{
				int index = mImageList.FindIndex(x => x == AppStorage.Instance.CurrentCharacter.Avatar);
				if (index >= 0)
				{
					UpdateSelectionBorder(index);
				}
				else
				{
					UpdateSelectionBorder(0);
				}
			}
			for (int i = 0; i < mButtons.Count; ++i)
			{
				if (i < mImageList.Count)
				{
					string item = string.Format(cImagePath, mImageList[i]);
					ResourceRequest request = Resources.LoadAsync<Texture2D>(item);
					do
					{
						yield return new WaitForSeconds(0.01f);
					}
					while (!request.isDone);
					Texture2D tex = request.asset as Texture2D;
					mButtons[i].GetComponent<Image>().sprite = ViewUtility.CreateSpriteUseTexture(tex.EncodeToPNG(),mButtons[i].GetComponent<Image>());
				}
				else
				{
					mButtons[i].SetActive(false);
				}
			}
			Title.text = cTitleText;
			LoadingSymbol.SetActive(false);
		}

		public void DeInitButtons()
		{
			foreach (var button in mButtons)
			{
				GameObject.Destroy(button);
			}
			mButtons.Clear();
		}

		public void InitButtons()
		{
			Title.text = cLoadingText;
			LoadingSymbol.SetActive(true);
			if (mViewHeight.HasValue)
			{
				int columns = 2;
				int height = 0;
				GameObject buttonPref = Resources.Load(cPrefabName) as GameObject;
				int buttonHeight = (int)buttonPref.GetComponent<LayoutElement>().minHeight;
				int spacing = (int)DragArea.transform.FindChild("ScrollArea").GetComponent<VerticalLayoutGroup>().spacing;
				buttonHeight += spacing;
				int numberOfGroups = (int)Math.Ceiling(mImageList.Count / (double)columns);
				for (int i = 0; i < numberOfGroups; ++i)
				{
					GameObject group = GameObject.Instantiate(buttonPref) as GameObject;
					group.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					for (int j = 0; j < columns; ++j)
					{
						GameObject button = group.transform.GetChild(j).gameObject;
						button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
						button.GetComponent<IntButtonHandler>().NotificationInt = i * columns + j;
						mButtons.Add(button);
					}
					height += buttonHeight;
				}
				if (height > mViewHeight.Value)
				{ // removing the spacing from the last button for nicer view
					height -= spacing;
				}
				mTotalHeight = height;
				StartCoroutine(SetButtons());
			}
		}
	}
}
