using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Extensions;
using Game;

public class BoosterPopup : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI amountText;
	[SerializeField] private TextMeshProUGUI priceText;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI descriptionText;
	[SerializeField] private Image boosterIcon;

	private System.Action onClose;
	private System.Action onBuy;
	private System.Action onAdsBuy;

	private void Awake()
	{
		amountText.ThrowIfNull();
		priceText.ThrowIfNull();
		boosterIcon.ThrowIfNull();
	}
	public void Initialized(System.Action onClose, System.Action onBuy, System.Action onAds, BoosterInfo boosterInfo)
	{
		this.onClose = onClose;
		this.onBuy = onBuy;
		this.onAdsBuy = onAds;
		
		this.titleText.text = boosterInfo.Name.ToString();
		this.descriptionText.text = boosterInfo.Description.ToString();
		this.priceText.text = boosterInfo.Price.ToString();
		this.amountText.text = boosterInfo.Amount.ToString();
		this.boosterIcon.sprite = boosterInfo.Sprite;
	}
	public void CloseButton()
	{
		onClose?.Invoke();
	}
	public void BuyButton()
	{
		onBuy?.Invoke();
	}
	public void AdsButton()
	{
		onAdsBuy?.Invoke();
	}
}
