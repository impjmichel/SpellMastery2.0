using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
public class DnDClassSoulScreen : CreationMainScreen
{
	private const int cDefaultSliderValue = 11;
	private const int cWizardScreen = 4;
	private const int cPrevScreen = 2;

	private const string cErrorMessage = "Select a positive number under or equal to {0}.";
	private const string cClassNotImplementedMessage = "Please select a different class,\nthis one is not implemented (yet).";
	private const string cAlreadyHasThisClass = "The character already has this class.";

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text ClassText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text ClassErrorText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text LevelErrorText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Slider ClassSlider;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public InputField LevelInput;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Button NextButton;

	public override void Update()
	{
		base.Update();
		if (ClassText != null && ClassSlider != null)
		{
			ClassText.text = ((DnDCharClass)((int)ClassSlider.value)).ToString();
			if (ClassErrorText != null)
			{
				if (HasClass())
				{
					ClassErrorText.text = cAlreadyHasThisClass;
				}
				else
				{
					ClassErrorText.text = cClassNotImplementedMessage;
				}
			}
		}
		if (NextButton != null && LevelErrorText != null)
		{
			bool correctLevel = CorrectLevelInput();
			bool correctClass = CorrectClassSelected() && !HasClass();
			if (correctLevel)
			{
				LevelErrorText.gameObject.SetActive(false);
			}
			else
			{
				int number = ((DnDCharacter)Storage.Character).LevelsToSpend;
				LevelErrorText.text = string.Format(cErrorMessage, number);
				LevelErrorText.gameObject.SetActive(true);
			}
			ClassErrorText.gameObject.SetActive(!correctClass);
			if (correctClass && correctLevel)
			{
				NextButton.interactable = true;
				NextButton.GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				NextButton.interactable = false;
				NextButton.GetComponent<CanvasGroup>().alpha = .4f;
			}
		}
	}

	public override void Next()
	{
		int level = 1;
		if (!string.IsNullOrEmpty(LevelInput.text))
		{
			level = int.Parse(LevelInput.text);
		}
		DnDCharClass chosen = (DnDCharClass)((int)ClassSlider.value);
		DnDClassSoul soul = null;
		int nextScreen = -1;
		switch (chosen)
		{
			case DnDCharClass.Wizard:
				soul = new DnDWizard((DnDCharacter)Storage.Character, level);
				nextScreen = cWizardScreen;
				break;
			default:
				break;
		}
		if (soul != null)
		{
			((DnDCharacter)Storage.Character).Classes.Add(soul);
		}
		if (nextScreen >= 0)
		{
			Storage.Screens[nextScreen].SetActive(true);
			this.gameObject.SetActive(false);
		}
		else
		{
			SwitchScene(ScreenBase.SCENE_SPELLMASTERY);
		}
	}

	public override void Back()
	{
		Reset();
		Storage.Screens[cPrevScreen].SetActive(true);
		this.gameObject.SetActive(false);
	}

	private void Reset()
	{
		ClassSlider.value = cDefaultSliderValue;
		LevelInput.text = "";
		LevelErrorText.text = "";
	}

	private bool CorrectLevelInput()
	{
		if (!string.IsNullOrEmpty(LevelInput.text))
		{
			int input = 0;
			try
			{
				input = int.Parse(LevelInput.text);
			}
			catch (Exception) {}
			
			return input > 0 && input <= ((DnDCharacter)Storage.Character).LevelsToSpend;
		}
		return true;
	}

	private bool CorrectClassSelected()
	{
		List<int> implemented = new List<int>() { 11 };
		return implemented.Contains((int)ClassSlider.value);
	}

	private bool HasClass()
	{
		DnDCharClass selected = (DnDCharClass)((int)ClassSlider.value);
		return ((DnDCharacter)Storage.Character).HasClass(selected);
	}
}
}
