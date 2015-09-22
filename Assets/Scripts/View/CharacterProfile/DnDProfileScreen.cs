using UnityEngine;
using SpellMastery.View.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using SpellMastery.Control;
using SpellMastery.Model;
using SpellMastery.Model.DnD;

namespace SpellMastery.View
{
	public class DnDProfileScreen : ScrollViewScreen, IButtonHandler
	{
		private const int cNameId = 0;
		private const int cExpId = 1;
		private const int cAttributesId = 2;
		private const int cGenderId = 3;
		private const int cRaceId = 4;
		private const int cAlignmentId = 5;
		/// <summary>
		/// this should be manually updated with every addition
		/// </summary>
		private const int cMaxItemId = 5;

		private const string cGenderText = "Gender:  {0}";
		private const string cRaceText = "Race:  {0}";
		private const string cView = "View";
		private const string cInput = "Input";

		private int mPreviousSelected = -1;
		private int mPrePreviousSelected = -1;
		private bool mFirstTime = true;
		private List<string> mAlignmentPaths = new List<string>()
		{
			"img/arrow/LG",
			"img/arrow/NG",
			"img/arrow/CG",
			"img/arrow/LN",
			"img/arrow/NN",
			"img/arrow/CN",
			"img/arrow/LE",
			"img/arrow/NE",
			"img/arrow/CE"
		};
		private Text mAlignmentText;
		private Text mRaceText;

		/// <summary>
		/// Attach in Unity
		/// </summary>
		public List<GameObject> mButtons;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public GameObject CharacterOptionScreen;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public GameObject AvatarSelectionScreen;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public AlignmentGrid AlignmentGrid;

		/// <summary>
		/// IButtonHandler interface list.
		/// </summary>
		public List<GameObject> ButtonList
		{
			get
			{
				return mButtons;
			}
			set
			{
				mButtons = value;
			}
		}

		public override void Next()
		{ // using this for "to Avatar selection screen"
			if (AvatarSelectionScreen != null)
			{
				ButtonCLickHandler(mPreviousSelected, 0, gameObject);
				AvatarSelectionScreen.SetActive(true);
				Reset();
				DeInitButtons();
				gameObject.SetActive(false);
			}
		}

		public override void Back()
		{
			base.Back();
			if (CharacterOptionScreen != null)
			{
				ButtonCLickHandler(mPreviousSelected, 0, gameObject);
				CharacterOptionScreen.GetComponent<CharacterOptionsScreen>().ShouldUpdate = true;
				CharacterOptionScreen.SetActive(true);
				Reset();
				DeInitButtons();
				gameObject.SetActive(false);
			}
		}

		// Update is called once per frame
		public override void Update()
		{
			base.Update();
			if (!mInitialized && mViewHeight.HasValue)
			{
				if (isActiveAndEnabled)
				{
					StartCoroutine(InitView());
					InitButtons();
				}
			}
			if (mPreviousSelected == cAlignmentId)
			{ // updating alignment images:
				if (mAlignmentText == null)
				{
					mAlignmentText = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Alignment").FindChild(cInput).FindChild("Top").FindChild("BoxSetText").GetComponent<Text>();
				}
				mAlignmentText.text = AlignmentGrid.SelectedAlignment.ToString();
			}
			if (mPreviousSelected == cRaceId)
			{ // updating race slider text:
				if (mRaceText == null)
				{
					mRaceText = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Race").FindChild(cInput).FindChild("SliderText").GetComponent<Text>();
				}
				mRaceText.text = ((DnDRace)(int)mRaceText.transform.parent.FindChild("Slider").GetComponent<Slider>().value).ToString();
			}
		}

		public void ButtonCLickHandler(int notification, int senderID, GameObject sender)
		{
			if (notification < 0)
				return;
			if (mPreviousSelected >= 0)
			{
				Activate(false, mPreviousSelected);
				SaveItem(mPreviousSelected);
			}
			if (notification != mPreviousSelected)
			{
				Activate(true, notification);
				SelectInput(notification);
			}
			else
			{
				if (notification == mPrePreviousSelected)
				{
					Activate(true, notification);
					SelectInput(notification);
					SaveItem(mPreviousSelected);
					mPreviousSelected = -1;
				}
			}
			mPrePreviousSelected = mPreviousSelected;
			mPreviousSelected = notification;
		}

		public void InitButtons()
		{
			foreach (var button in mButtons)
			{
				button.GetComponent<IntButtonHandler>().NotificationCatcher = this;
			}
		}

		public void DeInitButtons()
		{
		}

		private void Activate(bool enable, int notification)
		{
			Transform currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").GetChild(notification);
			if (currentParent != null)
			{
				currentParent.FindChild(cInput).gameObject.SetActive(enable);
				currentParent.FindChild(cView).gameObject.SetActive(!enable);
			}
		}

