using System.Collections.Generic;
using UnityEngine;
using System;
using Audio;

namespace Services
{
	public class AudioService
	{
		/// <summary>
		/// Action sound, sound volume change.
		/// </summary>
		public event Action<bool> OnSoundChanged;
		public event Action<float> OnSoundVolumeChanged;
		/// <summary>
		/// Action music, music volume change.
		/// </summary>
		public event Action<bool> OnMusicChanged;
		public event Action<float> OnMusicVolumeChanged;

		private bool soundOn;
		private bool musicOn;
		private float soundVolume;
		private float musicVolume;

		private bool vibrateOn;

		private Music music;
		private Dictionary<string, AudioSource> soundAudioSources;

		// Cache
		private Dictionary<string, float> soundVolumes = new Dictionary<string, float>();
		/// <summary>
		/// Initiate audio service.
		/// </summary>
		/// <param name="music">class music from entry scene</param>
		/// <param name="sounds">list sound from entry scene.</param>
		/// <param name="soundObject">object that attached audio source</param>
		public AudioService(Music music, List<Sound> sounds, GameObject soundObject)
		{
			this.music = music;
			this.music.Initialized(this);

			soundAudioSources = new Dictionary<string, AudioSource>();
			foreach (var sound in sounds)
			{
				AudioSource soundSource = soundObject.AddComponent<AudioSource>();
				soundSource.clip = sound.AudioClip;
				soundSource.volume = sound.Volume;
				soundSource.playOnAwake = false;
				soundAudioSources.Add(sound.Name, soundSource);
			}

			foreach (var audioSource in soundAudioSources)
			{
				soundVolumes.Add(audioSource.Key, audioSource.Value.volume);
			}
		}
		// Play Sound
		public void PlaySound(string keySound)
		{
			if (soundOn == true && soundVolume > 0.0f)
			{
				var audioSouce = soundAudioSources[keySound];
				audioSouce.volume = soundVolume * soundVolumes[keySound];
				audioSouce.Play();
			}
		}
		public bool IsSoundPlaying(SoundType soundType)
		{
			if (soundOn == true && soundVolume > 0.0f)
			{
				var audioSouce = soundAudioSources[soundType.ToString()];
				audioSouce.volume = soundVolume * soundVolumes[soundType.ToString()];
				return audioSouce.isPlaying;
			}
			return false;
		}
		public void PlayButton()
		{
			PlaySound(SoundType.Button);
		}
		public void PlaySound(SoundType soundType)
		{
			if (soundOn == true && soundVolume > 0.0f)
			{
				var audioSouce = soundAudioSources[soundType.ToString()];
				audioSouce.volume = soundVolume * soundVolumes[soundType.ToString()];
				audioSouce.Play();
			}
		}
		// Play Music
		public void PlayMusic()
		{
			if (musicOn == true && musicVolume > 0.0f)
			{
				music.PlayMusic("music");
			}
		}
		// Fade Music
		public void FadeMusic(float time)
		{
			if (musicOn == true && musicVolume > 0.0f)
			{
				music.FadeMusic("music", time);
			}
		}
		// End
		/// <summary>
		/// Stop all sound.
		/// </summary>
		public void StopAllSound()
		{
			foreach (var audioSource in soundAudioSources)
			{
				audioSource.Value.Stop();
			}
		}
		/// <summary>
		/// Stop music with name.
		/// </summary>
		public void StopMusic()
		{
			music.StopMusic("music");
		}
		/// <summary>
		/// Return true if music is playing.
		/// </summary>
		/// <returns></returns>
		public bool IsMusicPlaying()
		{
			return music.IsMusicPlaying("music");
		}
		/// <summary>
		/// Set volume of music.
		/// </summary>
		/// <param name="volume"></param>
		public void SetMusicVolume(float volume)
		{
			MusicVolume = volume;
		}
		/// <summary>
		/// Set volume of sound.
		/// </summary>
		/// <param name="volume"></param>
		public void SetSoundVolume(float volume)
		{
			SoundVolume = volume;
		}
		/// <summary>
		/// Set vibrate.
		/// </summary>
		/// <param name="isOn"></param>
		public void SetVibrate(bool isOn)
		{
			VibrateOn = isOn;
		}
		/// <summary>
		/// Play vibrate
		/// </summary>
		public void Vibrate()
		{
			if(vibrateOn == true)
			{
				Handheld.Vibrate();
			}
		}
		// GET - SET
		public float SoundVolume
		{
			get { return soundVolume; }
			set
			{
				soundVolume = value;
				foreach (var audioSource in soundAudioSources)
				{
					audioSource.Value.volume = soundVolume * soundVolumes[audioSource.Key];
				}
				OnSoundVolumeChanged?.Invoke(soundVolume);
			}
		}

		public float MusicVolume
		{
			get { return musicVolume; }
			set
			{
				musicVolume = value;
				OnMusicVolumeChanged?.Invoke(musicVolume);
			}
		}
		public bool SoundOn
		{
			get { return soundOn; }
			set
			{
				soundOn = value;
				if (SoundOn == false)
				{
					StopAllSound();
				}
				OnSoundChanged?.Invoke(soundOn);
			}
		}

		public bool MusicOn
		{
			get { return musicOn; }
			set
			{
				musicOn = value;
				if (musicOn == true && musicVolume > 0)
				{
					music.PlayMusic("music");
				}
				else
				{
					music.StopMusic("music");
				}
				OnMusicChanged?.Invoke(musicOn);
			}
		}
		public bool VibrateOn
		{
			get { return vibrateOn; }
			set
			{
				vibrateOn = value;
			}
		}
	}
}
