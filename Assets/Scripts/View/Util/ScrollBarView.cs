using UnityEngine;
using UnityEngine.UI;

namespace SpellMastery.View.Util
{
	public class ScrollBarView : MonoBehaviour
	{
		private const float cScrollDistance = -100f;

		private RectTransform mTouchArea;
		private bool mDragging = false;
		private bool mWasScrolled = false;
		private bool mTouched = true;
		private Vector2 mDragStartPos;
		private Vector2 mDeltaDragPos;

		public Scrollbar AttachedScrollBar;

		public bool IsNeeded
		{
			get
			{
				return AttachedScrollBar.size < 1.0f;
			}
		}

		public bool IsDragged
		{
			get { return mDragging; }
		}

		public bool WasScrolled
		{
			get { return mWasScrolled; }
		}

		public Vector2 DragStartPos
		{
			get { return mDragStartPos; }
		}

		public Vector2 DeltaDragPos
		{
			get { return mDeltaDragPos; }
		}

		public float ScrollBarValue
		{
			get
			{
				if (AttachedScrollBar != null)
				{
					return AttachedScrollBar.value;
				}
				return 0;
			}
			set
			{
				if (AttachedScrollBar != null)
				{
					AttachedScrollBar.value = value;
				}
			}
		}

		public void Init(float currentSize, float totalSize, RectTransform touchArea = null)
		{
			if (touchArea != null)
			{
				mTouchArea = touchArea;
			}
			float percentageScroll = currentSize / totalSize;
			if (percentageScroll > 1f)
				percentageScroll = 1f;
			if (percentageScroll < 0f)
				percentageScroll = 0f;
			AttachedScrollBar.size = percentageScroll;
		}

		// Update is called once per frame
		private void Update()
		{
			if (mTouchArea != null)
			{
				Vector2 scroll = Input.mouseScrollDelta;
				bool scrolling = scroll.y > 0 || scroll.y < 0;
				if (scrolling)
				{
					mDeltaDragPos = scroll * cScrollDistance;
					mDragging = true;
					mWasScrolled = true;
				}
				else if (Input.touchCount > 0)
				{
					mWasScrolled = false;
					if (mDragging)
					{
						mDeltaDragPos = Input.GetTouch(0).deltaPosition;
					}
					else
					{
						float y_max = mTouchArea.position.y + mTouchArea.rect.height / 2f;
						float y_min = mTouchArea.position.y - mTouchArea.rect.height / 2f;

						float x_max = mTouchArea.position.x + mTouchArea.rect.width / 2f;
						float x_min = mTouchArea.position.x - mTouchArea.rect.width / 2f;

						Vector2 position = Input.GetTouch(0).position;
						if (position.x < x_max && position.x > x_min) // X constraints
						{
							if (position.y < y_max && position.y > y_min) // Y constraints
							{
								if (mTouched)
								{
									if (position.x != mDragStartPos.x || position.y != mDragStartPos.y)
									{
										mDragging = true;
									}
								}
								else
								{
									mTouched = true;
									mDragStartPos = position;
								}
							
							}
						}
					}
				}
				else
				{
					mDragging = false;
					mTouched = false;
				}
			}
		}
	}
}
