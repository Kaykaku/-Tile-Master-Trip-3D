using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : MonoBehaviour
{
	private System.Action OnClose;
	private System.Action OnQuit;
	public void Initialized(System.Action OnClose, System.Action OnQuit)
	{
		this.OnClose = OnClose;
		this.OnQuit = OnQuit;
	}
	public void CLoseBtn()
	{
		OnClose?.Invoke();
	}
	public void QuitBtn()
	{
		OnQuit?.Invoke();
	}
}
