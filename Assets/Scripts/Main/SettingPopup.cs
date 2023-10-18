using UnityEngine;
using UnityEngine.UI;

using Services;
using Extensions;
using UnityEngine.SceneManagement;

public class SettingPopup : MonoBehaviour
{
	[SerializeField] private Slider music;
	[SerializeField] private Slider sound;

	private GameServices gameServices;
	private GameService gameService;
	private PlayerService playerService;
	private AudioService audioService;

	private System.Action OnClose;
	private void Awake()
	{
		music.ThrowIfNull();
		sound.ThrowIfNull();

		if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) != null)
		{
			gameServices = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
			playerService = gameServices.GetService<PlayerService>();
			gameService = gameServices.GetService<GameService>();
			audioService = gameServices.GetService<AudioService>();
		}
		else
		{
			SceneManager.LoadScene(Constants.EntryScene);
			return;
		}

		music.value = playerService.GetMusicVolume();
		sound.value = playerService.GetSoundVolume();
	}
	public void Initialized(System.Action OnClose)
	{
		this.OnClose = OnClose;
	}
	public void ChangeMusic()
	{
        audioService.PlaySound(SoundType.Button);

		playerService.SetMusicVolume(music.value);
	}
	public void ChangeSound()
	{
        audioService.PlaySound(SoundType.Button);
		playerService.SetSoundVolume(sound.value);
	}
	public void TermsButton()
	{
		gameService.TOS();
	}
	public void PrivacyButton()
	{
		gameService.Privacy();
	}
	public void CloseButton()
	{
        audioService.PlaySound(SoundType.Button);
		OnClose?.Invoke();
	}
}
