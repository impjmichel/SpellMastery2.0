using Boomlagoon.JSON;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellMastery.Model.DnD
{
	public enum DnDCharClass
	{
		NONE		=  0,
		Barbarian	=  1, 		// does not prepare spells
		Bard		=  2, 		// does not prepare spells
		Cleric		=  3, 	// WIS modifier
		Druid		=  4, 	// WIS modifier
		Fighter		=  5,		// does not prepare spells
		Monk		=  6,		// does not prepare spells
		Paladin		=  7,	// WIS modifier
		Ranger		=  8,	// WIS modifier
		Rogue		=  9,		// does not prepare spells
		Sorcerer	= 10,		// does not prepare spells // use as Spells known
		Wizard		= 11	// INT modifier
	}

	public enum DnDCastClass
	{
		NONE		= 0,
		Arcane		= 1,
		Divine		= 2
	}

	public enum DnDMagicSchool
	{
		NONE			= 0,
		Abjuration		= 1,
		Conjuration		= 2,
		Divination		= 3,
		Enchantment		= 4,
		Evocation		= 5,
		Illusion		= 6,
		Necromancy		= 7,
		Transmutation	= 8,
		Universal		= 9
	}

	public enum DnDClericDomain
	{
		NONE			=  0,
		Air				=  1,
		Animal			=  2,
		Chaos			=  3,
		Death			=  4,
		Destruction		=  5,
		Earth			=  6,
		Evil			=  7,
		Fire			=  8,
		Good			=  9,
		Healing			= 10,
		Knowledge		= 11,
		Law				= 12,
		Luck			= 13,
		Magic			= 14,
		Plant			= 15,
		Protection		= 16,
		Strength		= 17,
		Sun				= 18,
		Travel			= 19,
		Trickery		= 20,
		War				= 21,
		Water			= 22
	}

	public enum DnDCharacterSize
	{
		NONE		= 0,
		Fine		= 1,
		Diminutive	= 2,
		Tiny		= 3,
		Small		= 4,
		Medium		= 5,
		Large		= 6,
		Huge		= 7,
		Gargantuan	= 8,
		Colossal	= 9
	}

	public enum DnDRace
	{
		Undefined	=  0,
		Orc			=  1,
		HalfOrc		=  2,
		Elf			=  3,
		HalfElf		=  4,
		Human		=  5,
		Dwarf		=  6,
		Gnome		=  7,
		Halfling	=  8,
		Goblin		=  9,
		Elemental	= 10,
		Beast		= 11
	}

	public enum DnDAlignment
	{
		LawfulGood		= 0,
		NeutralGood		= 1,
		ChaoticGood		= 2,
		LawfulNeutral	= 3,
		TrueNeutral		= 4,
		ChaoticNeutral	= 5,
		LawfulEvil		= 6,
		NeutralEvil		= 7,
		ChaoticEvil		= 8
	}

	public enum DnDAbilities
	{
		Strength		= 0,
		Dexterity		= 1,
		Constitution	= 2,
		Intelligence	= 3,
		Wisdom			= 4,
		Charisma		= 5
	}

	/// <summary>
	/// This is the basic Dungeon and Dragons character information.
	/// </summary>
	public class DnDCharacter : PlayerCharacter
	{
		private const int cExpToLevelModifier = 1000;

		private List<DnDClassSoul> mClasses = new List<DnDClassSoul>();
		private Dictionary<DnDAbilities, int> mAbilities = new Dictionary<DnDAbilities, int>
		{
			{DnDAbilities.Strength, 0},
			{DnDAbilities.Dexterity, 0},
			{DnDAbilities.Constitution, 0},
			{DnDAbilities.Intelligence, 0},
			{DnDAbilities.Wisdom, 0},
			{DnDAbilities.Charisma, 0}
		};
		private DnDAlignment mAlignment = DnDAlignment.TrueNeutral;
		private DnDRace mRace = DnDRace.Undefined;
		private int mAge;
		private DnDDeity mDeity;
		private DnDCharacterSize mSize = DnDCharacterSize.Medium;

		/* the following attributes are not used or serialized (yet):

		private int mHitPoints;
		private int mSpeed;

		private int mInitiativeMiscModifier;

			// ac, touch, flat-footed are based on:
		private int mArmorBonus;
		private int mShieldBonus;
		private int mNaturalArmor;
		private int mDeflectionModifier;
		private int mArmorMiscModifier;

			// saves:
		private int mFortitudeMagicModifier;
		private int mFortitudeMiscModifier;
		private int mReflexMagicModifier;
		private int mReflexMiscModifier;
		private int mWillMagicModifier;
		private int mWillMiscModifier;

			// attack:
		private int mBaseAttack;
		private int mSpellResistance;
		private int mGrappleMiscModifier; // Grapple = mBaseAttack + strength-modifier + size-modifier + mGrappleMiscModifier
		*/

		public List<DnDClassSoul> Classes
		{
			get { return mClasses; }
		}

		public Dictionary<DnDAbilities, int> Abilities
		{
			get { return mAbilities; }
		}

		public DnDAlignment Alignment
		{
			get { return mAlignment; }
			set { mAlignment = value; }
		}

		public DnDRace Race
		{
			get { return mRace; }
			set { mRace = value; }
		}

		public int Age
		{
			get { return mAge; }
			set { mAge = value; }
		}

		public DnDDeity Deity
		{
			get { return mDeity; }
			set { mDeity = value; }
		}

		public DnDCharacterSize Size
		{
			get { return mSize; }
			set { mSize = value; }
		}

		public int CharacterLevel
		{
			get
			{
				int level = 0;
				int tempXP = mExperience;
				while (tempXP >= 0)
				{
					level++;
					tempXP -= level * cExpToLevelModifier;
				}
				return level;
			}
		}

		public bool IsMultiClass
		{
			get
			{
				return mClasses.Count > 1;
			}
		}

		public override bool NeedsToPrepareSpells
		{
			get
			{
				foreach (var soul in mClasses)
				{
					if (soul.NeedsToPrepareSpells)
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// returns TRUE if the combined class-soul-levels are less than the ClassLevel.
		/// </summary>
		public bool CanLevelUp
		{
			get
			{
				return LevelsToSpend > 0;
			}
		}

		public int LevelsToSpend
		{
			get
			{
				int total = 0;
				foreach (DnDClassSoul soul in mClasses)
				{
					total += soul.ClassLevel;
				}
				return CharacterLevel - total;
			}
		}

		public DnDCharacter(string name = "") : base(name)
		{
			mGame = CharacterGame.DnD_3_5;
		}

		public bool HasClass(DnDCharClass charClass)
		{
			foreach (var soul in mClasses)
			{
				if (soul.CharacterClass == charClass)
				{
					return true;
				}
			}
			return false;
		}

		public override JSONObject Serialize()
		{
			JSONObject obj = new JSONObject();
			obj.Add(NAME, mName);
			obj.Add(GENDER, (int)mGender);
			obj.Add(EXPERIENCE, mExperience);
			obj.Add(AVATAR, mAvatar);
			obj.Add(ALIGNMENT, (int)mAlignment);
			obj.Add(RACE, (int)mRace);
			obj.Add(AGE, mAge);
			if (mDeity != null)
			{
				obj.Add(DEITY, mDeity.Serialize());
			}
			obj.Add(SIZE, (int)mSize);
			// souls:
			JSONObject jSouls = new JSONObject();
			foreach (DnDClassSoul soul in mClasses)
			{
				jSouls.Add(""+soul.CharacterClass, soul.Serialize());
			}
			obj.Add(CLASS_SOULS, jSouls);
			// abilities:
			JSONArray tempArray = new JSONArray();
			foreach (var pair in mAbilities)
			{
				tempArray.Add(new JSONArray()
					{
						(int)pair.Key,
						pair.Value
					});
			}
			obj.Add(ABILITIES, tempArray);
			return obj;
		}

		public override void Deserialize(JSONObject obj)
		{
			mName = obj.GetString(NAME);
			mGender = (CharacterGender)(int)obj.GetNumber(GENDER);
			mExperience = (int)obj.GetNumber(EXPERIENCE);
			mAvatar = obj.GetString(AVATAR);
			mAlignment = (DnDAlignment)(int)obj.GetNumber(ALIGNMENT);
			mRace = (DnDRace)(int)obj.GetNumber(RACE);
			mAge = (int)obj.GetNumber(AGE);
			if (obj.ContainsKey(DEITY))
			{
				mDeity = new DnDDeity();
				mDeity.Deserialize(obj.GetObject(DEITY));
			}
			mSize = (DnDCharacterSize)(int)obj.GetNumber(SIZE);
			// souls:
			JSONObject jSouls = obj.GetObject(CLASS_SOULS);
			var classes = Enum.GetValues(typeof(DnDCharClass)).Cast<DnDCharClass>();
			foreach (DnDCharClass charClass in classes)
			{
				if (jSouls.ContainsKey(""+charClass))
				{
					if (!string.IsNullOrEmpty(jSouls.GetString(""+charClass)))
					{
						DnDClassSoul newSoul = null;
						switch (charClass)
						{
							case DnDCharClass.Wizard:
								newSoul = new DnDWizard(this);
								break;
							default:
								break;
						}
						if (newSoul != null)
						{
							newSoul.Deserialize(jSouls.GetObject("" + charClass));
							mClasses.Add(newSoul);
						}
					}
				}
			}
			// abilities:
			JSONArray tempArray = obj.GetArray(ABILITIES);
			foreach (var val in tempArray)
			{
				mAbilities[(DnDAbilities)((int)val.Array[0].Number)] = (int)val.Array[1].Number;
			}
		}
	}
}
