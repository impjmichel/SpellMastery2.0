using System;
using UnityEngine;

namespace SpellMastery.View
{
public abstract class ScreenBase : MonoBehaviour, IMainScreen
{
	protected const string SCENE_SPELLMASTERY = "SpellMastery";
	protected const string SCENE_CREATION = "CharacterCreation";
	protected const string SCENE_PROFILE = "CharacterProfile";

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public GameObject LoadingScreen;

	protected void SwitchScene(string sceneName)
	{
		if (LoadingScreen != null)
		{
			LoadingScreen.SetActive(true);
		}
		Application.LoadLevelAsync(sceneName);
	}

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Back();
		}
	}

	public abstract void Next();

	public abstract void Back();
}
}
