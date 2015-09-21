using SpellMastery.Model;
using SpellMastery.Model.DnD;
using SpellMastery.View.Util;
using System;
using UnityEngine.UI;

namespace SpellMastery.View
{
public class GameSelectionScreen : CreationMainScreen
{
	private const int DnD_NextScreen = 1;
	private Text mSliderText;

	public override void Next()
	{
		int selected = (int)Storage.GameSelector.value;
		switch(selected)
		{
			case 0:
				Storage.Screens[DnD_NextScreen].SetActive(true);
				break;
			default:
				break;
		}
		CreatePlayerCharacter();
		this.gameObject.SetActive(false);
	}

	public override void Back()
	{
		Storage.Reset();
		SwitchScene(SCENE_SPELLMASTERY);
	}

	public override void Update()
	{
		base.Update();
		if (mSliderText == null)
		{
			try
			{
				mSliderText = transform.FindChild("Menu").FindChild("GameSelector").FindChild("HandleSlideArea").FindChild("Text").GetComponent<Text>();
			}
			catch (Exception) { }
		}
		else
		{
			mSliderText.text = ViewUtility.ToViewString((CharacterGame)((int)Storage.GameSelector.value + 1));
		}
	}

	private void CreatePlayerCharacter()
	{
		Storage.Game = (CharacterGame)((int)Storage.GameSelector.value + 1); // 'NONE' can't be selected
		switch (Storage.Game)
		{
			case CharacterGame.DnD_3_5:
				Storage.Character = new DnDCharacter(Storage.NameInput.text);
				break;
			default:
				break;
		}
	}
}
}
