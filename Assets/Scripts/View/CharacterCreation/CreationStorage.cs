using UnityEngine;
using UnityEngine.UI;
using SpellMastery.Model;
using SpellMastery.Model.DnD;
using System.Collections;

namespace SpellMastery.View
{
public class CreationStorage : MonoBehaviour
{
	private CharacterGame mGame;
	private PlayerCharacter mCharacter;

	public CharacterGame Game
	{
		get { return mGame; }
		set { mGame = value; }
	}

	public PlayerCharacter Character
	{
		get { return mCharacter; }
		set { mCharacter = value; }
	}

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public GameObject[] Screens;

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Slider GameSelector;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public InputField NameInput;

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public InputField ExperienceInput;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public InputField[] AbilityInputs;

	public void Reset()
	{
		GameSelector.value = 0;
		NameInput.text = "";
		ExperienceInput.text = "0";
		foreach (InputField text in AbilityInputs)
		{
			text.text = "0";
		}
	}
}
}
