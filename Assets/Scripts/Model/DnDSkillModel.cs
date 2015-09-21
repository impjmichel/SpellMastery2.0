using Boomlagoon.JSON;
using System.Collections.Generic;

namespace SpellMastery.Model.DnD
{
	public class DnDSkillField : SerializableObject
	{
		private const string cTitle = "title";
		private const string cRanks = "ranks";
		private const string cModifier = "miscModifier";
		private const string cUntrained = "untrainedSkill";
		private const string cClassSkill = "classSkill";
		private const string cAbility = "ability";

		public string Title { get; set; }
		public int Ranks { get; set; }
		public int MiscModifier { get; set; }
		public bool CanBeUsedUntrained { get; set; }
		public bool ClassSkill { get; set; }
		public DnDAbilities KeyAbility { get; private set; }

		/// <summary>
		/// Use this constructor only for deserialization.
		/// </summary>
		public DnDSkillField()
		{
		}
		public DnDSkillField(int ranks, int miscMod, bool untrained, bool classSkill, DnDAbilities ability, string title = "")
		{
			Title = title;
			Ranks = ranks;
			MiscModifier = miscMod;
			CanBeUsedUntrained = untrained;
			ClassSkill = classSkill;
			KeyAbility = ability;
		}

		public override JSONObject Serialize()
		{
			JSONObject obj = new JSONObject();
			obj.Add(cRanks, Ranks);
			obj.Add(cModifier, MiscModifier);
			obj.Add(cUntrained, CanBeUsedUntrained);
			obj.Add(cClassSkill, ClassSkill);
			obj.Add(cAbility, (int)KeyAbility);
			return obj;
		}

		public override void Deserialize(JSONObject obj)
		{
			Ranks = (int)obj.GetNumber(cRanks);
			MiscModifier = (int)obj.GetNumber(cModifier);
			CanBeUsedUntrained = obj.GetBoolean(cUntrained);
			ClassSkill = obj.GetBoolean(cClassSkill);
			KeyAbility = (DnDAbilities)(int)obj.GetNumber(cAbility);

		}
	}

	public class DnDSkillModel : SerializableObject
	{
		private const string cAppraise = "appraise";
		private const string cBalance = "balance";
		private const string cBluff = "bluff";
		private const string cClimb = "climb";
		private const string cCraft = "craft";
		private const string cConcentration = "concentration";
		private const string cDecipherScript = "decipherScript";
		private const string cDiplomacy = "diplomacy";
		private const string cDisableDevice = "disableDevice";
		private const string cDisguise = "disguise";
		private const string cEscapeArtist = "escapeArtist";
		private const string cForgery = "forgery";
		private const string cGatherInformation = "gatherInformation";
		private const string cHandleAnimal = "handleAnimal";
		private const string cHeal = "heal";
		private const string cHide = "hide";
		private const string cIntimidate = "intimidate";
		private const string cJump = "jump";
		private const string cListen = "listen";
		private const string cKnowledge = "knowledge";
		private const string cMoveSilently = "moveSilently";
		private const string cOpenLock = "openLock";
		private const string cPerform = "perform";
		private const string cProfession = "profession";
		private const string cRide = "ride";
		private const string cSearch = "search";
		private const string cSenseMotive = "senseMotive";
		private const string cSleightOfHand = "sleightOfHand";
		private const string cSpellCraft = "spellCraft";
		private const string cSpot = "spot";
		private const string cSurvival = "survival";
		private const string cSwim = "swim";
		private const string cTumble = "tumble";
		private const string cUseMagicDevice = "useMagicDevice";
		private const string cUseRope = "useRope";

		private List<DnDSkillField> mCraft = new List<DnDSkillField>();
		private List<DnDSkillField> mKnowledge = new List<DnDSkillField>();
		private List<DnDSkillField> mPerform = new List<DnDSkillField>();
		private List<DnDSkillField> mProfession = new List<DnDSkillField>();

