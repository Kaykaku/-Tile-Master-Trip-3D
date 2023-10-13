using UnityEngine;
namespace Services
{
	public class DisplayService
	{
#if UNITY_EDITOR

		public bool IsFake { get; set; }
#endif
		/// <summary>
		/// Initiate display service. Set targetframe to 60.
		/// </summary>
		public DisplayService()
		{
			Application.targetFrameRate = 60;
		}
		/// <summary>
		/// Return true if screen is wide.
		/// </summary>
		/// <returns></returns>
		public bool WideScreen()
		{
			if ((float)Screen.width / (float)Screen.height > 0.58f)
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// Return safe area.
		/// </summary>
		/// <returns></returns>
		public Rect SafeArea()
		{
#if UNITY_EDITOR
			if (IsFake)
			{
				return new Rect(0f, 90f, Screen.width, Screen.height - 90f);
			}
			else
#endif
			{
				Rect safeArea = Screen.safeArea;
				if (safeArea.y == 0)
				{
					float posY = safeArea.height;
					foreach (Rect rect in Screen.cutouts)
					{
						if (posY > rect.y)
						{
							posY = rect.y;
						}
					}
					safeArea.y = Screen.height - posY;
				}
				return Screen.safeArea;
			}
		}
		/// <summary>
		/// Return list of cutouts
		/// </summary>
		/// <returns></returns>
		public Rect[] Cutouts()
		{
			return Screen.cutouts;
		}
	}
}
