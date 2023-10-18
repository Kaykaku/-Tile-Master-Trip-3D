using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class GameSettingWindow : EditorWindow
{
    private float space = 20f;
    private int tabs = 4;
    private string[] tabNames = new string[] {"GAMEPLAY", "BOOSTER", "TILE", "LEVEL" };
    private GameConfig gameConfig;
    private TileConfig tileConfig;
    private LevelConfig levelConfig;
    private BoosterConfig boosterConfig;

    [MenuItem("Tools/Game Settings")]
    public static void ShowWindow()
    {
        GameSettingWindow window = GetWindow<GameSettingWindow>("Game Settings");
        window.tabs = 0;
    }

    private void OnGUI()
    {
        tabs = GUILayout.Toolbar(tabs , tabNames);
        switch (tabs)
        {
            case 0:
                GameConfigTab();
                break;
            case 1:
                BoosterConfigTab();
                break;
            case 2:
                TileConfigTab();
                break;
            case 3:
                LevelConfig();
                break;
        }
    }
    private void OnEnable()
    {
        ResetGameConfig();
        ResetLevelConfig();
        ResetTileConfig();
        ResetBoosterConfig();
    }
    #region GAMEPLAY CONFIG TAB
    private float maxForce = 15f;
    private float minForce = 2f;
    private float spawnTime = 0.05f;
    private float hatFlyTime = 0.5f;
    private float ratio;
    private float comboTime;

    private int curLevel;
    private int coin;
    private void GameConfigTab()
    {
        gameConfig = (GameConfig) EditorGUILayout.ObjectField("Game Config", gameConfig, typeof(GameConfig), false);

        GUILayout.Space(space);
        GUILayout.Label("CONFIG", EditorStyles.boldLabel);
        minForce = EditorGUILayout.FloatField("Min Force", minForce);
        maxForce = EditorGUILayout.FloatField("Max Force", maxForce);
        spawnTime = EditorGUILayout.FloatField("Spawn Time", spawnTime);
        hatFlyTime = EditorGUILayout.FloatField("Hat Fly Time", hatFlyTime);
        ratio = EditorGUILayout.FloatField("Ratio", ratio);
        comboTime = EditorGUILayout.FloatField("Combo Time", comboTime);

        GUILayout.Label("PLAYERPREF", EditorStyles.boldLabel);
        curLevel = EditorGUILayout.IntField("CurLevel", curLevel);
        coin = EditorGUILayout.IntField("Coin", coin);

        GUILayout.Space(space);
        GUILayout.Label("OPTIONS", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset"))
        {
            ResetGameConfig();
        }
        if (GUILayout.Button("Apply Change"))
        {
            SaveGameConfig();
        }
    }
    private void ResetGameConfig()
    {
        gameConfig = Resources.Load<GameConfig>(Constants.GamePlayFloder);
        minForce = gameConfig.MinForce;
        maxForce = gameConfig.MaxForce;
        spawnTime = gameConfig.SpawnTime;
        ratio = gameConfig.Ratio;
        comboTime = gameConfig.ComboTime;
        hatFlyTime = gameConfig.HatFlyTime;
        curLevel = PlayerPrefs.GetInt(Constants.currentLevelKey,0);
        coin = PlayerPrefs.GetInt(Constants.coinKey,0);
    }
    private void SaveGameConfig()
    {
        gameConfig.MinForce = minForce;
        gameConfig.MaxForce = maxForce;
        gameConfig.SpawnTime = spawnTime;
        gameConfig.HatFlyTime = hatFlyTime;
        gameConfig.Ratio = ratio;
        gameConfig.ComboTime = comboTime;
        PlayerPrefs.SetInt(Constants.currentLevelKey, curLevel);
        PlayerPrefs.SetInt(Constants.coinKey, coin);
        EditorUtility.SetDirty(gameConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Save Gameplay Config");
    }
    #endregion

    #region BOOSTER CONFIG TAB
    private List<BoosterInfo> boosterInfos;
    private Vector2 scrollPosition2;

    [SerializeField] private int freezeTime = 20;
    [SerializeField] private int backSlotBoosterAmount = 3;
    private void BoosterConfigTab()
    {
        boosterConfig = (BoosterConfig)EditorGUILayout.ObjectField("Tile Config", boosterConfig, typeof(BoosterConfig), false);

        GUILayout.Space(space);
        GUILayout.Label("BOOSTER CONFIG", EditorStyles.boldLabel);

        freezeTime = EditorGUILayout.IntField("Freeze Time", freezeTime);
        backSlotBoosterAmount = EditorGUILayout.IntField("Roll Back Booster Amount", backSlotBoosterAmount);

        GUILayout.Label("BOOSTER DATA", EditorStyles.boldLabel);
        if (GUILayout.Button("NEW"))
        {
            boosterInfos.Add(new BoosterInfo());
        }
        scrollPosition2 = EditorGUILayout.BeginScrollView(scrollPosition2);
        int rows = boosterInfos.Count;

        GUILayout.BeginHorizontal();
        GUILayout.Label("ID", EditorStyles.boldLabel);
        GUILayout.Label("Name", EditorStyles.boldLabel);
        GUILayout.Label("Description", EditorStyles.boldLabel);
        GUILayout.Label("Type", EditorStyles.boldLabel);
        GUILayout.Label("Amount", EditorStyles.boldLabel);
        GUILayout.Label("Price", EditorStyles.boldLabel);
        GUILayout.Label("Sprite", EditorStyles.boldLabel);
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        for (int row = 0; row < rows; row++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(row.ToString(), EditorStyles.label);
            boosterInfos[row].Name = EditorGUILayout.TextField(boosterInfos[row].Name);
            boosterInfos[row].Description = EditorGUILayout.TextArea(boosterInfos[row].Description);
            boosterInfos[row].Type = (BoosterType)EditorGUILayout.EnumPopup(boosterInfos[row].Type);
            boosterInfos[row].Amount = EditorGUILayout.IntField(boosterInfos[row].Amount);
            boosterInfos[row].Price = EditorGUILayout.IntField(boosterInfos[row].Price);
            boosterInfos[row].Sprite = EditorGUILayout.ObjectField(boosterInfos[row].Sprite, typeof(Sprite), false) as Sprite;
            if (boosterInfos[row].Sprite != null)
            {
                Rect spriteRect = GUILayoutUtility.GetRect(50, 50, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(spriteRect, boosterInfos[row].Sprite.texture);
            }
            GUIStyle redButtonStyle = new GUIStyle(GUI.skin.button);
            redButtonStyle.normal.textColor = Color.white;
            redButtonStyle.hover.textColor = Color.white;
            redButtonStyle.active.textColor = Color.white;
            redButtonStyle.normal.background = MakeTex(1, 1, new Color(0.6f, 0f, 0f, 1f));
            if (GUILayout.Button("X", redButtonStyle))
            {
                boosterInfos.RemoveAt(row);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(space);
        GUILayout.Label("OPTIONS", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset"))
        {
            ResetBoosterConfig();
        }
        if (GUILayout.Button("Apply Change"))
        {
            SaveBoosterConfig();
        }
    }
    private void ResetBoosterConfig()
    {
        boosterConfig = Resources.Load<BoosterConfig>(Constants.BoosterFloder);
        boosterInfos = boosterConfig.BoosterInfos.ToList();
        freezeTime = boosterConfig.FreezeTime;
        backSlotBoosterAmount = boosterConfig.BackSlotBoosterAmount;
    }
    private void SaveBoosterConfig()
    {
        boosterConfig.BoosterInfos = boosterInfos;
        boosterConfig.FreezeTime = freezeTime;
        boosterConfig.BackSlotBoosterAmount = backSlotBoosterAmount;
        EditorUtility.SetDirty(boosterConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Save Booster Config");
    }
    #endregion

    #region TILE CONFIG TAB
    private List<TileInfo> tileInfos;
    private Vector2 scrollPosition;

    private float moveToSlotTime = 0.5f;
    private float scaleInSlot = 0.5f;
    private float windForce = 7f;
    private float windRotation = 2f;
    private float lockTime = 0.1f;
    private float rotateSpeed = 3f;
    private void TileConfigTab()
    {
        tileConfig = (TileConfig) EditorGUILayout.ObjectField("Tile Config", tileConfig, typeof(TileConfig), false);

        GUILayout.Space(space);
        GUILayout.Label("TILE CONFIG", EditorStyles.boldLabel);

        moveToSlotTime = EditorGUILayout.FloatField("Move To Slot Time", moveToSlotTime);
        scaleInSlot = EditorGUILayout.FloatField("Scale In Slot", scaleInSlot);
        windForce = EditorGUILayout.FloatField("Wind Force", windForce);
        windRotation = EditorGUILayout.FloatField("Wind Rotation", windRotation);
        lockTime = EditorGUILayout.FloatField("Lock Time", lockTime);
        rotateSpeed = EditorGUILayout.FloatField("Rotate Speed", rotateSpeed);

        GUILayout.Label("TILE DATA", EditorStyles.boldLabel);
        if (GUILayout.Button("NEW"))
        {
            tileInfos.Add(new TileInfo());
        }
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        int rows = tileInfos.Count;

        GUILayout.BeginHorizontal();
        GUILayout.Label("ID", EditorStyles.boldLabel);
        GUILayout.Label("Type", EditorStyles.boldLabel);
        GUILayout.Label("Sprite", EditorStyles.boldLabel);
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        for (int row = 0; row < rows; row++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(row.ToString(), EditorStyles.label);
            tileInfos[row].Type = (TileType) EditorGUILayout.EnumPopup(tileInfos[row].Type);
            tileInfos[row].Sprite = EditorGUILayout.ObjectField(tileInfos[row].Sprite, typeof(Sprite), false) as Sprite;
            if (tileInfos[row].Sprite != null)
            {
                Rect spriteRect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(spriteRect, tileInfos[row].Sprite.texture);
            }
            GUIStyle redButtonStyle = new GUIStyle(GUI.skin.button);
            redButtonStyle.normal.textColor = Color.white;
            redButtonStyle.hover.textColor = Color.white;
            redButtonStyle.active.textColor = Color.white;
            redButtonStyle.normal.background = MakeTex(1, 1, new Color(0.6f, 0f, 0f, 1f));
            if (GUILayout.Button("X", redButtonStyle))
            {
                tileInfos.RemoveAt(row);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(space);
        GUILayout.Label("OPTIONS", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset"))
        {
            ResetTileConfig();
        }
        if (GUILayout.Button("Apply Change"))
        {
            SaveTileConfig();
        }
    }
    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }

        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(pix);
        tex.Apply();

        return tex;
    }
    private void ResetTileConfig()
    {
        tileConfig = Resources.Load<TileConfig>(Constants.TileFloder);
        tileInfos = tileConfig.TileInfos.ToList();
        moveToSlotTime = tileConfig.MoveToSlotTime;
        scaleInSlot = tileConfig.ScaleInSlot;
        windForce = tileConfig.WindForce;
        windRotation = tileConfig.WindRotation;
        lockTime = tileConfig.LockTime;
        rotateSpeed = tileConfig.RotateSpeed;
    }
    private void SaveTileConfig()
    {
        tileConfig.TileInfos = tileInfos;
        tileConfig.MoveToSlotTime = moveToSlotTime;
        tileConfig.ScaleInSlot = scaleInSlot;
        tileConfig.WindForce = windForce ;
        tileConfig.WindRotation = windRotation;
        tileConfig.LockTime = lockTime;
        tileConfig.RotateSpeed = rotateSpeed;
        EditorUtility.SetDirty(tileConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Save Tile Config");
    }
    #endregion

    #region LEVEL CONFIG TAB
    private List<LevelInfo> levels;
    private int selectedTab;
    private int tabsPerRow = 5;
    private void LevelConfig()
    {
        levelConfig = (LevelConfig) EditorGUILayout.ObjectField("Level Config", levelConfig, typeof(LevelConfig), false);

        GUILayout.Space(space);
        GUILayout.Label("Select Level", EditorStyles.boldLabel);
        int selectedTabRow = selectedTab / tabsPerRow;

        for (int i = 0; i <= levels.Count; i++)
        {
            if (i % tabsPerRow == 0)
            {
                GUILayout.BeginHorizontal();
            }

            if (i < levels.Count) {
                GUIContent tabContent = new GUIContent("Level " + (i + 1));
                if (GUILayout.Toggle(i == selectedTab, tabContent, "Button"))
                {
                    selectedTab = i;
                }
            } 
            else
            {
                GUIContent tabContent = new GUIContent("+");
                if (GUILayout.Button("+"))
                {
                    LevelInfo info = new LevelInfo();
                    info.Details = new();
                    info.Name = string.Empty;
                    info.DisplayName = string.Empty;
                    levels.Add(info);
                }
            }
            if (i % tabsPerRow == tabsPerRow - 1 || i == levels.Count)
            {
                GUILayout.EndHorizontal();
            }
        }
        if(levels.Count > 0)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.blue;
            labelStyle.fontSize = 20;
            labelStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("Level Info " + (selectedTab + 1), labelStyle);
            GUILayout.Space(space);
            levels[selectedTab].Name = EditorGUILayout.TextField("Name", levels[selectedTab].Name);
            levels[selectedTab].DisplayName = EditorGUILayout.TextField("Display Name", levels[selectedTab].DisplayName);
            levels[selectedTab].Level = EditorGUILayout.IntField("Level", levels[selectedTab].Level);
            levels[selectedTab].PlayTime = EditorGUILayout.IntField("Play Time", levels[selectedTab].PlayTime);

            GUILayout.Space(space);
            GUILayout.Label("Details", EditorStyles.boldLabel);
            if (GUILayout.Button("NEW"))
            {
                levels[selectedTab].Details.Add(new TargetDetail());
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            int rows = levels[selectedTab].Details == null ? 0 : levels[selectedTab].Details.Count;

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID", EditorStyles.boldLabel);
            GUILayout.Label("Preview", EditorStyles.boldLabel);
            GUILayout.Label("Type", EditorStyles.boldLabel);
            GUILayout.Label("Match Count", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            for (int row = 0; row < rows; row++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(row.ToString(), EditorStyles.label);
                TileInfo info = tileInfos.Find(t => t.Type == levels[selectedTab].Details[row].Type);
                if (info != null)
                {
                    Rect spriteRect = GUILayoutUtility.GetRect(70, 70, GUILayout.ExpandWidth(false));
                    EditorGUI.DrawPreviewTexture(spriteRect, info.Sprite.texture);
                }
                levels[selectedTab].Details[row].Type = (TileType)EditorGUILayout.EnumPopup(levels[selectedTab].Details[row].Type);
                levels[selectedTab].Details[row].MatchCount = EditorGUILayout.IntField(levels[selectedTab].Details[row].MatchCount);
                GUIStyle redButtonStyle = new GUIStyle(GUI.skin.button);
                redButtonStyle.normal.textColor = Color.white;
                redButtonStyle.hover.textColor = Color.white;
                redButtonStyle.active.textColor = Color.white;
                redButtonStyle.normal.background = MakeTex(1, 1, new Color(0.6f, 0f, 0f, 1f));
                if (GUILayout.Button("X", redButtonStyle))
                {
                    levels[selectedTab].Details.RemoveAt(row);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            int total = levels[selectedTab].Details == null ? 0 : levels[selectedTab].Details.Sum(l => l.MatchCount * 3);
            labelStyle.normal.textColor = Color.yellow;
            EditorGUILayout.LabelField("TOTAL : " + total, labelStyle);
            if (GUILayout.Button("DELETE MAP " + (selectedTab + 1)))
            {
                levels.RemoveAt(selectedTab);
            }
        }
        
        GUILayout.Label("OPTIONS", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset"))
        {
            ResetLevelConfig();
        }
        if (GUILayout.Button("Apply Change"))
        {
            SaveLevelConfig();
        }
    }
    private void ResetLevelConfig()
    {
        levelConfig = Resources.Load<LevelConfig>(Constants.LevelFloder);
        levels = levelConfig.Levels.ToList();
    }
    private void SaveLevelConfig()
    {
        levelConfig.Levels = levels;
        EditorUtility.SetDirty(levelConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Save Level Config");
    }
    #endregion
}
#endif