using UnityEngine;

namespace SpellMastery.View.Util
{
	public class IntButtonHandler : MonoBehaviour 
	{
		public IButtonHandler NotificationCatcher;
		public int NotificationInt = 0;
		public int ButtonID = 0;

		public void OnClick_WithNotification()
		{
			NotificationCatcher.ButtonCLickHandler(NotificationInt, ButtonID);
		}
	}
}
