using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TargetDetail
{
    [SerializeField] private TileType type;
    [SerializeField] private int matchCount;
    public TileType Type { get => type; set => type = value; }
    public int MatchCount { get => matchCount; set => matchCount = value; }
}
[System.Serializable]
public class LevelInfo
{
    [SerializeField] private string name;
    [SerializeField] private string displayName;
    [SerializeField] private int level;
    [SerializeField] private int playTime;
    [SerializeField] private List<TargetDetail> details;

    public string Name { get => name; set => name = value; }
    public string DisplayName { get => displayName; set => displayName = value; }
    public int Level { get => level; set => level = value; }
    public int PlayTime { get => playTime; set => playTime = value; }
    public int Total { get => details.Sum(d => d.MatchCount * 3);}
    public List<TargetDetail> Details { get => details; set => details = value; }
}
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Config/LevelConfig", order = 2)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<LevelInfo> levels;

    public List<LevelInfo> Levels { get => levels; set => levels = value; }
}
