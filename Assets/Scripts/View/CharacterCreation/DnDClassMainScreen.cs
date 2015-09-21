using SpellMastery.Control;
using SpellMastery.Model;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View
{
public class DnDClassMainScreen : CreationMainScreen
{
	private const int cExtraDataScreen = 9;
	private const int cNextScreen = 3;
	private const int cPrevScreen = 1;
	private const string cLevelText = "{0} level is {1}.\n{2}";
	private const string cNoSoulsSingleLevelText = "You can select a class to spend the point.";
	private const string cNoSoulsMultiLevelText = "You can spend these points on a single class or divide them over multiple classes.";
	private const string cSoulsText = "You have {0} remaining points to spend.";
	private const string cPopupBody = "{0} is ready\nbut you can add more data for a more personalized character sheet.";

	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text TopText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Text BottomText;
	/// <summary>
	/// Attach in Unity
	/// </summary>
	public Button AddClassButton;

	public override void Update()
	{
		base.Update();
		if (Storage.Game == CharacterGame.DnD_3_5)
		{
			int toSpend = ((DnDCharacter)Storage.Character).LevelsToSpend;
			if (toSpend == 0)
			{
				AddClassButton.interactable = false;
				AddClassButton.GetComponent<CanvasGroup>().alpha = .3f;
			}
			else
			{
				AddClassButton.interactable = true;
				AddClassButton.GetComponent<CanvasGroup>().alpha = 1f;
			}
			int souls = ((DnDCharacter)Storage.Character).Classes.Count;
			if (TopText != null)
			{
				string name = ((DnDCharacter)Storage.Character).Name + "'";
				if (!name.EndsWith("s'"))
				{
					name += "s";
				}
				int level = ((DnDCharacter)Storage.Character).CharacterLevel;
				string secondLine = cNoSoulsSingleLevelText;
				if (level > 1)
				{
					secondLine = cNoSoulsMultiLevelText;
				}
				if (souls > 0)
				{
					secondLine = string.Format(cSoulsText, toSpend);
				}
				TopText.text = string.Format(cLevelText, name, level, secondLine);
			}
			if (BottomText != null)
			{
				if (souls > 0)
				{
					string text = Storage.Character.Name;
					foreach (DnDClassSoul soul in ((DnDCharacter)Storage.Character).Classes)
					{
						text += "\n" + soul.ShortInfo();
					}
					BottomText.text = text;
				}
				else
				{
					BottomText.text = "";
				}
			}
		}
	}

	public void OnClick_AddClass()
	{
		Storage.Screens[cNextScreen].SetActive(true);
		this.gameObject.SetActive(false);
	}

	public override void Next()
	{
		ShowPopup();
	}

	public override void Back()
	{
		Storage.Screens[cPrevScreen].SetActive(true);
		Reset();
		this.gameObject.SetActive(false);
	}

	private void SaveCharacter()
	{
		AppStorage.Instance.SaveNewCharacter(Storage.Character);
	}

	private void Reset()
	{
		((DnDCharacter)Storage.Character).Classes.Clear();
	}

	private void ShowPopup()
	{
		GameObject popup = transform.FindChild("MessagePanel").gameObject;
		string body = string.Format(cPopupBody, Storage.Character.Name);
		popup.transform.FindChild("Background").FindChild("Body").FindChild("BodyText").GetComponent<Text>().text = body;
		popup.SetActive(true);
	}

	public void OnClick_PopupLeft()
	{
		Storage.Screens[cExtraDataScreen].SetActive(true);
		this.gameObject.SetActive(false);
	}

	public void OnClick_PopupRight()
	{
		SaveCharacter();
		Storage.Reset();
		SwitchScene(SCENE_SPELLMASTERY);
	}
}
}
