using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpellMastery.Control;
using SpellMastery.View.Util;
using System;

namespace SpellMastery.View
{
	public class MainMenu : ScreenBase
	{
		private const int cExpectedmessageLength = 3;
		private const string cVersion = "SpellMastery   version {0}";
		private const string cVersionMessageHeader = "SpellMastery\nv{0} released!";
		private const string cVersionMessageBody = "Please download a newer verion of the application or contact imp for more or less information.";
		private const string cVersionMessageButton = "Exit application";
		private const string cSpellListErrorHeader = "Ooops!";
		private const string cSpellListErrorBody = "There seems to be an error with the spell list, you can continue, but keep in mind that you won't have any spells unless you restart the app with internet access.";
		private const string cSpellListErrorButton = "O k";

		private bool QuitOnCloseMessage = false;

		/// <summary>
		/// Attach in Unity
		/// </summary>
		public GameObject MessageBox;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public Text VersionText;

		public void OnClick_SelectCharacter()
		{
			// change later:
			SwitchScene(SCENE_PROFILE);
		}

		public void OnClick_CreateNew()
		{
			SwitchScene(SCENE_CREATION);
		}

		public void OnClick_MessageButton()
		{
			if (QuitOnCloseMessage)
			{
				Application.Quit();
			}
			else
			{
				CloseMessageBox();
			}
		}

		private void Start()
		{
			CloseMessageBox();
			if (LoadingScreen != null)
			{
				LoadingScreen.SetActive(true);
			}
			if (VersionText != null)
			{
				VersionText.text = string.Format(cVersion, AppStorage.Instance.Version);
			}
			StartCoroutine(OnlineControl());
		}

		public override void Update()
		{
			if (MessageBox != null)
			{
				if (Input.GetKeyUp(KeyCode.Escape))
				{
					if (MessageBox.activeInHierarchy)
					{
						OnClick_MessageButton();
					}
					else
					{
						Application.Quit();
					}
				}
			}
		}

		private IEnumerator OnlineControl()
		{
			yield return StartCoroutine(AppStorage.Instance.SpellList.GetOnlineList());

			if (!string.IsNullOrEmpty(AppStorage.Instance.SpellList.Version))
			{
				bool versionOk = AppStorage.Instance.VersionControl(AppStorage.Instance.SpellList.Version);
				yield return new WaitForEndOfFrame();
				if (versionOk)
				{
					//check message
					MessageControl();
				}
				else
				{
					// showing out-of-date message
					string title = string.Format(cVersionMessageHeader, AppStorage.Instance.SpellList.Version);
					QuitOnCloseMessage = true;
					ShowMessageBox(title, cVersionMessageBody, cVersionMessageButton);
				}
				yield return new WaitForEndOfFrame();
			}
			else
			{
				MessageControl();
			}
			yield return new WaitForSeconds(0.5f);
			if (LoadingScreen != null)
			{
				LoadingScreen.SetActive(false);
			}
		}

		private void ShowMessageBox(string title, string body, string buttonText)
		{
			if (MessageBox != null)
			{
				MessageBox.transform.FindChild("Background").FindChild("Top").FindChild("TitleText").GetComponent<Text>().text = title;
				MessageBox.transform.FindChild("Background").FindChild("Body").FindChild("BodyText").GetComponent<Text>().text = body;
				GameObject button = MessageBox.transform.FindChild("Background").FindChild("Bottom").FindChild("Button").gameObject;
				ViewUtility.ChangeSimpleButtonText(buttonText, button);
				MessageBox.gameObject.SetActive(true);
			}
		}

		private void CloseMessageBox()
		{
			if (MessageBox != null)
			{
				MessageBox.gameObject.SetActive(false);
			}
		}

		private void MessageControl()
		{
			if (AppStorage.Instance.SpellList.Message != null)
			{
				// this should be the only function to use these PlayerPref strings!
				string previous1 = PlayerPrefs.GetString("message1");
				string previous2 = PlayerPrefs.GetString("message2");
				string previous3 = PlayerPrefs.GetString("message3");
				List<string> message = AppStorage.Instance.SpellList.Message;
				if (message.Count == cExpectedmessageLength)
				{
					if (previous1 != message[0] || previous2 != message[1] || previous3 != message[2])
					{
						ShowMessageBox(message[0], message[1], message[2]);
						PlayerPrefs.SetString("message1", message[0]);
						PlayerPrefs.SetString("message2", message[1]);
						PlayerPrefs.SetString("message3", message[2]);
						return; // skip the SpellList message, the current one might be more important.
					}
				}
			}
			SpellListControl();
		}

		private void SpellListControl()
		{
			if (AppStorage.Instance.SpellList.Spells.Count == 0)
			{
				ShowMessageBox(cSpellListErrorHeader, cSpellListErrorBody, cSpellListErrorButton);
			}
		}

		public override void Next()
		{
			throw new NotImplementedException();
		}

		public override void Back()
		{
			throw new NotImplementedException();
		}
	}
}