		private void SaveItem(int notification, bool write = true)
		{
			Transform parent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").GetChild(notification);
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			switch (notification)
			{
				case cNameId:
					string newName = parent.FindChild(cInput).GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newName))
					{
						character.Name = newName;
						parent.FindChild(cInput).FindChild("Placeholder").GetComponent<Text>().text = newName;
						parent.FindChild(cView).GetComponent<Text>().text = newName;
					}
					break;
				case cExpId:
					string currentText = parent.FindChild(cInput).FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(currentText))
					{
						try
						{
							character.Experience = int.Parse(currentText);
						}
						catch (Exception) { }
						parent.FindChild(cInput).FindChild("InputField").FindChild("Placeholder").GetComponent<Text>().text = character.Experience.ToString();
						parent.FindChild(cView).FindChild("Bottom").FindChild("Value").GetComponent<Text>().text = character.Experience.ToString();
						parent.FindChild(cView).FindChild("Top").FindChild("Value").GetComponent<Text>().text = character.CharacterLevel.ToString();
					}
					break;
				case cAttributesId:
					// str:
					string newAbility = parent.FindChild(cInput).FindChild("Left").FindChild("Str").FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newAbility))
					{
						character.Abilities[DnDAbilities.Strength] = int.Parse(newAbility);
					}
					// dex:
					newAbility = parent.FindChild(cInput).FindChild("Left").FindChild("Dex").FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newAbility))
					{
						character.Abilities[DnDAbilities.Dexterity] = int.Parse(newAbility);
					}
					// con:
					newAbility = parent.FindChild(cInput).FindChild("Left").FindChild("Con").FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newAbility))
					{
						character.Abilities[DnDAbilities.Constitution] = int.Parse(newAbility);
					}
					// int:
					newAbility = parent.FindChild(cInput).FindChild("Right").FindChild("Int").FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newAbility))
					{
						character.Abilities[DnDAbilities.Intelligence] = int.Parse(newAbility);
					}
					// wis:
					newAbility = parent.FindChild(cInput).FindChild("Right").FindChild("Wis").FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newAbility))
					{
						character.Abilities[DnDAbilities.Wisdom] = int.Parse(newAbility);
					}
					// cha:
					newAbility = parent.FindChild(cInput).FindChild("Right").FindChild("Cha").FindChild("InputField").GetComponent<InputField>().text.Trim();
					if (!string.IsNullOrEmpty(newAbility))
					{
						character.Abilities[DnDAbilities.Charisma] = int.Parse(newAbility);
					}
					SetAbilitiesTexts(parent);
					break;
				case cGenderId:
					var array = Enum.GetValues(typeof(CharacterGender)).Cast<CharacterGender>();
					foreach (CharacterGender gender in array)
					{
						Toggle toggle = parent.FindChild(cInput).FindChild("GenderGroup").GetChild((int)gender).GetComponent<Toggle>();
						if (toggle.isOn)
						{
							character.Gender = gender;
						}
					}
					SetGenderText(parent);
					break;
				case cRaceId:
					Slider slider = parent.FindChild(cInput).FindChild("Slider").GetComponent<Slider>();
					character.Race = (DnDRace)slider.value;
					SetRaceText(parent);
					break;
				case cAlignmentId:
					character.Alignment = AlignmentGrid.SelectedAlignment;
					break;
			}
			if (write)
			{
				AppStorage.Instance.SaveCharacters();
			}
		}

		private void SelectInput(int notification)
		{
			Transform parent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").GetChild(notification).FindChild(cInput);
			switch (notification)
			{
				case cNameId:
					parent.GetComponent<Selectable>().Select();
					break;
				case cExpId:
					parent.FindChild("InputField").GetComponent<Selectable>().Select();
					break;
			}
		}

		private IEnumerator InitView()
		{
			float onePerCent = Screen.height / 100f;
			float multiplier = 0.18f;
			if (mFirstTime)
			{
				mFirstTime = false;
			}
			else
			{
				onePerCent = 1f;
				multiplier = 1f;
			}
			mTotalHeight = 0;
			const float cWaitTime = 0.01f;
			DnDCharacter character = AppStorage.Instance.CurrentCharacter as DnDCharacter;
			// TOP:
			float heightMultiplier = Screen.height * .2f / 160;
			Transform currentParent = transform.FindChild("Top");
			RectTransform rect = currentParent.GetComponent<RectTransform>();
			rect.localScale = new Vector3(heightMultiplier, heightMultiplier, heightMultiplier);
			Image AvatarImage = currentParent.FindChild("Avatar").FindChild("AvatarMask").FindChild("Avatar").GetComponent<Image>();
			AvatarImage.sprite = ViewUtility.CreateSpriteUseTexture(character.AvatarBytes, AvatarImage);
			// MENU:
			// name:
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Name");
			Text text = currentParent.FindChild(cView).GetComponent<Text>();
			text.text = character.Name;
			text = currentParent.FindChild(cInput).FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Name;
			float current = currentParent.GetComponent<LayoutElement>().minHeight;
			currentParent.GetComponent<LayoutElement>().minHeight = current * multiplier * onePerCent;
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
			currentParent.gameObject.SetActive(true);
			yield return new WaitForSeconds(cWaitTime);
			// level / xp
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("ExpLevel");
			currentParent.FindChild(cInput).FindChild("InputField").FindChild("Placeholder").GetComponent<Text>().text = character.Experience.ToString();
			currentParent.FindChild(cView).FindChild("Bottom").FindChild("Value").GetComponent<Text>().text = character.Experience.ToString();
			currentParent.FindChild(cView).FindChild("Top").FindChild("Value").GetComponent<Text>().text = character.CharacterLevel.ToString();
			current = currentParent.GetComponent<LayoutElement>().minHeight;
			currentParent.GetComponent<LayoutElement>().minHeight = current * multiplier * onePerCent;
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
			currentParent.gameObject.SetActive(true);
			yield return new WaitForSeconds(cWaitTime);
			// abilities
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Attributes");
			SetAbilitiesTexts(currentParent);
			current = currentParent.GetComponent<LayoutElement>().minHeight;
			currentParent.GetComponent<LayoutElement>().minHeight = current * multiplier * onePerCent;
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
			currentParent.gameObject.SetActive(true);
			yield return new WaitForSeconds(cWaitTime);
			// gender:
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Gender");
			SetGenderText(currentParent);
			current = currentParent.GetComponent<LayoutElement>().minHeight;
			currentParent.GetComponent<LayoutElement>().minHeight = current * multiplier * onePerCent;
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
			currentParent.gameObject.SetActive(true);
			yield return new WaitForSeconds(cWaitTime);
			// race:
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Race");
			SetRaceText(currentParent);
			current = currentParent.GetComponent<LayoutElement>().minHeight;
			currentParent.GetComponent<LayoutElement>().minHeight = current * multiplier * onePerCent;
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
			currentParent.gameObject.SetActive(true);
			yield return new WaitForSeconds(cWaitTime);
			// alignment:
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Alignment");
			Image image = currentParent.FindChild(cView).FindChild("AlignmentImage").GetComponent<Image>();
			string path = mAlignmentPaths[(int)character.Alignment];
			ResourceRequest request = Resources.LoadAsync<Texture2D>(path);
			do
			{
				yield return new WaitForSeconds(.001f);
			}
			while (!request.isDone);
			Texture2D tex = request.asset as Texture2D;
			byte[] array = tex.EncodeToPNG();
			image.sprite = ViewUtility.CreateSpriteUseTexture(array, image);
			yield return new WaitForSeconds(.001f);
			text = currentParent.FindChild(cInput).FindChild("Top").FindChild("BoxSetText").GetComponent<Text>();
			text.text = AlignmentGrid.SelectedAlignment.ToString();
			current = currentParent.GetComponent<LayoutElement>().minHeight;
			currentParent.GetComponent<LayoutElement>().minHeight = current * multiplier * onePerCent;
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
			currentParent.gameObject.SetActive(true);
			yield return new WaitForSeconds(cWaitTime);
			// buffer:
			currentParent = transform.FindChild("Menu").FindChild("DragArea").FindChild("ScrollArea").FindChild("Buffer");
			currentParent.GetComponent<LayoutElement>().minHeight = (int)(Screen.height * 0.12f);
			mTotalHeight += (int)currentParent.GetComponent<LayoutElement>().minHeight;
			UpdateNow = true;
		}

		private void SetAbilitiesTexts(Transform parent)
		{
			DnDCharacter character = AppStorage.Instance.CurrentCharacter as DnDCharacter; 
			// str:
			Text text = parent.FindChild(cView).FindChild("Left").FindChild("Str").FindChild("Value").GetComponent<Text>();
			int ability = character.Abilities[DnDAbilities.Strength];
			text.text = ability.ToString();
			text = parent.FindChild(cView).FindChild("Left").FindChild("Str").FindChild("Mod").GetComponent<Text>();
			ability = (ability / 2 - 5);
			if (ability >= 0)
				text.text = "+" + ability.ToString();
			else
				text.text = "  " + ability.ToString();
			// dex:
			text = parent.FindChild(cView).FindChild("Left").FindChild("Dex").FindChild("Value").GetComponent<Text>();
			ability = character.Abilities[DnDAbilities.Dexterity];
			text.text = ability.ToString();
			text = parent.FindChild(cView).FindChild("Left").FindChild("Dex").FindChild("Mod").GetComponent<Text>();
			ability = (ability / 2 - 5);
			if (ability >= 0)
				text.text = "+" + ability.ToString();
			else
				text.text = "  " + ability.ToString();
			// con:
			text = parent.FindChild(cView).FindChild("Left").FindChild("Con").FindChild("Value").GetComponent<Text>();
			ability = character.Abilities[DnDAbilities.Constitution];
			text.text = ability.ToString();
			text = parent.FindChild(cView).FindChild("Left").FindChild("Con").FindChild("Mod").GetComponent<Text>();
			ability = (ability / 2 - 5);
			if (ability >= 0)
				text.text = "+" + ability.ToString();
			else
				text.text = "  " + ability.ToString();
			// int:
			text = parent.FindChild(cView).FindChild("Right").FindChild("Int").FindChild("Value").GetComponent<Text>();
			ability = character.Abilities[DnDAbilities.Intelligence];
			text.text = ability.ToString();
			text = parent.FindChild(cView).FindChild("Right").FindChild("Int").FindChild("Mod").GetComponent<Text>();
			ability = (ability / 2 - 5);
			if (ability >= 0)
				text.text = "+" + ability.ToString();
			else
				text.text = " " + ability.ToString();
			// wis:
			text = parent.FindChild(cView).FindChild("Right").FindChild("Wis").FindChild("Value").GetComponent<Text>();
			ability = character.Abilities[DnDAbilities.Wisdom];
			text.text = ability.ToString();
			text = parent.FindChild(cView).FindChild("Right").FindChild("Wis").FindChild("Mod").GetComponent<Text>();
			ability = (ability / 2 - 5);
			if (ability >= 0)
				text.text = "+" + ability.ToString();
			else
				text.text = " " + ability.ToString();
			// cha:
			text = parent.FindChild(cView).FindChild("Right").FindChild("Cha").FindChild("Value").GetComponent<Text>();
			ability = character.Abilities[DnDAbilities.Charisma];
			text.text = ability.ToString();
			text = parent.FindChild(cView).FindChild("Right").FindChild("Cha").FindChild("Mod").GetComponent<Text>();
			ability = (ability / 2 - 5);
			if (ability >= 0)
				text.text = "+" + ability.ToString();
			else
				text.text = " " + ability.ToString();
			// input placeholders:
			text = parent.FindChild(cInput).FindChild("Left").FindChild("Str").FindChild("InputField").FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Abilities[DnDAbilities.Strength].ToString();
			text = parent.FindChild(cInput).FindChild("Left").FindChild("Dex").FindChild("InputField").FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Abilities[DnDAbilities.Dexterity].ToString();
			text = parent.FindChild(cInput).FindChild("Left").FindChild("Con").FindChild("InputField").FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Abilities[DnDAbilities.Constitution].ToString();
			text = parent.FindChild(cInput).FindChild("Right").FindChild("Int").FindChild("InputField").FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Abilities[DnDAbilities.Intelligence].ToString();
			text = parent.FindChild(cInput).FindChild("Right").FindChild("Wis").FindChild("InputField").FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Abilities[DnDAbilities.Wisdom].ToString();
			text = parent.FindChild(cInput).FindChild("Right").FindChild("Cha").FindChild("InputField").FindChild("Placeholder").GetComponent<Text>();
			text.text = character.Abilities[DnDAbilities.Charisma].ToString();
		}

		private void SetGenderText(Transform parent)
		{
			DnDCharacter character = AppStorage.Instance.CurrentCharacter as DnDCharacter; 
			Text text = parent.FindChild(cView).FindChild("Text").GetComponent<Text>();
			text.text = string.Format(cGenderText, character.Gender.ToString());
			var genders = Enum.GetValues(typeof(CharacterGender)).Cast<CharacterGender>().ToArray();
			for (int i = 0; i < genders.Length; ++i)
			{
				bool chosen = character.Gender == genders[i];
				Toggle toggle = parent.FindChild(cInput).FindChild("GenderGroup").GetChild(i).GetComponent<Toggle>();
				toggle.isOn = chosen;
			}
		}

		private void SetRaceText(Transform parent)
		{
			DnDCharacter character = AppStorage.Instance.CurrentCharacter as DnDCharacter; 
			Text text = parent.FindChild(cView).FindChild("Text").GetComponent<Text>();
			text.text = string.Format(cRaceText, character.Race.ToString());
			Slider slider = parent.FindChild(cInput).FindChild("Slider").GetComponent<Slider>();
			slider.value = (int)character.Race;
			text = parent.FindChild(cInput).FindChild("SliderText").GetComponent<Text>();
			text.text = character.Race.ToString();
		}
	}
}
