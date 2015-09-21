using Boomlagoon.JSON;
using SpellMastery.Model;
using SpellMastery.Model.DnD;
using System.Collections.Generic;
using UnityEngine;

namespace SpellMastery.Control
{
	public class AppStorage
	{
		/// <summary>
		/// The number for a version control system thing. Any change results in a "memory wipe".
		/// </summary>
		private const string cVersion = "2.0.0";
		private const string cCharFileName = "/char.data";

		private static AppStorage mInstance;

		private List<PlayerCharacter> mCharacters = new List<PlayerCharacter>();
		private PlayerCharacter mCurrentCharacter = null;
		private SpellList mSpellList;
		private string mFilePath;

		public static AppStorage Instance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = new AppStorage();
				}
				return mInstance;
			}
		}

		public string Version
		{
			get { return cVersion; }
		}

		public List<PlayerCharacter> Characters
		{
			get { return mCharacters; }
		}

		public PlayerCharacter CurrentCharacter
		{
			get { return mCurrentCharacter; }
			set { mCurrentCharacter = value; }
		}

		public SpellList SpellList
		{
			get { return mSpellList; }
		}

		public void SaveNewCharacter(PlayerCharacter character)
		{
			mCharacters.Add(character);
			WriteCharactersToFile();
		}

		public void SaveCharacters()
		{
			WriteCharactersToFile();
		}

		/// <summary>
		/// returns true if the current version is newer or equal to online support version
		/// </summary>
		public bool VersionControl(string onlineVersion)
		{
			string[] onlinePieces = onlineVersion.Split('.');
			string[] offlinePieces = cVersion.Split('.');
			for (int i = 0; i < offlinePieces.Length; ++i)
			{
				if (onlinePieces.Length > i)
				{
					int offline = int.Parse(offlinePieces[i]);
					int online = int.Parse(onlinePieces[i]);
					if (online < offline)
					{
						break;
					}
					else if (online > offline)
					{
						return false;
					}
				}
			}
			return true;
		}

		private AppStorage()
		{
			mFilePath = Application.persistentDataPath;
			mSpellList = new SpellList();
			InitCharacters();
		}

		private void InitCharacters()
		{
			string data = "";
			data = IOManager.ReadData(mFilePath + cCharFileName);
			if (!string.IsNullOrEmpty(data))
			{
				mCharacters.Clear();
				JSONObject obj = JSONObject.Parse(data);
				JSONArray array = obj.GetArray(SerializableObject.CHARACTER_LIST);
				foreach (var val in array)
				{
					PlayerCharacter newChar = null;
					CharacterGame game = (CharacterGame)(int)val.Array[0].Number;
					switch (game)
					{
						case CharacterGame.DnD_3_5:
							newChar = new DnDCharacter();
							break;
						default:
							break;
					}
					if (newChar != null)
					{
						newChar.Deserialize(val.Array[1].Obj);
						mCharacters.Add(newChar);
					}
				}
			}
		}

		private void WriteCharactersToFile()
		{
			JSONObject data = CharactersJSON();
			IOManager.WriteData(mFilePath + cCharFileName, data.ToString());
		}

		private JSONObject CharactersJSON()
		{
			JSONObject obj = new JSONObject();
			JSONArray array = new JSONArray();
			foreach (PlayerCharacter character in mCharacters)
			{
				JSONArray tempArray = new JSONArray();
				tempArray.Add((int)character.Game);
				tempArray.Add(character.Serialize());
				array.Add(tempArray);
			}
			obj.Add(SerializableObject.CHARACTER_LIST, array);
			return obj;
		}
	}
}

