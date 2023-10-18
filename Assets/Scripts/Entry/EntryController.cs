using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Services;
using Audio;
using Utilities;

namespace Entry
{
	public class EntryController : MonoBehaviour
	{
		private const string soundObjectName = "Sound";
		private const string UnUsed = "unused";
		[SerializeField] private EntryModel model;

		[SerializeField] private List<Sound> sounds;
		[SerializeField] private Music music;
		[SerializeField] private GameObject musicObject;

		[Space(8.0f)]
		[SerializeField] private float loadingTime = 3f;

		private bool isReady = false;
		private GameServices gameServices = null;

		void Awake()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
                GameObject gameServiceObject = new(nameof(GameServices))
                {
                    tag = Constants.ServicesTag
                };
                gameServices = gameServiceObject.AddComponent<GameServices>();

				// Instantie Audio
				DontDestroyOnLoad(musicObject);

				GameObject soundObject = new(soundObjectName);
				DontDestroyOnLoad(soundObject);

				// Add Services
				gameServices.AddService(new AudioService(music, sounds, soundObject));
				gameServices.AddService(new DisplayService());
				gameServices.AddService(new InputService());
				gameServices.AddService(new PlayerService());
				gameServices.AddService(new GameService(model.TOSURL, model.PrivacyURL, model.RateURL));

				//var adsServices = gameServices.GetService<AdsService>();
				var audioService = gameServices.GetService<AudioService>();
				var playerService = gameServices.GetService<PlayerService>();

				// --------------------------- Audio ---------------------------------
				// Set Volume
				playerService.OnMusicVolumeChange = audioService.SetMusicVolume;
				playerService.OnSoundVolumeChange = audioService.SetSoundVolume;

				playerService.OnVibrateChange = audioService.SetVibrate;

				audioService.MusicVolume = playerService.GetMusicVolume();
				audioService.SoundVolume = playerService.GetSoundVolume();

				audioService.VibrateOn = playerService.GetVibrate();

				audioService.MusicOn = true;
				audioService.SoundOn = true;

				audioService.StopMusic();
				// ------------------------------------------------------------------
			}
		}
        private void Start()
        {
			Loading();
		}
		private void Loading()
		{
			StartCoroutine(Wait());
		}
		private IEnumerator Wait()
		{
			float timer = loadingTime * 0.1f;
			while (timer < loadingTime * 1.1f)
			{
				timer += Time.deltaTime;
				yield return null;
			}
			timer = 0;
			while (!isReady && timer < loadingTime)
			{
				yield return null;
				timer += Time.deltaTime;
			}
			SceneManager.LoadScene(Constants.MainScene);
		}
		private void OnFetchSuccess()
		{
			Time.timeScale = 1.0f;
			isReady = true;
		}
	}
}
