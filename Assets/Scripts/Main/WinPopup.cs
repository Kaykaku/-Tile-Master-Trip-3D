using UnityEngine;

using TMPro;
using Extensions;

public class WinPopup : MonoBehaviour
{
	private System.Action OnClose;
	private System.Action OnContinue;
	public void Initialized(System.Action OnClose, System.Action OnContinue)
	{
		this.OnClose = OnClose;
		this.OnContinue = OnContinue;
	}
	public void CLoseBtn()
	{
		OnClose?.Invoke();
	}
	public void ContinueBtn()
	{
		OnContinue?.Invoke();
	}
}
