using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
	public class PlayerService
	{
		/// <summary>
		/// All keys for save data in PlayerPrefs
		/// </summary>
		private const string MusicVolumeKey = "mvl";
		private const string SoundVolumeKey = "svl";
		private const string VibrateKey = "vbr";

		private const string currentLevelKey = "clk";
		private const string coinKey = "cnk";
		private const string starKey = "srk";
		private const string ownedItemsKey = "oik";

		private const string Break = "~";
		private Dictionary<BoosterType, int> ownedItems = new Dictionary<BoosterType, int>();

		/// <summary>
		/// Action for catch event when music volume change
		/// </summary>
		public Action<float> OnMusicVolumeChange;
		/// <summary>
		/// Action for catch event when sound volume change
		/// </summary>
		public Action<float> OnSoundVolumeChange;
		/// <summary>
		/// Action for catch event when vibrate change
		/// </summary>
		public Action<bool> OnVibrateChange;
		/// <summary>
		/// Get the music volume of game
		/// </summary>
		/// <returns>Music volume</returns>
		public float GetMusicVolume()
		{
			return PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
		}
		/// <summary>
		/// Set the music volume of game
		/// </summary>
		/// <param name="volume"></param>
		public void SetMusicVolume(float volume)
		{
			PlayerPrefs.SetFloat(MusicVolumeKey, volume);
			OnMusicVolumeChange?.Invoke(volume);
		}
		/// <summary>
		/// Get the sound volume of game
		/// </summary>
		/// <returns>Sound volume</returns>
		public float GetSoundVolume()
		{
			return PlayerPrefs.GetFloat(SoundVolumeKey, 1.0f);
		}
		/// <summary>
		/// Set the sound volume of game
		/// </summary>
		/// <param name="volume"></param>
		public void SetSoundVolume(float volume)
		{
			PlayerPrefs.SetFloat(SoundVolumeKey, volume);
			OnSoundVolumeChange?.Invoke(volume);
		}
		/// <summary>
		/// Get vibrate of game
		/// </summary>
		/// <returns>is vibrate or not</returns>
		public bool GetVibrate()
		{
			return PlayerPrefs.GetInt(VibrateKey, 1) == 0 ? false : true;
		}
		/// <summary>
		/// Set vibrate of game
		/// </summary>
		/// <param name="isVibrate"></param>
		public void SetVibrate(bool isVibrate)
		{
			OnVibrateChange?.Invoke(isVibrate);
			if (isVibrate == true)
			{
				PlayerPrefs.SetInt(VibrateKey, 1);
			}
			else
			{
				PlayerPrefs.SetInt(VibrateKey, 0);
			}
		}
		/// <summary>
		/// Save a list of value to PlayerPrefs
		/// </summary>
		/// <typeparam name="T">type of value</typeparam>
		/// <param name="key">name key of list value</param>
		/// <param name="value">list of value that need to save</param>
		/// <exception cref="Exception"></exception>
		private void SaveList<T>(string key, List<T> value)
		{
			if (value == null)
			{
				Logger.Warning("Input list null");
				value = new List<T>();
			}
			if (value.Count == 0)
			{
				PlayerPrefs.SetString(key, string.Empty);
				return;
			}
			if (typeof(T) == typeof(string))
			{
				foreach (var item in value)
				{
					string tempCompare = item.ToString();
					if (tempCompare.Contains(Break))
					{
						throw new Exception("Invalid input. Input contain '~'.");
					}
				}
			}
			PlayerPrefs.SetString(key, string.Join(Break, value));
		}
		/// <summary>
		/// Get list of value that saved
		/// </summary>
		/// <typeparam name="T">type of value</typeparam>
		/// <param name="key">name key of list value</param>
		/// <param name="defaultValue">default value if playerprefs doesn't have value</param>
		/// <returns></returns>
		private List<T> GetList<T>(string key, List<T> defaultValue)
		{
			if (PlayerPrefs.HasKey(key) == false)
			{
				return defaultValue;
			}
			if (PlayerPrefs.GetString(key) == string.Empty)
			{
				return new List<T>();
			}
			string temp = PlayerPrefs.GetString(key);
			string[] listTemp = temp.Split(Break);
			List<T> list = new List<T>();

			foreach (string s in listTemp)
			{
				list.Add((T)Convert.ChangeType(s, typeof(T)));
			}
			return list;
		}
		public void SetLevel(int level)
		{
			PlayerPrefs.SetInt(currentLevelKey, level);
		}
		public int GetLevel()
		{
			return PlayerPrefs.GetInt(currentLevelKey, 0);
		}
		public void SetCoin(int coin)
		{
			PlayerPrefs.SetInt(coinKey, coin);
		}
		public void AddCoin(int coin)
		{
			PlayerPrefs.SetInt(coinKey, GetCoin() + coin);
		}
		public int GetCoin()
		{
			return PlayerPrefs.GetInt(coinKey, 0);
		}
		public void AddStar(int star)
		{
			PlayerPrefs.SetInt(starKey, GetCoin() + star);
		}
		public int GetStar()
		{
			return PlayerPrefs.GetInt(starKey, 0);
		}
		public int GetBoosterAmount(BoosterType itemType)
		{
			if (ownedItems.ContainsKey(itemType)) return ownedItems[itemType];
			int qty = PlayerPrefs.GetInt(ownedItemsKey + (int)itemType, 0);
			ownedItems.Add(itemType, qty);
			return qty;
		}
		public void SetBoosterAmount(BoosterType itemType, int quantity = 0)
		{
			PlayerPrefs.SetInt(ownedItemsKey + (int)itemType, quantity);
			if (ownedItems.ContainsKey(itemType)) ownedItems[itemType] = quantity;
			else ownedItems.Add(itemType, quantity);
		}
		public void Save()
		{
			PlayerPrefs.Save();
		}
	}
}
