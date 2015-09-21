using SpellMastery.Control;
using SpellMastery.Model.DnD;
using System;
using System.Collections.Generic;

namespace SpellMastery.View
{
	public class PrepareSpellScreen : SpellScreen
	{
		private const int cRegularSpell = 0;
		private const int cExtraSpell = 1;

		public override void Next()
		{
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (character.Classes.Count < mSelectedClass)
				return;
			DnDClassSoul soul = character.Classes[mSelectedClass];
			if (soul.KnownSpells.Count < mSelectedRank)
				return;
			List<Spell> rank = soul.KnownSpells[mSelectedRank];
			if (rank.Count < mSelectedSpell)
				return;
			Spell spell = rank[mSelectedSpell];
			if (mSelectedSpellSender == cRegularSpell)
			{
				soul.PrepareMainSpell(spell, mSelectedRank);
			}
			else if (mSelectedSpellSender == cExtraSpell)
			{
				soul.PrepareExtraSpell(spell, mSelectedRank);
			}
		}

		protected override void InitSpellSelection()
		{
			DnDCharacter character = (DnDCharacter)AppStorage.Instance.CurrentCharacter;
			if (character.Classes.Count < mSelectedClass)
				return;
			DnDClassSoul soul = character.Classes[mSelectedClass];
			if (soul.KnownSpells.Count < mSelectedRank)
				return;
			if (soul.CanCastExtraSpell)
			{ // double buttons

			}
			else
			{ // single buttons

			}
			throw new NotImplementedException();
		}
	}
}
