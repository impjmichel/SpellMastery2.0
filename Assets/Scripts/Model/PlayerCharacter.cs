namespace SpellMastery.Model
{
public enum CharacterGame
{
	None = 0,
	DnD_3_5 = 1,
	DnD_5_0 = 2,
	Pathfinder = 3
}

public enum CharacterGender
{
	Female = 0,
	Undefined = 1,
	Male = 2
}

	public abstract class PlayerCharacter : SerializableObject
	{
		private const string cBasicAvatar = "img/avatars/not_loaded";

		protected CharacterGame mGame = CharacterGame.None;
		protected CharacterGender mGender = CharacterGender.Undefined;
		protected int mExperience = 0;
		protected string mName = string.Empty;
		protected string mAvatar = cBasicAvatar;
	
		public byte[] AvatarBytes { get; set; }

		public CharacterGame Game
		{
			get { return mGame; }
		}

		public CharacterGender Gender
		{
			get { return mGender; }
			set { mGender = value; }
		}

		public int Experience
		{
			get { return mExperience; }
			set { mExperience = value; }
		}

		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}

		public string Avatar
		{
			get { return mAvatar; }
			set { mAvatar = value; }
		}

		public virtual bool NeedsToPrepareSpells
		{
			get { return false; }
		}

		protected PlayerCharacter(string name)
		{
			mName = name;
		}
	}
}