		public DnDSkillField Appraise { get; set; }
		public DnDSkillField Balance { get; set; }
		public DnDSkillField Bluff { get; set; }
		public DnDSkillField Climb { get; set; }
		public List<DnDSkillField> Craft { get { return mCraft; } }
		public DnDSkillField Concentration { get; set; }
		public DnDSkillField DecipherScript { get; set; }
		public DnDSkillField Diplomacy { get; set; }
		public DnDSkillField DisableDevice { get; set; }
		public DnDSkillField Disguise { get; set; }
		public DnDSkillField EscapeArtist { get; set; }
		public DnDSkillField Forgery { get; set; }
		public DnDSkillField GatherInformation { get; set; }
		public DnDSkillField HandleAnimal { get; set; }
		public DnDSkillField Heal { get; set; }
		public DnDSkillField Hide { get; set; }
		public DnDSkillField Intimidate { get; set; }
		public DnDSkillField Jump { get; set; }
		public List<DnDSkillField> Knowledge { get { return mKnowledge; } }
		public DnDSkillField Listen { get; set; }
		public DnDSkillField MoveSilently { get; set; }
		public DnDSkillField OpenLock { get; set; }
		public List<DnDSkillField> Perform { get { return mPerform; } }
		public List<DnDSkillField> Profession { get { return mProfession; } }
		public DnDSkillField Ride { get; set; }
		public DnDSkillField Search { get; set; }
		public DnDSkillField SenseMotive { get; set; }
		public DnDSkillField SleightOfHand { get; set; }
		public DnDSkillField SpellCraft { get; set; }
		public DnDSkillField Spot { get; set; }
		public DnDSkillField Survival { get; set; }
		public DnDSkillField Swim { get; set; }
		public DnDSkillField Tumble { get; set; }
		public DnDSkillField UseMagicDevice { get; set; }
		public DnDSkillField UseRope { get; set; }

		/// <summary>
		/// Not only adds a new craft but also removes all entries with the same title.
		/// </summary>
		public void AddCraftSkill(string title, int rank, int miscMod, bool classSkill)
		{
			mCraft.RemoveAll(x => x.Title == title);
			mCraft.Add(new DnDSkillField(rank, miscMod, true, classSkill, DnDAbilities.Intelligence, title));
		}

		/// <summary>
		/// Not only adds a new knowledge but also removes all entries with the same title.
		/// </summary>
		public void AddKnowledgeSkill(string title, int rank, int miscMod, bool classSkill)
		{
			mKnowledge.RemoveAll(x => x.Title == title);
			mKnowledge.Add(new DnDSkillField(rank, miscMod, false, classSkill, DnDAbilities.Intelligence, title));
		}

		/// <summary>
		/// Not only adds a new knowledge but also removes all entries with the same title.
		/// </summary>
		public void AddPerformSkill(string title, int rank, int miscMod, bool classSkill)
		{
			mPerform.RemoveAll(x => x.Title == title);
			mPerform.Add(new DnDSkillField(rank, miscMod, false, classSkill, DnDAbilities.Charisma, title));
		}

		/// <summary>
		/// Not only adds a new knowledge but also removes all entries with the same title.
		/// </summary>
		public void AddProfessionSkill(string title, int rank, int miscMod, bool classSkill)
		{
			mProfession.RemoveAll(x => x.Title == title);
			mProfession.Add(new DnDSkillField(rank, miscMod, false, classSkill, DnDAbilities.Wisdom, title));
		}

