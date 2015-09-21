using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
public class DnDWizardScreen : CreationMainScreen
{
	private const string cNoSpecText = "No specialization";
	private const string cFirstForbiddenText = "Forbidden: {0}";
	private const string cSecondForbiddenText = "Forbidden: {0}";
	private const int cLowerConstraint = 1;
	private const int cUpperConstraint = 8;
	private const int cForbiddenSchool = 3;
	private const int cNextScreen = 5;
	private const int cPrevScreen = 3;
	private const int cDefaultSpec = 0;
	private const int cDefaultOne = 1;
	private const int cDefaultTwo = 2;

	private int mCurrentUpper = cUpperConstraint;
	private int mCurrentLower = cLowerConstraint;
	private int previousFOne = 1;
	private int currentFOne = 1;
	private int previousFTwo = 2;
	private int currentFTwo = 2;

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text SpecText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text ForbiddenOneText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text ForbiddenTwoText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Slider SpecSlider;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Slider ForbiddenOneSlider;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Slider ForbiddenTwoSlider;

	public override void Next()
	{
		Storage.Screens[cNextScreen].SetActive(true);
		SetValues();
		this.gameObject.SetActive(false);
	}

	public override void Back()
	{
		Storage.Screens[cPrevScreen].SetActive(true);
		Reset();
		this.gameObject.SetActive(false);
	}

	public void OnSliderSpecChange()
	{
		int current = (int)SpecSlider.value;
		if (currentFOne == current)
		{
			ForbiddenOneSlider.value = MoveNumber(currentFOne, previousFOne);
		}
		if (currentFTwo == current)
		{
			ForbiddenTwoSlider.value = MoveNumber(currentFTwo, previousFTwo);
		}
	}

	public void OnSliderOneChange()
	{
		previousFOne = currentFOne;
		currentFOne = (int)ForbiddenOneSlider.value;
		int spec = (int)SpecSlider.value;
		if (currentFOne == cForbiddenSchool || currentFOne == currentFTwo || currentFOne == spec)
		{
			ForbiddenOneSlider.value = MoveNumber(currentFOne, previousFOne);
		}
	}
	public void OnSliderTwoChange()
	{
		previousFTwo = currentFTwo;
		currentFTwo = (int)ForbiddenTwoSlider.value;
		int spec = (int)SpecSlider.value;
		if (currentFTwo == cForbiddenSchool || currentFTwo == currentFOne || currentFTwo == spec)
		{
			ForbiddenTwoSlider.value = MoveNumber(currentFTwo, previousFTwo);
		}
	}

	public override void Update()
	{
		base.Update();
		if (SpecSlider != null && SpecText != null)
		{
			if (SpecSlider.value > 0f)
			{
				SpecText.text = ((DnDMagicSchool)((int)SpecSlider.value)).ToString();
				ForbiddenOneText.transform.parent.gameObject.SetActive(true);
				ForbiddenTwoText.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				SpecText.text = cNoSpecText;
				ForbiddenOneText.transform.parent.gameObject.SetActive(false);
				ForbiddenTwoText.transform.parent.gameObject.SetActive(false);
			}
		}
		if (ForbiddenOneSlider != null && ForbiddenOneText != null)
		{
			ForbiddenOneText.text = string.Format(cFirstForbiddenText,((DnDMagicSchool)((int)ForbiddenOneSlider.value)).ToString());
		}
		if (ForbiddenTwoSlider != null && ForbiddenTwoText != null)
		{
			ForbiddenTwoText.text = string.Format(cSecondForbiddenText,((DnDMagicSchool)((int)ForbiddenTwoSlider.value)).ToString());
		}

		if (ForbiddenOneSlider != null && ForbiddenTwoSlider != null && SpecSlider != null)
		{
			if (ForbiddenOneSlider.value <= cLowerConstraint || ForbiddenTwoSlider.value <= cLowerConstraint || SpecSlider.value == cLowerConstraint)
			{
				mCurrentLower = cLowerConstraint + 1;
			}
			else
			{
				mCurrentLower = cLowerConstraint;
			}
			if (ForbiddenOneSlider.value >= cUpperConstraint || ForbiddenTwoSlider.value >= cUpperConstraint || SpecSlider.value >= cUpperConstraint)
			{
				mCurrentUpper = cUpperConstraint - 1;
			}
			else
			{
				mCurrentUpper = cUpperConstraint;
			}
		}
	}

	private int MoveNumber(int current, int previous)
	{
		if (current >= previous) // ++
		{
			if (current >= mCurrentUpper)
			{
				return current - 1;
			}
			return current + 1;
		}
		else // --
		{
			if (current <= mCurrentLower)
			{
				return current + 1;
			}
			return current - 1;
		}
	}

	private void Reset()
	{
		SpecSlider.value = cDefaultSpec;
		ForbiddenOneSlider.value = cDefaultOne;
		ForbiddenTwoSlider.value = cDefaultTwo;
		previousFOne = cDefaultOne;
		currentFOne = cDefaultOne;
		previousFTwo = cDefaultTwo;
		currentFTwo = cDefaultTwo;
		int lastIndex = ((DnDCharacter)Storage.Character).Classes.Count - 1;
		if (lastIndex >= 0)
		{
			((DnDCharacter)Storage.Character).Classes.RemoveAt(lastIndex);
		}
	}

	private void SetValues()
	{
		DnDWizard wizard = (DnDWizard)((DnDCharacter)Storage.Character).Classes.Find(x => x.CharacterClass == DnDCharClass.Wizard);
		if (wizard != null)
		{
			wizard.Specialization = (DnDMagicSchool)((int)SpecSlider.value);
			wizard.ForbiddenSchools.Clear();
			if (wizard.Specialization != DnDMagicSchool.NONE)
			{
				wizard.ForbiddenSchools.Add((DnDMagicSchool)((int)ForbiddenOneSlider.value));
				wizard.ForbiddenSchools.Add((DnDMagicSchool)((int)ForbiddenTwoSlider.value));
			}
		}
	}
}
}
