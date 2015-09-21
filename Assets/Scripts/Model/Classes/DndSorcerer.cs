using Boomlagoon.JSON;
using System;

namespace SpellMastery.Model.DnD
{
	public class DnDSorcerer : DnDClassSoul
	{
		public override int HighestCastableRank
		{
			get
			{
				int abilityMax = int.MaxValue;
				if (mCharacter != null)
				{
					abilityMax = mCharacter.Abilities[mSpellModifier] - 10;
				}
				int levelMax = (int)Math.Ceiling(mClassLevel / 2f);
				if (levelMax > SpellAttribute.MAX_RANK)
					levelMax = SpellAttribute.MAX_RANK;

				if (levelMax < abilityMax)
					return levelMax;
				else
					return abilityMax;
			}
		}

		public DnDSorcerer(DnDCharacter character, int level = 1) : base(character)
		{
			Init();
			mClassLevel = level;
		}

		public override int[] SpellsPerDay()
		{
			return SpellsPerDay(GetModifier(mSpellModifier));
		}

		public override int[] SpellsPerDay(int abilityModifier)
		{
			int[] ranks = new int[SpellAttribute.MAX_RANK + 1] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			for (int rank = 0; rank < ranks.Length; ++rank)
			{
				ranks[rank] = SpellsPerDayPerRank(rank) + SpellsPerDayPerModifier(abilityModifier, rank);
			}
			return ranks;
		}

		public override void Deserialize(JSONObject obj)
		{
			throw new NotImplementedException();
		}

		public override JSONObject Serialize()
		{
			throw new NotImplementedException();
		}

		private void Init()
		{
			mClass = DnDCharClass.Sorcerer;
			mCasterClass = DnDCastClass.Arcane;
			mSpellModifier = DnDAbilities.Charisma;
		}

		/// <summary>
		/// just trust this.
		/// </summary>
		private int SpellsPerDayPerRank(int rank)
		{
			if (!CanCastWithAbility(rank))
				return 0; 
			int maxRank = (int)Math.Ceiling(mClassLevel / 2f);
			if (rank < maxRank)
				return 0;
		
			if (rank == 0)
			{
				if (mClassLevel == 1)
				{
					return 5;
				}
				else
				{
					return 6;
				}
			}
			else if (rank == 1)
			{
				int result = mClassLevel + 2;
				if (result > 6)
				{
					return result;
				}
			}
			else if (rank == 9)
			{
				if (mClassLevel <= 17)
				{
					return -1;
				}
				else if (mClassLevel == 18)
				{
					return 3;
				}
				else if (mClassLevel == 19)
				{
					return 4;
				}
				else
				{
					return 6;
				}
			}
			else
			{
				if (mClassLevel >= rank * 2)
				{
					int result = (rank * 2) - mClassLevel + 3;
					if (result > 6)
					{
						result = 6;
					}
					return result;
				}
				else
				{
					return -1;
				}
			}
			return -1;
		}

		protected override bool CanCastWithAbility(int rank)
		{
			if (mCharacter != null)
			{
				int ability = mCharacter.Abilities[mSpellModifier] - 10;
				return ability >= rank;
			}
			return true;
		}

		public override DnDSkillModel CreateSkillModel()
		{
			throw new NotImplementedException();
		}
	}
}