		public override void Deserialize(JSONObject obj)
		{
			JSONObject tempObj = obj.GetObject(cAppraise);
			DnDSkillField tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Appraise = tempField;
			tempObj = obj.GetObject(cBalance);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Balance = tempField;
			tempObj = obj.GetObject(cBluff);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Bluff = tempField;
			tempObj = obj.GetObject(cClimb);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Climb = tempField;
			tempObj = obj.GetObject(cConcentration);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Concentration = tempField;
			tempObj = obj.GetObject(cDecipherScript);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			DecipherScript = tempField;
			tempObj = obj.GetObject(cDiplomacy);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Diplomacy = tempField;
			tempObj = obj.GetObject(cDisableDevice);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			DisableDevice = tempField;
			tempObj = obj.GetObject(cDisguise);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Disguise = tempField;
			tempObj = obj.GetObject(cEscapeArtist);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			EscapeArtist = tempField;
			tempObj = obj.GetObject(cForgery);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Forgery = tempField;
			tempObj = obj.GetObject(cGatherInformation);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			GatherInformation = tempField;
			tempObj = obj.GetObject(cHandleAnimal);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			HandleAnimal = tempField;
			tempObj = obj.GetObject(cHeal);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Heal = tempField;
			tempObj = obj.GetObject(cHide);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Hide = tempField;
			tempObj = obj.GetObject(cIntimidate);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Intimidate = tempField;
			tempObj = obj.GetObject(cJump);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Jump = tempField;
			tempObj = obj.GetObject(cListen);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Listen = tempField;
			tempObj = obj.GetObject(cMoveSilently);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			MoveSilently = tempField;
			tempObj = obj.GetObject(cOpenLock);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			OpenLock = tempField;
			tempObj = obj.GetObject(cRide);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Ride  = tempField;
			tempObj = obj.GetObject(cSearch);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Search = tempField;
			tempObj = obj.GetObject(cSenseMotive);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			SenseMotive = tempField;
			tempObj = obj.GetObject(cSleightOfHand);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			SleightOfHand = tempField;
			tempObj = obj.GetObject(cSpellCraft);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			SpellCraft = tempField;
			tempObj = obj.GetObject(cSpot);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Spot = tempField;
			tempObj = obj.GetObject(cSurvival);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Survival = tempField;
			tempObj = obj.GetObject(cSwim);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Swim = tempField;
			tempObj = obj.GetObject(cTumble);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			Tumble = tempField;
			tempObj = obj.GetObject(cUseMagicDevice);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			UseMagicDevice = tempField;
			tempObj = obj.GetObject(cUseRope);
			tempField = new DnDSkillField();
			tempField.Deserialize(tempObj);
			UseRope = tempField;
		}

		public override JSONObject Serialize()
		{
			JSONObject obj = new JSONObject();
			obj.Add(cAppraise, Appraise.Serialize());
			obj.Add(cBalance, Balance.Serialize());
			obj.Add(cBluff, Bluff.Serialize());
			obj.Add(cClimb, Climb.Serialize());
			obj.Add(cConcentration, Concentration.Serialize());
			obj.Add(cDecipherScript, DecipherScript.Serialize());
			obj.Add(cDiplomacy, Diplomacy.Serialize());
			obj.Add(cDisableDevice, DisableDevice.Serialize());
			obj.Add(cDisguise, Disguise.Serialize());
			obj.Add(cEscapeArtist, EscapeArtist.Serialize());
			obj.Add(cForgery, Forgery.Serialize());
			obj.Add(cGatherInformation, GatherInformation.Serialize());
			obj.Add(cHandleAnimal, HandleAnimal.Serialize());
			obj.Add(cHeal, Heal.Serialize());
			obj.Add(cHide, Hide.Serialize());
			obj.Add(cIntimidate, Intimidate.Serialize());
			obj.Add(cJump, Jump.Serialize());
			obj.Add(cListen, Listen.Serialize());
			obj.Add(cMoveSilently, MoveSilently.Serialize());
			obj.Add(cOpenLock, OpenLock.Serialize());
			obj.Add(cRide, Ride.Serialize());
			obj.Add(cSearch, Search.Serialize());
			obj.Add(cSenseMotive, SenseMotive.Serialize());
			obj.Add(cSleightOfHand, SleightOfHand.Serialize());
			obj.Add(cSpellCraft, SpellCraft.Serialize());
			obj.Add(cSpot, Spot.Serialize());
			obj.Add(cSurvival, Survival.Serialize());
			obj.Add(cSwim, Swim.Serialize());
			obj.Add(cTumble, Tumble.Serialize());
			obj.Add(cUseMagicDevice, UseMagicDevice.Serialize());
			obj.Add(cUseRope, UseRope.Serialize());
			return obj;
		}
	}
}
