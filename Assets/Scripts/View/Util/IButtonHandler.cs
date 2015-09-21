using System.Collections.Generic;
using UnityEngine;

namespace SpellMastery.View.Util
{
	public interface IButtonHandler
	{
		List<GameObject> ButtonList
		{
			get; set;
		}

		void ButtonCLickHandler(int notification, int senderID);

		void InitButtons();

		void DeInitButtons();
	}
}
