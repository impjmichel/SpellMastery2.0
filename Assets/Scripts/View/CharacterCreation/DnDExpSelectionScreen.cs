using SpellMastery.Model.DnD;
using SpellMastery.View.Util;

namespace SpellMastery.View
{
public class DnDExpSelectionScreen : CreationMainScreen
{
	private const int cPrevScreen = 0;
	private const int cNextScreen = 2;

	public override void Next()
	{
		SetValues();
		Storage.Screens[cNextScreen].SetActive(true);
		this.gameObject.SetActive(false);
	}

	public override void Back()
	{
		Storage.Screens[cPrevScreen].SetActive(true);
		this.gameObject.SetActive(false);
	}

	private void SetValues()
	{
		if (!string.IsNullOrEmpty(Storage.ExperienceInput.text))
			((DnDCharacter)Storage.Character).Experience = int.Parse(Storage.ExperienceInput.text);
		else
			((DnDCharacter)Storage.Character).Experience = 0;
		if (!string.IsNullOrEmpty(Storage.AbilityInputs[(int)DnDAbilities.Strength].text))
			((DnDCharacter)Storage.Character).Abilities[DnDAbilities.Strength] = int.Parse(Storage.AbilityInputs[(int)DnDAbilities.Strength].text);
		if (!string.IsNullOrEmpty(Storage.AbilityInputs[(int)DnDAbilities.Dexterity].text))
			((DnDCharacter)Storage.Character).Abilities[DnDAbilities.Dexterity] = int.Parse(Storage.AbilityInputs[(int)DnDAbilities.Dexterity].text);
		if (!string.IsNullOrEmpty(Storage.AbilityInputs[(int)DnDAbilities.Constitution].text))
			((DnDCharacter)Storage.Character).Abilities[DnDAbilities.Constitution] = int.Parse(Storage.AbilityInputs[(int)DnDAbilities.Constitution].text);
		if (!string.IsNullOrEmpty(Storage.AbilityInputs[(int)DnDAbilities.Intelligence].text))
			((DnDCharacter)Storage.Character).Abilities[DnDAbilities.Intelligence] = int.Parse(Storage.AbilityInputs[(int)DnDAbilities.Intelligence].text);
		if (!string.IsNullOrEmpty(Storage.AbilityInputs[(int)DnDAbilities.Wisdom].text))
			((DnDCharacter)Storage.Character).Abilities[DnDAbilities.Wisdom] = int.Parse(Storage.AbilityInputs[(int)DnDAbilities.Wisdom].text);
		if (!string.IsNullOrEmpty(Storage.AbilityInputs[(int)DnDAbilities.Charisma].text))
			((DnDCharacter)Storage.Character).Abilities[DnDAbilities.Charisma] = int.Parse(Storage.AbilityInputs[(int)DnDAbilities.Charisma].text);
	}
}
}
