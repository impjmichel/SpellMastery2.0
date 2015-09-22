using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SpellMastery.Model;
using SpellMastery.Model.DnD;

namespace SpellMastery.View.Util
{
	public static class ViewUtility 
	{
		private static List<char> mReplaceDictionary = new List<char>()
		{
			//'A',
			'B',
			//'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			//'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			//'Y',
			'Z'
		};
		private const string cSpellSummonMonster = "Summon Monster";

		public static List<string> AvatarList = new List<string>()
		{
			"avatar_basic",
			"avatar_adventurer",
			"avatar_beast_druid",
			"avatar_death",
			"avatar_female_beast_ranger",
			"avatar_female_caster",
			"avatar_female_elf_druid",
			"avatar_female_elf_druid2",
			"avatar_female_hooded",
			"avatar_female_human_paladin",
			"avatar_female_human_ranger",
			"avatar_female_monster",
			"avatar_female_paladin",
			"avatar_female_ranger",
			"avatar_female_warrior",
			"avatar_female_wizard",
			"avatar_frog_warrior",
			"avatar_male_barbarian",
			"avatar_male_barbarian2",
			"avatar_male_cleric",
			"avatar_male_dwarf",
			"avatar_male_dwarf_warrior",
			"avatar_male_human_ranger",
			"avatar_male_ranger",
			"avatar_male_sorcerer",
			"avatar_orc",
			"avatar_puppeteer"
		};

		public static void ChangeSimpleButtonText(string text, GameObject button)
		{
			int count = button.transform.childCount;
			for (int i = 0; i < count; ++i)
			{
				Text textObj = button.transform.GetChild(i).GetComponent<Text>();
				if (textObj != null)
				{
					textObj.text = text;
				}
			}
		}

		public static void ChangeTwoPartButtonText(string left, string right, GameObject button)
		{
			GameObject go = button.transform.FindChild("Left").gameObject;
			ChangeSimpleButtonText(left, go);

			go = button.transform.FindChild("Right").gameObject;
			ChangeSimpleButtonText(right, go);
		}

		/// <summary>
		/// Normally used for preparing spells
		/// </summary>
		public static void ChangeDualButtonText(GameObject button, string mainText, string mainTop = "Prepare:", string extra = "Prepare as:\nExtra Spell")
		{
			GameObject go = button.transform.FindChild("ButtonLeft").FindChild("Bottom").gameObject;
			ChangeSimpleButtonText(mainText, go);

			go = button.transform.FindChild("ButtonLeft").FindChild("Top").gameObject;
			ChangeSimpleButtonText(mainTop, go);

			go = button.transform.FindChild("ButtonRight").gameObject;
			ChangeSimpleButtonText(extra, go);
		}

		public static string MakeStringVivaldiViable(string input)
		{
			string result = input;
			if (result.Contains(cSpellSummonMonster))
			{
				string removedName = result.Replace(cSpellSummonMonster, "{0}").ToLower();
				result = string.Format(removedName, cSpellSummonMonster);
			}

			for (int i = 0; i < result.Length; ++i)
			{
				int next = i + 1;
				if (next < result.Length)
				{
					if (result[next] != ' ' || result[next] != '\n' || result[next] != '\t')
					{
						if (mReplaceDictionary.Contains(result[i]))
						{
							result = result.Insert(next, " ");
						}
					}
				}
			}
			return result;
		}

		public static void EnableSimpleButton(GameObject button, bool enable)
		{
			if (enable)
			{
				button.GetComponent<CanvasGroup>().alpha = 1f;
				button.GetComponent<Button>().interactable = true;
			}
			else
			{
				button.GetComponent<CanvasGroup>().alpha = .4f;
				button.GetComponent<Button>().interactable = false;
			}
		}

		public static void EnableDualButton(GameObject button, bool enableLeft, bool enableRight)
		{
			GameObject go = button.transform.FindChild("ButtonLeft").gameObject;
			EnableSimpleButton(go, enableLeft);
			go = button.transform.FindChild("ButtonRight").gameObject;
			EnableSimpleButton(go, enableRight);
		}

		public static string ToViewString(CharacterGame game)
		{
			switch (game)
			{
				case CharacterGame.DnD_3_5:
					return "D &D  3.5";
				case CharacterGame.DnD_5_0:
					return "D &D  5";
				case CharacterGame.Pathfinder:
					return "Pathfinder";
				default:
					return "";
			}
		}
	
		public static string ToViewString(DnDRace race)
		{
			switch (race)
			{
				case DnDRace.HalfElf:
					return "Half Elf";
				case DnDRace.HalfOrc:
					return "Half Orc";
				default:
					return race.ToString();
			}
		}

		public static string ToViewString(DnDAlignment alignment)
		{
			switch (alignment)
			{
				case DnDAlignment.TrueNeutral:
					return "True Neutral";
				case DnDAlignment.NeutralGood:
					return "Neutral Good";
				case DnDAlignment.NeutralEvil:
					return "Neutral Evil";
				case DnDAlignment.LawfulNeutral:
					return "Lawful Neutral";
				case DnDAlignment.LawfulGood:
					return "Lawful Good";
				case DnDAlignment.LawfulEvil:
					return "Lawful Evil";
				case DnDAlignment.ChaoticNeutral:
					return "Chaotic Neutral";
				case DnDAlignment.ChaoticGood:
					return "Chaotic Good";
				case DnDAlignment.ChaoticEvil:
					return "Chaotic Evil";
				default:
					return alignment.ToString();
			}
		}

		public static Sprite CreateSpriteUseTexture(byte[] texture, Image targetImage)
		{
			if(texture == null)
			{
				return null;
			}
			Texture2D m = new Texture2D((int)targetImage.rectTransform.rect.width, (int)targetImage.rectTransform.rect.height, TextureFormat.RGB24, false);
			m.LoadImage(texture);
			return Sprite.Create(m, new Rect(0, 0, m.width, m.height), targetImage.sprite.pivot);
		}
	}
}
