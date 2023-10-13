using UnityEngine;
using UnityEngine.EventSystems;

namespace Services
{
	public class InputService
	{
		private EventSystem eventSystem;
		private bool isSkipFrame;

		public InputService()
		{
			Input.multiTouchEnabled = false;
		}
		public InputService(EventSystem eventSystem)
		{
			Input.multiTouchEnabled = false;
			this.eventSystem = eventSystem;
		}

		public Vector2 GetTouchPosition()
		{
			if (Input.touchSupported == true)
			{
				if (Input.touchCount <= 0) return Vector2.zero;
				UnityEngine.Touch unityTouch = Input.GetTouch(0);
				return unityTouch.position;
			}
			else
			{
				return (Vector2)Input.mousePosition;
			}
		}

		public TouchPhase GetTouchPhase()
		{
			if (Input.touchSupported == true)
			{
				if (Input.touchCount <= 0 || isSkipFrame)
				{
					isSkipFrame = false;
					return TouchPhase.None;
				}
				UnityEngine.Touch unityTouch = Input.GetTouch(0);
				// Phase
				if (unityTouch.phase == UnityEngine.TouchPhase.Began)
				{
					return TouchPhase.Down;
				}
				else if (unityTouch.phase == UnityEngine.TouchPhase.Ended || unityTouch.phase == UnityEngine.TouchPhase.Canceled)
				{
					return TouchPhase.Up;
				}
				else if (unityTouch.phase == UnityEngine.TouchPhase.Moved || unityTouch.phase == UnityEngine.TouchPhase.Stationary)
				{
					return TouchPhase.Move;
				}
			}
			else
			{
				// Phase
				if (Input.GetMouseButtonDown(0))
				{
					return TouchPhase.Down;
				}
				else if (Input.GetMouseButtonUp(0))
				{
					return TouchPhase.Up;
				}
				else if (Input.GetMouseButton(0))
				{
					return TouchPhase.Move;
				}
			}
			return TouchPhase.None;
		}
		public bool IsTouch()
		{
			if (Input.touchSupported == true)
			{
				return Input.touchCount > 0;
			}
			else if (GetTouchPhase() != TouchPhase.None)
			{
				return true;
			}
			return false;
		}
		public bool IsMoveOnUI()
		{
			eventSystem = EventSystem.current;
			if (eventSystem == null)
			{
				isSkipFrame = true;
				return true;
			}
			if (eventSystem.IsPointerOverGameObject() == true || eventSystem.IsPointerOverGameObject(0))
			{
				isSkipFrame = true;
				return true;
			}
			return false;
		}
	}
	public enum TouchPhase
	{
		None,
		Down,
		Move,
		Up
	}
}
