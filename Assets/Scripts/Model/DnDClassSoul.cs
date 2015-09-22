using Boomlagoon.JSON;
using System;
using System.Collections.Generic;

namespace SpellMastery.Model.DnD
{
	public abstract class DnDClassSoul : SerializableObject
	{
		protected DnDCharacter mCharacter;
		protected DnDCharClass mClass;
		protected DnDCastClass mCasterClass;
		protected DnDAbilities mSpellModifier;
		protected DnDSkillModel mSkills;
		protected int mClassLevel;
		protected List<List<Spell>> mKnownSpells = new List<List<Spell>>();
		/// <summary>
		/// boolean provided is whether or not the spell is used.
		/// </summary>
		protected List<List<KeyValuePair<Spell, bool>>> mMainSpells = new List<List<KeyValuePair<Spell, bool>>>();
		/// <summary>
		/// boolean provided is whether or not the spell is used.
		/// </summary>
		protected List<KeyValuePair<Spell, bool>> mExtraSpells = new List<KeyValuePair<Spell, bool>>();

		public DnDCharacter Character
		{
			set { mCharacter = value; }
		}

		public DnDCharClass CharacterClass
		{
			get { return mClass; }
		}

		public DnDCastClass CasterClass
		{
			get { return mCasterClass; }
		}

		public DnDAbilities SpellModifier
		{
			get { return mSpellModifier; }
		}

		public DnDSkillModel Skills
		{
			get { return mSkills; }
		}

		public int ClassLevel
		{
			get { return mClassLevel; }
			set { mClassLevel = value; }
		}

		public List<List<Spell>> KnownSpells
		{
			get { return mKnownSpells; }
		}

		public List<List<KeyValuePair<Spell, bool>>> MainSpells
		{
			get { return mMainSpells; }
		}

		public List<KeyValuePair<Spell, bool>> ExtraSpells
		{
			get { return mExtraSpells; }
		}

		/// <summary>
		/// override if need be!
		/// </summary>
		public virtual bool CanCastExtraSpell
		{
			get { return false; }
		}

		public virtual bool NeedsToPrepareSpells
		{
			get { return false; }
		}

		/// <summary>
		/// override if need be!
		/// </summary>
		public virtual int HighestCastableRank
		{
			get { return -1; }
		}

		public DnDClassSoul(DnDCharacter character)
		{
			mCharacter = character;
		}

		public float MaxSkillRank(DnDSkillField field)
		{
			int max = (int)(Math.PI + ClassLevel);
			if (field.ClassSkill)
			{
				return max;
			}
			else
			{
				return (float)Math.Round(max / 2.0, 2);
			}
		}

		public virtual int[] SpellsPerDay()
		{
			return SpellsPerDay(0);
		}
		/// <summary>
		/// override if need be!
		/// </summary>
		public virtual int[] SpellsPerDay(int abilityModifier)
		{ // for all the non-caster classes:
			return new int[SpellAttribute.MAX_RANK +1] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		}

		public virtual int NumberofUnknownSpells(int rank)
		{
			return 0;
		}

		public int GetModifier(DnDAbilities ability)
		{
			if (mCharacter != null)
			{
				return mCharacter.Abilities[ability] / 2 - 5;
			}
			return 0;
		}

		public void ResetSpells()
		{
			ResetMainSpells();
			ResetExtraSpells();
		}

		protected abstract bool CanCastWithAbility(int rank);

		protected int SpellsPerDayPerModifier(int mod, int rank)
		{
			if (!CanCastWithAbility(rank))
				return 0; 
			if (mod <= 0 || rank <= 0 || mod - rank < 0)
			{
				return 0;
			}
			float result = (float)(mod - rank + 1) / 4f;
			return (int)Math.Ceiling(result);
		}

		private void ResetMainSpells()
		{
			mMainSpells.Clear();
			for (int i = 0; i <= HighestCastableRank; ++i)
			{
				List<KeyValuePair<Spell, bool>> list = new List<KeyValuePair<Spell, bool>>();
				mMainSpells.Add(list);
			}
		}

		private void ResetExtraSpells()
		{
			mExtraSpells = new List<KeyValuePair<Spell, bool>>();
			if (CanCastExtraSpell)
			{
				for (int i = 0; i <= HighestCastableRank; ++i)
				{
					mExtraSpells.Add(new KeyValuePair<Spell, bool>(new Spell(), true));
				}
			}
		}

		public void AddSpellToKnownList(Spell spell, int rank)
		{
			while (mKnownSpells.Count <= rank)
			{
				mKnownSpells.Add(new List<Spell>());
			}
			mKnownSpells[rank].Add(spell);
		}

		public void ReadyAllSpells()
		{
			List<List<KeyValuePair<Spell, bool>>> readyList = new List<List<KeyValuePair<Spell, bool>>>();
			foreach (List<KeyValuePair<Spell, bool>> rank in mMainSpells)
			{
				List<KeyValuePair<Spell, bool>> readyRank = new List<KeyValuePair<Spell, bool>>();
				foreach (KeyValuePair<Spell, bool> pair in rank)
				{
					readyRank.Add(new KeyValuePair<Spell, bool>(pair.Key, false));
				}
				readyList.Add(readyRank);
			}
			mMainSpells = readyList;
			if (CanCastExtraSpell)
			{
				List<KeyValuePair<Spell, bool>> readyExtra = new List<KeyValuePair<Spell, bool>>();
				for (int i = 0; i <= HighestCastableRank; ++i)
				{
					readyExtra.Add(new KeyValuePair<Spell, bool>(new Spell(), true));
				}
				for (int rank = 0; rank < mExtraSpells.Count; ++rank)
				{
					KeyValuePair<Spell, bool> pair = mExtraSpells[rank];
					readyExtra[rank] = new KeyValuePair<Spell, bool>(pair.Key, false);
				}
				mExtraSpells = readyExtra;
			}
		}

