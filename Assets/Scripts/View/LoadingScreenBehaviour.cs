using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SpellMastery.View
{
	public class LoadingScreenBehaviour : MonoBehaviour
	{
		private const int cRotationModulo = 360;
		private const int cSmallStep = 12;
		private const int cBigStep = -8;

		private bool mIsRotating = false;
		private bool mRotate = true;

		/// <summary>
		/// Attach in Unity
		/// </summary>
		public RectTransform[] SmallSymbols;
		/// <summary>
		/// Attach in Unity
		/// </summary>
		public RectTransform[] BigSymbols;

		void Update()
		{
			if (mRotate && !mIsRotating)
			{
				StartCoroutine(FixedRotation());
			}
		}

		private IEnumerator FixedRotation()
		{
			mIsRotating = true;
			while (mRotate)
			{
				foreach (var symbol in SmallSymbols)
				{
					symbol.Rotate(0, 0, cSmallStep);
				}
				foreach (var symbol in BigSymbols)
				{
					symbol.Rotate(0, 0, cBigStep);
				}
				yield return new WaitForSeconds(0.005f);
			}
			mIsRotating = false;
		}
	}
}
