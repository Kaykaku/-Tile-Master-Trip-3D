using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using Extensions;

using Services;

public class HomeController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI starTxt;
    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private SettingPopup settingPopup;

    private GameServices gameServices;
    private PlayerService playerService;
    private DisplayService displayService;
    private AudioService audioService;
    private AdsService adsService;
    private void Awake()
    {
        //Check null
        starTxt.ThrowIfNull();
        coinTxt.ThrowIfNull();
        levelTxt.ThrowIfNull();
        //Receive services and params
        GameObject go = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
        if (go != null)
        {
            gameServices = go.GetComponent<GameServices>();
            playerService = gameServices.GetService<PlayerService>();
            displayService = gameServices.GetService<DisplayService>();
            audioService = gameServices.GetService<AudioService>();
            adsService = gameServices.GetService<AdsService>();
        }
        else
        {
            SceneManager.LoadSceneAsync(Constants.EntryScene);
            return;
        }

        starTxt.text = playerService.GetStar().ToString();
        coinTxt.text = playerService.GetCoin().ToString();
        levelTxt.text = (playerService.GetLevel()+1).ToString();

        audioService.PlayMusic();
        settingPopup.Initialized(()=> settingPopup.gameObject.SetActive(false));
        adsService.RequestBannerAd(GoogleMobileAds.Api.AdSize.Banner,GoogleMobileAds.Api.AdPosition.Bottom);
    }
    public void AddCoinButton()
    {
        audioService.PlaySound(SoundType.Button);
        playerService.AddCoin(1000);
        coinTxt.text = playerService.GetCoin().ToString();
    }
    public void PlayButton()
    {
        audioService.PlaySound(SoundType.Button);
        adsService.DestroyBannerAd();
        SceneManager.LoadSceneAsync(Constants.GameScene);
    }
    public void SettingButton()
    {
        audioService.PlaySound(SoundType.Button);
        settingPopup.gameObject.SetActive(true);
    }
}