		public void PrepareMainSpell(Spell spell, int rank)
		{
			int number = SpellsPerDay()[rank];
			while (rank >= mMainSpells.Count)
			{
				mMainSpells.Add(new List<KeyValuePair<Spell, bool>>());
			}
			if (mMainSpells[rank].Count >= number)
			{
				int index = -1;
				for (int i = 0; i < mMainSpells[rank].Count; ++i)
				{
					if (mMainSpells[rank][i].Value)
					{
						index = i;
						break;
					}
				}
				if (index == -1)
				{
					mMainSpells[rank] = new List<KeyValuePair<Spell, bool>>();
				}
				else
				{
					mMainSpells[rank].RemoveAt(index);
				}
			}
			mMainSpells[rank].Add(new KeyValuePair<Spell, bool>(spell, false));
		}

		public void PrepareExtraSpell(Spell spell, int rank)
		{
			if (rank >= mExtraSpells.Count)
			{
				while (mExtraSpells.Count <= rank)
				{
					mExtraSpells.Add(new KeyValuePair<Spell, bool>());
				}
			}
			mExtraSpells[rank] = new KeyValuePair<Spell, bool>(spell, false);
		}

		public string ShortInfo()
		{
			return string.Format("{0} [{1}]", mClass, mClassLevel);
		}

		public int NumberOfPreparedMainSpells(int rank)
		{
			int result = 0;
			if (mMainSpells.Count <= rank)
				return result;
			foreach (KeyValuePair<Spell, bool> pair in mMainSpells[rank])
			{
				if (!pair.Value)
					result++;
			}
			return result;
		}

		public int NumberOfPreparedExtraSpells(int rank)
		{
			int result = 0;
			if (mExtraSpells.Count <= rank)
				return result;
			if (!mExtraSpells[rank].Value)
				result++;
			return result;
		}

		public abstract DnDSkillModel CreateSkillModel();

#region Serialization help
		protected JSONArray SerializeKnownSpells()
		{
			JSONArray array = new JSONArray();
			foreach(List<Spell> list in mKnownSpells)
			{
				JSONArray innerArray = new JSONArray();
				foreach (Spell spell in list)
				{
					innerArray.Add(spell.Serialize());
				}
				array.Add(innerArray);
			}
			return array;
		}
		protected void DeserializeKnownSpells(JSONObject obj)
		{
			mKnownSpells.Clear();
			JSONArray array = obj.GetArray(SerializableObject.KNOWN_SPELLS);
			foreach (var list in array)
			{
				List<Spell> rankList = new List<Spell>();
				foreach(var listObj in list.Array)
				{
					Spell spell = new Spell();
					spell.Deserialize(listObj.Obj);
					rankList.Add(spell);
				}
				mKnownSpells.Add(rankList);
			}
		}

		protected JSONArray SerializeMainSpells()
		{
			JSONArray array = new JSONArray();
			foreach (List<KeyValuePair<Spell, bool>> list in mMainSpells)
			{
				array.Add(SerializeListWithPair(list));
			}
			return array;
		}
		protected void DeserializeMainSpells(JSONObject obj)
		{
			mMainSpells.Clear();
			JSONArray array = obj.GetArray(SerializableObject.MAIN_SPELLS);
			foreach (var list in array)
			{
				mMainSpells.Add(DeserializeListWithPair(list.Array));
			}
		}

		protected JSONArray SerializeExtraSpells()
		{
			return SerializeListWithPair(mExtraSpells);
		}
		protected void DeserializeExtraSpells(JSONObject obj)
		{
			JSONArray array = obj.GetArray(SerializableObject.EXTRA_SPELLS);
			mExtraSpells = DeserializeListWithPair(array);
		}

		private JSONArray SerializeListWithPair(List<KeyValuePair<Spell, bool>> list)
		{
			JSONArray array = new JSONArray();
			foreach (var pair in list)
			{
				JSONObject pairObj = new JSONObject();
				pairObj.Add(SerializableObject.SPELL, pair.Key.Serialize());
				pairObj.Add(SerializableObject.USED, pair.Value);
				array.Add(pairObj);
			}
			return array;
		}
		private List<KeyValuePair<Spell, bool>> DeserializeListWithPair(JSONArray objArray)
		{
			List<KeyValuePair<Spell, bool>> list = new List<KeyValuePair<Spell, bool>>();
			foreach (var array in objArray)
			{
				Spell spell = new Spell();
				spell.Deserialize(array.Obj.GetObject(SerializableObject.SPELL));
				bool used = array.Obj.GetBoolean(SerializableObject.USED);
				list.Add(new KeyValuePair<Spell, bool>(spell, used));
			}
			return list;
		}
#endregion
	}
}
