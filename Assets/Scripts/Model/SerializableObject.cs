using Boomlagoon.JSON;

namespace SpellMastery.Model
{
	public abstract class SerializableObject
	{
		protected const string ABILITIES = "abilities";
		protected const string AGE = "age";
		protected const string AVATAR = "avatar";
		protected const string ALIGNMENT = "alignment";
		protected const string CLASS = "class";
		protected const string CLASS_SOULS = "classSouls";
		protected const string EXPERIENCE = "experience";
		protected const string EXTRA_SPELLS = "extraSpells";
		protected const string DEITY = "deity";
		protected const string GENDER = "gender";
		protected const string KNOWN_SPELLS = "knownSpells";
		protected const string LEVEL = "level";
		protected const string MAIN_SPELLS = "mainSpells";
		protected const string NAME = "name";
		protected const string RACE = "race";
		protected const string SIZE = "size";
		protected const string SKILLS = "skills";
		protected const string SPECIALIZATION = "specialization";
		protected const string SPELL = "spell";
		protected const string USED = "used";

		public const string CHARACTER_LIST = "characters";

		public abstract JSONObject Serialize();

		public abstract void Deserialize(JSONObject obj);
	}
}
