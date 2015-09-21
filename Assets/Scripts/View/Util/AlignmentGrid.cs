using SpellMastery.Control;
using SpellMastery.Model.DnD;
using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View.Util
{
	public class AlignmentGrid : MonoBehaviour
	{
		private int mPreviousSelected = 0;
		private int mRows = 3;
		private int mColumns = 3;

		public DnDAlignment SelectedAlignment { get; private set; }
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public GameObject ButtonBox;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public GameObject ImageBox;

		private void Start()
		{
			if (AppStorage.Instance.CurrentCharacter != null)
			{
				if (AppStorage.Instance.CurrentCharacter.GetType() == typeof(DnDCharacter))
				{
					DnDCharacter character = AppStorage.Instance.CurrentCharacter as DnDCharacter;
					SelectedAlignment = character.Alignment;
				}
				else
				{
					SelectedAlignment = DnDAlignment.TrueNeutral;
				}
			}
			else
			{
				SelectedAlignment = DnDAlignment.TrueNeutral;
			}
			mPreviousSelected = (int)SelectedAlignment;
			if (ButtonBox != null)
			{
				for (int row = 0; row < mRows; ++row)
				{
					for (int column = 0; column < mColumns; ++column)
					{
						Button button = ButtonBox.transform.GetChild(row).GetChild(column).GetComponent<Button>();
						int notification = row * mRows + column;
						button.onClick.AddListener(() => OnSelectedButtonChange(notification));
					}
				}
				OnSelectedButtonChange(mPreviousSelected);
			}
		}

		private void OnSelectedButtonChange(int notification)
		{
			if (ImageBox != null)
			{
				SelectedAlignment = (DnDAlignment)notification;
				ImageBox.transform.GetChild(mPreviousSelected / mRows).GetChild(mPreviousSelected % mColumns).GetChild(0).gameObject.SetActive(false);
				ImageBox.transform.GetChild(notification / mRows).GetChild(notification % mColumns).GetChild(0).gameObject.SetActive(true);
				mPreviousSelected = notification;
			}
		}
	}
}
