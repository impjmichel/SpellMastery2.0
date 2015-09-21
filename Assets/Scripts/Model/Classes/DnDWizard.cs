using Boomlagoon.JSON;
using SpellMastery.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpellMastery.Model.DnD
{
	public class DnDWizard : DnDClassSoul
	{
		private DnDMagicSchool mSpecialization = DnDMagicSchool.NONE;
		private List<DnDMagicSchool> mForbiddenSchools = new List<DnDMagicSchool>();

		public DnDMagicSchool Specialization
		{
			get { return mSpecialization; }
			set { mSpecialization = value; }
		}

		public List<DnDMagicSchool> ForbiddenSchools
		{
			get { return mForbiddenSchools; }
		}

		public override bool CanCastExtraSpell
		{
			get
			{
				return mSpecialization != DnDMagicSchool.NONE && mSpecialization != DnDMagicSchool.Universal;
			}
		}

		public override bool NeedsToPrepareSpells
		{
			get { return true; }
		}

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

		public DnDWizard(DnDCharacter character, int level = 1) : base(character)
		{
			Init();
			mClassLevel = level;
		}
		public DnDWizard(DnDCharacter character, DnDMagicSchool specialization, List<DnDMagicSchool> forbiddenSchools, int level = 1) : base(character)
		{
			Init();
			mSpecialization = specialization;
			mForbiddenSchools = forbiddenSchools;
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

		public override int NumberofUnknownSpells(int rank)
		{
			var schools = Enum.GetValues(typeof(DnDMagicSchool)).Cast<DnDMagicSchool>();
			List<DnDMagicSchool> castable = schools.ToList();
			castable.RemoveAll(x => mForbiddenSchools.Contains(x));
			List<Spell> unknownSpells = new List<Spell>();
			foreach (DnDMagicSchool school in castable)
			{
				unknownSpells.AddRange(AppStorage.Instance.SpellList.GetWizardSchoolSpecializationSpells(school, rank));
			}
			if (mKnownSpells.Count > rank)
				unknownSpells.RemoveAll(x => mKnownSpells[rank].Find(y => y.Equals(x)) != null);
			unknownSpells.Sort();
			return unknownSpells.Count;
		 }

		private void Init()
		{
			mClass = DnDCharClass.Wizard;
			mCasterClass = DnDCastClass.Arcane;
			mSpellModifier = DnDAbilities.Intelligence;
			mSkills = CreateSkillModel();
		}

		private int SpellsPerDayPerRank(int rank)
		{
			if (!CanCastWithAbility(rank))
				return 0; 
			const int rankZero = 0;
			const int maxResult = 7;
			if (rank > rankZero)
			{
				int result = mClassLevel - ((rank - 1) * 2);
				if (result == 1)
				{
					return 1;
				}
				else if (result == 2 || result == 3)
				{
					return 2;
				}
				else if (result >= 4 && result < maxResult)
				{
					return 3;
				}
				else if (result >= maxResult)
				{
					return 4;
				}
				else // below 0 result
				{
					return 0;
				}
			}
			else
			{
				if (mClassLevel == 1)
				{
					return 3;
				}
				else
				{
					return 4;
				}
			}
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

		public override JSONObject Serialize()
		{
			JSONObject obj = new JSONObject();
			obj.Add(LEVEL, mClassLevel);
			obj.Add(KNOWN_SPELLS, SerializeKnownSpells());
			obj.Add(MAIN_SPELLS, SerializeMainSpells());
			obj.Add(EXTRA_SPELLS, SerializeExtraSpells());
			obj.Add(SKILLS, mSkills.Serialize());
			// spec:
			JSONArray tempArray = new JSONArray();
			tempArray.Add((int)mSpecialization);
			foreach (var forbidden in mForbiddenSchools)
			{
				tempArray.Add((int)forbidden);
			}
			obj.Add(SPECIALIZATION, tempArray);
			return obj;
		}

		public override void Deserialize(JSONObject obj)
		{
			mClassLevel = (int)obj.GetNumber(LEVEL);
			DeserializeKnownSpells(obj);
			DeserializeMainSpells(obj);
			DeserializeExtraSpells(obj);
			mSkills = new DnDSkillModel();
			mSkills.Deserialize(obj.GetObject(SKILLS));
			// spec:
			JSONArray tempArray = obj.GetArray(SPECIALIZATION);
			for (int i = 0; i < tempArray.Length; ++i)
			{
				if (i == 0) // first item is the specialization
				{
					mSpecialization = (DnDMagicSchool)((int)tempArray[i].Number);
				}
				else // the other items are the forbidden schools
				{
					mForbiddenSchools.Add((DnDMagicSchool)((int)tempArray[i].Number));
				}
			}
		}

		public override DnDSkillModel CreateSkillModel()
		{
			DnDSkillModel result = new DnDSkillModel();
			result.Appraise = new DnDSkillField(0, 0, true, false, DnDAbilities.Intelligence);
			result.Balance = new DnDSkillField(0, 0, true, false, DnDAbilities.Dexterity);
			result.Bluff = new DnDSkillField(0, 0, true, false, DnDAbilities.Charisma);
			result.Climb = new DnDSkillField(0, 0, true, false, DnDAbilities.Strength);
			result.Concentration = new DnDSkillField(0, 0, true, true, DnDAbilities.Constitution);
			result.DecipherScript = new DnDSkillField(0, 0, false, true, DnDAbilities.Intelligence);
			result.Diplomacy = new DnDSkillField(0, 0, true, false, DnDAbilities.Charisma);
			result.DisableDevice = new DnDSkillField(0, 0, false, false, DnDAbilities.Intelligence);
			result.Disguise = new DnDSkillField(0, 0, true, false, DnDAbilities.Charisma);
			result.EscapeArtist = new DnDSkillField(0, 0, true, false, DnDAbilities.Dexterity);
			result.Forgery = new DnDSkillField(0, 0, true, false, DnDAbilities.Intelligence);
			result.GatherInformation = new DnDSkillField(0, 0, true, false, DnDAbilities.Charisma);
			result.HandleAnimal = new DnDSkillField(0, 0, false, false, DnDAbilities.Charisma);
			result.Heal = new DnDSkillField(0, 0, true, false, DnDAbilities.Wisdom);
			result.Hide = new DnDSkillField(0, 0, true, false, DnDAbilities.Dexterity);
			result.Intimidate = new DnDSkillField(0, 0, true, false, DnDAbilities.Charisma);
			result.Jump = new DnDSkillField(0, 0, true, false, DnDAbilities.Strength);
			result.Listen = new DnDSkillField(0, 0, true, false, DnDAbilities.Wisdom);
			result.MoveSilently = new DnDSkillField(0, 0, true, false, DnDAbilities.Dexterity);
			result.OpenLock = new DnDSkillField(0, 0, false, false, DnDAbilities.Dexterity);
			result.Ride = new DnDSkillField(0, 0, true, false, DnDAbilities.Dexterity);
			result.Search = new DnDSkillField(0, 0, true, false, DnDAbilities.Intelligence);
			result.SenseMotive = new DnDSkillField(0, 0, true, false, DnDAbilities.Wisdom);
			result.SleightOfHand = new DnDSkillField(0, 0, false, false, DnDAbilities.Dexterity);
			result.SpellCraft = new DnDSkillField(0, 0, false, true, DnDAbilities.Intelligence);
			result.Spot = new DnDSkillField(0, 0, true, false, DnDAbilities.Wisdom);
			result.Survival = new DnDSkillField(0, 0, true, false, DnDAbilities.Wisdom);
			result.Swim = new DnDSkillField(0, 0, true, false, DnDAbilities.Strength);
			result.Tumble = new DnDSkillField(0, 0, false, false, DnDAbilities.Dexterity);
			result.UseMagicDevice = new DnDSkillField(0, 0, false, false, DnDAbilities.Charisma);
			result.UseRope = new DnDSkillField(0, 0, true, false, DnDAbilities.Dexterity);
			return result;
		}
	}
}
