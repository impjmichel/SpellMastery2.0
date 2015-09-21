using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SpellMastery.View.Util
{
	public abstract class ScrollViewScreen : ScreenBase, IMainScreen
	{
		private const float cDragMultiplier = 1.25f;

		private Vector2 mDeltaPos;
		private Vector2 mPrevDeltaPos = new Vector2(0,0);
		private bool mJustLiftedFinger;
		private bool mUpdateNow = true;

		protected bool mInitialized = false;

		protected int? mTotalHeight;
		protected int? mViewHeight;

		public ScrollBarView ScrollView;
		public RectTransform DragArea;

		public bool UpdateNow
		{
			get { return mUpdateNow; }
			set { mUpdateNow = value; }
		}

		public override void Back()
		{
			Reset();
		}

		public override void Update()
		{
			base.Update();
			if (!mInitialized || mUpdateNow)
			{
				Init();
			}
			if (ScrollView.gameObject.activeInHierarchy && mTotalHeight.HasValue && mViewHeight.HasValue)
			{
				RectTransform rect = DragArea.transform.FindChild("ScrollArea").GetComponent<RectTransform>();
				float maxHeight = (mTotalHeight.Value - mViewHeight.Value);
				if (ScrollView.IsDragged)
				{
					mDeltaPos = ScrollView.DeltaDragPos;
					float top = rect.offsetMax.y + mDeltaPos.y * cDragMultiplier;
					if (top > maxHeight)
						top = maxHeight;
					if (top < 0)
						top = 0;
					rect.offsetMax = new Vector2(0, top);
					ScrollView.ScrollBarValue = top / maxHeight;
					if ((mDeltaPos.y * mDeltaPos.y) > (mPrevDeltaPos.y * mPrevDeltaPos.y))
					{
						mPrevDeltaPos = mDeltaPos;
					}
					mJustLiftedFinger = true;
				}
				else
				{
					if (mJustLiftedFinger)
					{
						
						if (!ScrollView.WasScrolled)
						{
							StartCoroutine(AfterLiftingFinger());
						}
						mJustLiftedFinger = false;
					}
					else
					{
						mDeltaPos = new Vector2(0, 0);
					}
				}
			}
		}

		protected void Start()
		{
			mViewHeight = (int)DragArea.rect.height;
		}

		protected void Init()
		{
			if (mTotalHeight.HasValue && mViewHeight.HasValue)
			{
				if (DragArea != null && ScrollView != null)
				{
					mViewHeight = (int)DragArea.rect.height;
					ScrollView.Init(mViewHeight.Value, mTotalHeight.Value, DragArea);
					ScrollView.gameObject.SetActive(ScrollView.IsNeeded);
					mInitialized = true;
					mUpdateNow = false;
				}
			}
		}

		protected void Reset()
		{
			RectTransform rect = DragArea.transform.FindChild("ScrollArea").GetComponent<RectTransform>();
			rect.offsetMax = new Vector2(0, 0);
			mInitialized = false;
			mTotalHeight = new int?();
		}

		private IEnumerator AfterLiftingFinger()
		{
			float delta = mPrevDeltaPos.y * cDragMultiplier;
			mPrevDeltaPos = new Vector2(0, 0);
			float deltaStep = .7f;
			bool reverse = delta < 0;
			if (reverse)
				deltaStep *= -1f;
			float maxHeight = (mTotalHeight.Value - mViewHeight.Value);
			RectTransform rect = DragArea.transform.FindChild("ScrollArea").GetComponent<RectTransform>();
			while (delta != 0f)
			{
				float top = rect.offsetMax.y + delta;
				if (top > maxHeight)
					top = maxHeight;
				if (top < 0)
					top = 0;
				rect.offsetMax = new Vector2(0, top);
				ScrollView.ScrollBarValue = top / maxHeight;
				yield return new WaitForEndOfFrame();
				delta -= deltaStep;
				deltaStep *= 1.1f;
				if ( (delta < 0f && !reverse) || (delta > 0f && reverse) )
					delta = 0f;
			}
		}
	}
}
