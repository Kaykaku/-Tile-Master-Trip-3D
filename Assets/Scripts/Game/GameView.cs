using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using TMPro;
using Extensions;

using Services;

namespace Game
{
    public class GameView : MonoBehaviour
    {
		[SerializeField] private Camera mainCamera;
		[SerializeField] private Transform environment;
		[SerializeField] private Canvas canvas;
		[SerializeField] private RectTransform mainRectTf;
		[SerializeField] private RectTransform boosterRectTf;

		[SerializeField] private Transform forwardBarrier;
		[SerializeField] private Transform backBarrier;
		[SerializeField] private Transform leftBarrier;
		[SerializeField] private Transform rightBarrier;
		[SerializeField] private Transform slotTf;

		[SerializeField] private GameObject comboGo;
		[SerializeField] private Slider comboSlider;

		[SerializeField] private TextMeshProUGUI comboTxt;
		[SerializeField] private TextMeshProUGUI timerTxt;
		[SerializeField] private TextMeshProUGUI levelTxt;
		[SerializeField] private TextMeshProUGUI starTxt;
		[SerializeField] private TextMeshProUGUI[] boosterAmountText;

		[Header("Popup")]
		[SerializeField] private GameObject mask;
		[SerializeField] private GameObject freeze;
		[SerializeField] private WinPopup winPopup;
		[SerializeField] private BoosterPopup boosterPopup;
		[SerializeField] private PausePopup pausePopup;
		[SerializeField] private LosePopup losePopup;

		private bool isStop;
		private int timer;
		private Coroutine comboCoroutine;
		private PlayerService playerService;
		private AudioService audioService;

		private UnityAction onTimeUp;
		private UnityAction onDisable;
		private UnityAction<bool> onContinue;
		private void Awake()
		{
			timerTxt.ThrowIfNull();
		}
		public void SetServices(PlayerService playerService , AudioService audioService)
		{
			this.playerService = playerService;
			this.audioService = audioService;
			UpdateInfo();
		}
		public void UpdateInfo()
		{
			boosterAmountText[0].text = playerService.GetBoosterAmount(BoosterType.Back).ToString();
			boosterAmountText[1].text = playerService.GetBoosterAmount(BoosterType.Magnet).ToString();
			boosterAmountText[2].text = playerService.GetBoosterAmount(BoosterType.Fan).ToString();
			boosterAmountText[3].text = playerService.GetBoosterAmount(BoosterType.Freeze).ToString();
		}
		public void StartCountDown()
		{
			RestartCountDown();
		}
		public void RestartCountDown()
		{
			StartCoroutine(StartTimer());
		}
		private IEnumerator StartTimer()
		{
			while (timer >= 0)
			{
				timerTxt.text = timer / 60 + " : " + (timer % 60 >= 10 ? string.Empty : "0") + timer % 60;
				yield return new WaitForSeconds(1f);
				if (!isStop) timer -= 1;
			}
			onTimeUp?.Invoke();
		}
		public void ShowCombo(int combo, float comboTimer ,System.Action onEndCombo)
        {
			comboGo.SetActive(true);
			comboSlider.value = 1f;
			comboTxt.text = combo.ToString();
			if(comboCoroutine != null)
            {
				StopCoroutine(comboCoroutine);
            }
			comboCoroutine = StartCoroutine(StartComboTimer(comboTimer, onEndCombo));
        }
		private IEnumerator StartComboTimer(float comboTime, System.Action onEndCombo)
		{
			float comboTimer = comboTime;
			while (comboTimer >= 0)
			{
				yield return null;
				comboTimer -= Time.deltaTime;
				comboSlider.value = comboTimer / comboTime;
			}
			onEndCombo?.Invoke();
			comboCoroutine = null;
			comboGo.SetActive(false);
		}
		public void Continue()
		{
			OpenPopup(SceneType.Game);
			freeze.SetActive(false);
			isStop = false;
			onContinue?.Invoke(true);
		}
		public void Pause()
		{
			isStop = true;
			onDisable?.Invoke();
			onContinue?.Invoke(false);
		}
		public void ShowStar(int star)
		{
			starTxt.text = star.ToString();
		}
		public void Init(LevelInfo levelInfo, UnityAction<bool> onContinue, UnityAction onDisable)
		{
			this.onContinue = onContinue;
			this.onDisable = onDisable;
			levelTxt.text = "LV " + (levelInfo.Level);
			timer = levelInfo.PlayTime;
			timerTxt.text = timer / 60 + " : " + (timer % 60 >= 10 ? string.Empty : "0") + timer % 60;
		}
		public void FreezePause()
		{
			freeze.SetActive(true);
			isStop = true;
		}
        public void OpenPopup(SceneType popup)
        {
			audioService.PlaySound(SoundType.Button);
			mask.SetActive(true);
            boosterPopup.gameObject.SetActive(false);
            pausePopup.gameObject.SetActive(false);
            winPopup.gameObject.SetActive(false);
            losePopup.gameObject.SetActive(false);

            switch (popup)
            {
                case SceneType.WinPopup:
					audioService.PlaySound(SoundType.Win);
					winPopup.gameObject.SetActive(true);
                    break;
				case SceneType.LosePopup:
					audioService.PlaySound(SoundType.Lose);
					losePopup.gameObject.SetActive(true);
					break;
				case SceneType.Pause:
                    pausePopup.gameObject.SetActive(true);
                    break;
                case SceneType.Booster:
                    boosterPopup.gameObject.SetActive(true);
                    break;
                case SceneType.NextLevel:
                    SceneManager.LoadScene(Constants.GameScene);
                    break;
                case SceneType.Main:
                    SceneManager.LoadScene(Constants.MainScene);
                    break;
                case SceneType.Game:
                    mask.SetActive(false);
                    break;
            }
        }
        public void BackToHome()
        {
			audioService.PlaySound(SoundType.Button);
			OpenPopup(SceneType.Main);
        }
        public void NextLevel()
        {
			audioService.PlaySound(SoundType.Button);
			OpenPopup(SceneType.NextLevel);
        }
		public void Retry()
		{
			audioService.PlaySound(SoundType.Button);
			OpenPopup(SceneType.NextLevel);
		}
		public void ShowWinPopup()
        {
            Pause();
            winPopup.Initialized(BackToHome, NextLevel);
            OpenPopup(SceneType.WinPopup);
        }
		public void ShowLosePopup()
		{
			Pause();
			losePopup.Initialized(BackToHome, Retry);
			OpenPopup(SceneType.LosePopup);
		}
		public void ShowPausePopup()
        {
            Pause();
            pausePopup.Initialized(Continue, BackToHome);
            OpenPopup(SceneType.Pause);
        }
        public void ShowBoosterPopup(System.Action onBuy, System.Action onAds, BoosterInfo info)
        {
            Pause();
            boosterPopup.Initialized(Continue, onBuy , onAds, info);
            OpenPopup(SceneType.Booster);
        }
    }
}
