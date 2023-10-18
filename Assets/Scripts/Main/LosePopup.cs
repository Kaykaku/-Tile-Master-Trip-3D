using UnityEngine;

public class LosePopup : MonoBehaviour
{
	private System.Action OnClose;
	private System.Action OnRetry;
	public void Initialized(System.Action OnClose, System.Action OnRetry)
	{
		this.OnClose = OnClose;
		this.OnRetry = OnRetry;
	}
	public void CLoseBtn()
	{
		OnClose?.Invoke();
	}
	public void RetryBtn()
	{
		OnRetry?.Invoke();
	}
}
