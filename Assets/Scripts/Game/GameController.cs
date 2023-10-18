using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;
using Extensions;

using Services;
using Parameters;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private TileConfig tileConfig;
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private BoosterConfig boosterConfig;
        [Header("UI")]
        [SerializeField] private GameView view;
        [Header("Preferences")]
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private Transform hat;
        [SerializeField] private Transform tileHolder;
        [SerializeField] private List<Transform> slots;

        [SerializeField] private Vector3 hatSpawnPos = new Vector3(0f, 18f, 0f);
        [SerializeField] private Vector3 hatDesPos = new Vector3(10f, 18f, 10f);

        private GameServices gameServices;
        private PlayerService playerService;
        private AudioService audioService;
        private AdsService adsService;
        private InputService inputService;

		private bool isWin = false;
        private bool isMoving = false;
        private bool isCloseUI = false;
        private int level;
        private int star;
        private int combo;
        private Tile selectTile;
        private Dictionary<TileType, TileInfo> tileDic;
        private Dictionary<int, Tile> slotDic;
        private Dictionary<TileType, List<Tile>> allTiles;
        private List<Tile> tileStepList;

        private void Awake()
        {
            //Check null
            gameConfig.ThrowIfNull();
            tileConfig.ThrowIfNull();
            levelConfig.ThrowIfNull();
            view.ThrowIfNull();
            tilePrefab.ThrowIfNull();
            tileHolder.ThrowIfNull();
            //Receive services and params
            GameObject go = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
            if (go != null)
            {
                gameServices = go.GetComponent<GameServices>();
                playerService = gameServices.GetService<PlayerService>();
                audioService = gameServices.GetService<AudioService>();
                adsService = gameServices.GetService<AdsService>();
                inputService = gameServices.GetService<InputService>();
            }
            else
            {
                SceneManager.LoadSceneAsync(Constants.EntryScene);
                return;
            }
            go = GameObject.FindGameObjectWithTag(Constants.ParamsTag);
            if (go != null)
            {
                PopUpParameter popUpParameter = go.GetComponent<PopUpParameter>();
                Destroy(go);
            }

            slotDic = new();
            tileStepList = new();
            allTiles = new();
            tileDic = new();
            foreach (var item in tileConfig.TileInfos)
            {
                tileDic.Add(item.Type,item);
            }

            for (int i = 0; i < slots.Count; i++)
            {
                slotDic.Add(i, null);
            }

            adsService.RequestAndLoadInterstitialAd();
            adsService.RequestAndLoadRewardedAd();
            level = playerService.GetLevel();

            LevelInfo info = levelConfig.Levels[level];
            view.SetServices(playerService,audioService);
            view.Init(info , Continue , () => isCloseUI = true);
            view.StartCountDown();
        }
        void Start()
        {
            StartCoroutine(GenerateLevel(level));
        }
        void Update()
        {
            if (isMoving || isWin) return;
            InputControl();
        }
        private void InputControl()
        {
            if (inputService.IsMoveOnUI() || isCloseUI)
            {
                isCloseUI = false;
                return;
            }
            if (isCloseUI)
            {
                isCloseUI = false;
                return;
            }
            Services.TouchPhase phase = inputService.GetTouchPhase();
            if (phase == Services.TouchPhase.None) return;
            Ray ray = Camera.main.ScreenPointToRay(inputService.GetTouchPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (raycastHit.transform.CompareTag("Tile"))
                {
                    if (phase == Services.TouchPhase.Down || phase == Services.TouchPhase.Move)
                    {
                        Tile temp = raycastHit.transform.GetComponent<Tile>();
                        if (selectTile != temp)
                        {
                            if (selectTile != null) selectTile.Select(false);
                            selectTile = temp;
                            selectTile.Select(true);
                        }
                    }
                    else if (phase == Services.TouchPhase.Up)
                    {
                        selectTile?.Select(false);
                        selectTile = null;

                        Tile tile = raycastHit.transform.GetComponent<Tile>();
                        StartCoroutine(CheckAndMoveToSlot(tile));
                    }
                }
            }
        }
        private void Continue(bool isContinue)
        {
            this.isMoving = !isContinue;
        }
        private IEnumerator CheckAndMoveToSlot(Tile tile)
        {
            audioService.PlaySound(SoundType.Collect);
            isMoving = true;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotDic[i] == null)
                {
                    slotDic[i] = tile;
                    tile.MoveToSlot(slots[i]);
                    tileStepList.Add(tile);
                    allTiles[tile.Type].Remove(tile);
                    SortTheSlot();
                    break;
                }
                else if (slotDic[i].Type == tile.Type)
                {
                    Tile temp1 = slotDic[i];
                    for (int j = i; j < slots.Count - 1; j++)
                    {
                        Tile temp2 = slotDic[j + 1];
                        slotDic[j + 1] = temp1;
                        temp1 = temp2;
                        if (temp1 == null)
                        {
                            break;
                        }
                    }
                    slotDic[i] = tile;
                    tile.MoveToSlot(slots[i]);
                    tileStepList.Add(tile);
                    allTiles[tile.Type].Remove(tile);
                    SortTheSlot();
                    break;
                }
            }
            if (allTiles[tile.Type].Count == 0)
            {
                allTiles.Remove(tile.Type);
            }
            yield return new WaitForSeconds(tileConfig.MoveToSlotTime);
            CheckMatch();
        }

        private IEnumerator GenerateLevel(int level  = 0)
        {
            hat.transform.DOMove(hatSpawnPos , gameConfig.HatFlyTime);
            yield return new WaitForSeconds(gameConfig.HatFlyTime);
            List<TargetDetail> targets = levelConfig.Levels[level].Details;
            List<TileType> tileTypes = new();
            for (int i = 0; i < targets.Count; i++)
            {
                for (int j = 0; j < targets[i].MatchCount * 3; j++)
                {
                    tileTypes.Add(targets[i].Type);
                }
            }

            tileTypes = tileTypes.OrderBy(t => UnityEngine.Random.Range(0f,100f)).ToList();
            for (int i = tileTypes.Count - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(gameConfig.SpawnTime);
                SpawnTile(tileTypes[i], i / (tileTypes.Count * gameConfig.Ratio) * 360f, i / (float)levelConfig.Levels[level].Total * (gameConfig.MaxForce - gameConfig.MinForce) + gameConfig.MinForce);
            }
            hat.transform.DOMove(hatDesPos, gameConfig.HatFlyTime);
        }
        private Tile SpawnTile(TileType type , float rotate , float force)
        {
            Vector3 randomPos = new Vector3(0f, 5f, 0f);
            GameObject go = SimplePool.Spawn(tilePrefab.gameObject, randomPos, Quaternion.identity);
            Rigidbody rb = go.GetComponent<Rigidbody>();
            Tile tile = go.GetComponent<Tile>();
            tile.Init(type,tileDic[type].Sprite , tileConfig);
            if (!allTiles.ContainsKey(tile.Type))
            {
                allTiles[tile.Type] = new();
            }
            allTiles[tile.Type].Add(tile);
            float xcomponent = Mathf.Cos(rotate * Mathf.PI / 180) * force;
            float ycomponent = Mathf.Sin(rotate * Mathf.PI / 180) * force;
            rb.AddForce(new Vector3(xcomponent, 0f, ycomponent) , ForceMode.Impulse);
            return tile;
        }
        private void SortTheSlot()
        {
            int moveCount = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotDic[i] == null)
                {
                    moveCount++;
                }
                else
                {
                    slotDic[i].MoveToSlot(slots[i - moveCount]);
                    if (moveCount > 0)
                    {
                        slotDic[i - moveCount] = slotDic[i];
                        slotDic[i] = null;
                    }
                }
            }
        }
        private void CheckMatch()
        {
            if (slotDic[0] == null)
            {
                return;
            }
            //Find the match
            TileType type = slotDic[0].Type;
            List<int> match = new();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotDic[i] == null)
                {
                    isMoving = false;
                    break;
                }
                else if (slotDic[i].Type == type)
                {
                    match.Add(i);
                    if (match.Count >= 3)
                    {
                        ClearMatch(match);
                        match.Clear();
                        isMoving = false;
                        break;
                    }
                    else if (i == slots.Count - 1)
                    {
                        ShowResult(false);
                    }
                }
                else if (i == slots.Count - 1)
                {
                    ShowResult(false);
                }
                else
                {
                    type = slotDic[i].Type;
                    match.Clear();
                    match.Add(i);
                }
            }
        }
        private void ClearMatch(List<int> match)
        {
            audioService.PlaySound(SoundType.Match);
            foreach (var item in match)
            {
                SimplePool.Despawn(slotDic[item].gameObject);
                tileStepList.Remove(slotDic[item]);
                slotDic[item] = null;
            }
            combo++;
            star += 3 * combo;
            view.ShowStar(star);
            view.ShowCombo(combo, gameConfig.ComboTime, () =>combo = 0);
            SortTheSlot();
            if (allTiles.Count == 0)
            {
                ShowResult(true);
            }
        }
        private void ShowResult(bool win)
        {
            if (win)
            {
                if(level < levelConfig.Levels.Count) playerService.SetLevel(level + 1);
                playerService.AddStar(star);
                view.ShowWinPopup();
            }
            else view.ShowLosePopup();
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            return new(UnityEngine.Random.Range(-5.5f, 5.5f), UnityEngine.Random.Range(3f, 5f), UnityEngine.Random.Range(-5.5f, 8f));
        }
        
        private void ReleaseSlot(int index)
        {
            isMoving = false;
            Vector3 spawnPos = GetRandomSpawnPosition();
            slotDic[index].ReleaseFromSlot(spawnPos);
            tileStepList.Remove(slotDic[index]);
            allTiles[slotDic[index].Type].Add(slotDic[index]);
            slotDic[index] = null;
        }
        #region BOOSTER
        public bool CheckBoosterAmount(BoosterType type)
        {
            if (playerService.GetBoosterAmount(type) > 0)
            {
                audioService.PlaySound(SoundType.Booster);
                return true;
            }
            BoosterInfo info = boosterConfig.BoosterInfos.Find(b => b.Type == type);
            view.ShowBoosterPopup(() => OnBuyItem(info), () => OnAdsBuyItem(info), info);
            return false;
        }
        public void OnBuyItem(BoosterInfo info)
        {
            audioService.PlaySound(SoundType.Button);
            if (playerService.GetCoin() > info.Price)
            {
                audioService.PlaySound(SoundType.GetBooster);
                playerService.AddCoin(-info.Price);
                playerService.SetBoosterAmount(info.Type, playerService.GetBoosterAmount(info.Type) + 1);
                view.UpdateInfo();
                view.Continue();
            }
            else
            {
                //TODO : OUT OF MONEY
            }
        }
        public void OnAdsBuyItem(BoosterInfo info)
        {
            audioService.PlaySound(SoundType.Button);
            if (adsService.IsRewardReady())
            {
                adsService.ShowRewardedAd(isComplete =>
                {
                    audioService.PlaySound(SoundType.GetBooster);
                    playerService.SetBoosterAmount(info.Type, playerService.GetBoosterAmount(info.Type) + 1);
                    view.UpdateInfo();
                    view.Continue();
                });
            }
        }
        public void UseItem(BoosterType type)
        {
            playerService.SetBoosterAmount(type, playerService.GetBoosterAmount(type) - 1);
            view.UpdateInfo();
        }
        public void MagnetBooster()
        {
            if (!CheckBoosterAmount(BoosterType.Magnet)) return;
            Dictionary<TileType, int> cache = new();
            int emptySlot = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                if (slotDic[i] != null)
                {
                    if (cache.ContainsKey(slotDic[i].Type))
                    {
                        StartCoroutine(CheckAndMoveToSlot(allTiles[slotDic[i].Type][0]));
                        UseItem(BoosterType.Magnet);
                        return;
                    }
                    else
                    {
                        cache.Add(slotDic[i].Type, 1);
                    }
                }
                else
                {
                    emptySlot++;
                }
            }

            if (cache.Keys.Count == slotDic.Keys.Count - 1) 
            {
                return;
            }
            else if(cache.Keys.Count > 0)
            {
                var randomKey = cache.Keys.ElementAt(UnityEngine.Random.Range(0, cache.Keys.Count));
                if (allTiles[randomKey].Count >= 3)
                {
                    for (int i = 1; i >= 0; i--)
                    {
                        StartCoroutine(CheckAndMoveToSlot(allTiles[randomKey][i]));
                    }
                }
            }
            else
            {
                var randomKey = allTiles.Keys.ElementAt(UnityEngine.Random.Range(0, allTiles.Keys.Count));
                for (int i = 2; i >= 0; i--)
                {
                    StartCoroutine(CheckAndMoveToSlot(allTiles[randomKey][i]));
                }
            }
            UseItem(BoosterType.Magnet);
        }
        public void BackBooster()
        {
            if (!CheckBoosterAmount(BoosterType.Back)) return;
            if (tileStepList.Count <= 0)
            {
                //TODO : UNUSED
                return;
            }
            int count = boosterConfig.BackSlotBoosterAmount;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotDic[i] == null)
                {
                    break;
                }
                else if (slotDic[i].gameObject == tileStepList[tileStepList.Count - 1].gameObject)
                {
                    ReleaseSlot(i);
                    SortTheSlot();
                    i = 0;
                    count--;
                    if (count == 0) break;
                }
            }
            UseItem(BoosterType.Back);
        }
        public void FanBooster()
        {
            if (!CheckBoosterAmount(BoosterType.Fan)) return;
            foreach (var key in allTiles.Keys)
            {
                foreach (var tile in allTiles[key])
                {
                    tile.AddWind();
                }
            }
            UseItem(BoosterType.Fan);
        }
        public void FreezeBooster()
        {
            if (!CheckBoosterAmount(BoosterType.Freeze)) return;
            view.FreezePause();
            StartCoroutine(WaitAction(boosterConfig.FreezeTime, view.Continue));
            UseItem(BoosterType.Freeze);
        }
        private IEnumerator WaitAction(int time, System.Action action = null)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
        #endregion
    }
}
