using SpellMastery.Control;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
	public class CharacterOptionsScreen : ScreenBase
	{
		private List<Button> mButtons = new List<Button>();
		private GameObject mNextScreen;
		private bool mIsDirty = false;
		private bool mFirstTime = true;

		public GameObject ProfileScreen;
		public GameObject CastRankScreen;
		public GameObject PrepareRankScreen;
		public GameObject PrevScreen;
		public Image AvatarImage;
		public Text NameText;

		public bool ShouldUpdate
		{
			get { return mIsDirty; }
			set { mIsDirty = value; }
		}

		public IEnumerator Init(GameObject caller)
		{
			if (mFirstTime)
			{
				mFirstTime = false;
				float current = NameText.transform.parent.GetComponent<LayoutElement>().minHeight;
				NameText.transform.parent.GetComponent<LayoutElement>().minHeight = current * 0.18f * (Screen.height / 100f);
			}
			yield return StartCoroutine(UpdateAvatar());
			gameObject.SetActive(true);
			caller.GetComponent<SelectCharacterScreen>().OnExit();
			LoadingScreen.SetActive(false);
			UpdateName();
			SetButtons();
		}

		public void UpdateName()
		{
			NameText.text = AppStorage.Instance.CurrentCharacter.Name;
		}

		public void SetButtons()
		{
			// Avatar button:
			Button button = transform.FindChild("Top").GetComponentInChildren<Button>();
			button.onClick.AddListener(() =>
			{
				mNextScreen = ProfileScreen;
				Next();
			});
			mButtons.Add(button);
			// prepare button:
			button = transform.FindChild("Menu").FindChild("PrepareButton").GetComponent<Button>();
			if (AppStorage.Instance.CurrentCharacter.NeedsToPrepareSpells)
			{
				button.onClick.AddListener(() =>
				{
					mNextScreen = PrepareRankScreen;
					Next();
				});
			}
			else
			{
				button.gameObject.SetActive(false);
			}
			mButtons.Add(button);
			// cast button:
			button = transform.FindChild("Menu").FindChild("CastButton").GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				mNextScreen = CastRankScreen;
				Next();
			});
			mButtons.Add(button);
		}

		public override void Back()
		{
			if (PrevScreen != null)
			{
				DeInit();
				AppStorage.Instance.CurrentCharacter = null;
				PrevScreen.SetActive(true);
				gameObject.SetActive(false);
			}
		}

		public override void Next()
		{
			if (mNextScreen != null)
			{
				DeInit();
				mNextScreen.SetActive(true);
				gameObject.SetActive(false);
			}
		}

		public override void Update()
		{
			base.Update();
			if (mIsDirty)
			{
				mIsDirty = false;
				StartCoroutine(UpdateAvatar());
				UpdateName();
				DeInit();
				SetButtons();
			}
		}

		private void DeInit()
		{
			foreach (Button button in mButtons)
			{
				button.onClick.RemoveAllListeners();
				button.gameObject.SetActive(true);
			}
			mButtons.Clear();
		}

		private IEnumerator UpdateAvatar()
		{
			ResourceRequest request = Resources.LoadAsync<Texture2D>(AppStorage.Instance.CurrentCharacter.Avatar);
			do
			{
				yield return new WaitForSeconds(.01f);
			}
			while (!request.isDone);
			Texture2D tex = request.asset as Texture2D;
			AppStorage.Instance.CurrentCharacter.AvatarBytes = tex.EncodeToPNG();
			AvatarImage.sprite = ViewUtility.CreateSpriteUseTexture(AppStorage.Instance.CurrentCharacter.AvatarBytes, AvatarImage);
			yield return new WaitForEndOfFrame();
		}
	}
}
