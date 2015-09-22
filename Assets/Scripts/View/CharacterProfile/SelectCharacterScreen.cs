using System;
using System.Collections.Generic;
using SpellMastery.Control;
using SpellMastery.View.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public class SelectCharacterScreen : ScrollViewScreen, IButtonHandler
	{
		private const string cButtonPrefab = "Prefabs/TwoPartButton";

		private List<GameObject> mButtons = new List<GameObject>();

		public CharacterOptionsScreen NextScreen;

		public List<GameObject> ButtonList
		{
			get { return mButtons; }
			set { mButtons = value; }
		}

		public void ButtonCLickHandler(int notification, int senderID, GameObject sender)
		{
			AppStorage.Instance.CurrentCharacter = AppStorage.Instance.Characters[notification];
			Next();
		}

		public override void Next()
		{
			if (LoadingScreen != null)
			{
				LoadingScreen.SetActive(true);
			}
			NextScreen.gameObject.SetActive(true);
			StartCoroutine(NextScreen.Init(gameObject));
		}

		public void OnExit()
		{
			DeInitButtons();
			Reset();
			gameObject.SetActive(false);
		}

		public override void Back()
		{
			SwitchScene(SCENE_SPELLMASTERY);
			base.Back();
		}

		public override void Update()
		{
			base.Update();
			if (!mInitialized)
			{
				InitButtons();
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
				for (int i = 0; i < AppStorage.Instance.Characters.Count; ++i)
				{
					GameObject button = GameObject.Instantiate(buttonPref) as GameObject;
					button.transform.SetParent(DragArea.transform.FindChild("ScrollArea"));
					button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
					button.GetComponent<IntButtonHandler>().NotificationInt = i;
					string gameText = ViewUtility.ToViewString(AppStorage.Instance.Characters[i].Game);
					ViewUtility.ChangeTwoPartButtonText(AppStorage.Instance.Characters[i].Name, gameText, button);
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
