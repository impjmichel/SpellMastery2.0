using UnityEngine;
using SpellMastery.View.Util;
using System;
using System.Linq;
using UnityEngine.UI;
using SpellMastery.Model.DnD;
using SpellMastery.Model;

namespace SpellMastery.View
{
	public class DnDExtraScreen : ScrollViewScreen
	{
		private const int cLastScreen = 8;
		private const int cPanelHeight = 430;
		private bool mSet = false;

		private Text AlignmentText;

		/// <summary>
		/// Attach in Unity
		/// </summary>
		public AlignmentGrid AlignmentGrid;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public ToggleGroup GenderGroup;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public GameObject RaceObject;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public CreationStorage Storage;

		public override void Next()
		{
			SetValues();
			Storage.Screens[cLastScreen].SetActive(true);
			gameObject.SetActive(false);
		}

		public override void Back()
		{
			Next();
		}

		public override void Update()
		{
			base.Update();
			if (mSet)
			{
				mTotalHeight = cPanelHeight;
			}
			// update texts:
			if (AlignmentGrid != null)
			{
				if (AlignmentText == null)
				{
					AlignmentText = AlignmentGrid.transform.FindChild("BoxSetText").GetComponent<Text>();
				}
				else
				{
					AlignmentText.text = ViewUtility.ToViewString(AlignmentGrid.SelectedAlignment);
				}
			}
			if (RaceObject != null)
			{
				Text sliderText = RaceObject.transform.FindChild("SliderText").GetComponent<Text>();
				int value = (int)RaceObject.GetComponentInChildren<Slider>().value;
				sliderText.text = ViewUtility.ToViewString((DnDRace)value);
			}
			mSet = true;
		}

		private void SetValues()
		{
			DnDCharacter character = (DnDCharacter)Storage.Character;
			var genders = Enum.GetValues(typeof(CharacterGender)).Cast<CharacterGender>().ToArray();
			for (int i = 0; i < genders.Length; ++i)
			{
				Toggle toggle = GenderGroup.transform.GetChild(i).GetComponent<Toggle>();
				if (toggle.isOn)
				{
					character.Gender = genders[i];
				}
			}
			character.Race = (DnDRace)(int)RaceObject.GetComponentInChildren<Slider>().value;
			character.Alignment = AlignmentGrid.SelectedAlignment;
		}
	}
}
