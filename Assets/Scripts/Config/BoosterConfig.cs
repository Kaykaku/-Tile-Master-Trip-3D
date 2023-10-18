using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoosterInfo
{
    [SerializeField] private BoosterType type;
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private int price;
    [SerializeField] private int amount;
    [SerializeField] private Sprite sprite;

    public BoosterType Type { get => type; set => type = value; }
    public int Price { get => price; set => price = value; }
    public int Amount { get => amount; set => amount = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
}
[CreateAssetMenu(fileName = "BoosterConfig", menuName = "Config/BoosterConfig", order = 3)]
public class BoosterConfig : ScriptableObject
{
    [SerializeField] private int freezeTime = 20;
    [SerializeField] private int backSlotBoosterAmount = 3;

    [SerializeField] private List<BoosterInfo> boosterInfos;

    public List<BoosterInfo> BoosterInfos { get => boosterInfos; set => boosterInfos = value; }
    public int FreezeTime { get => freezeTime; set => freezeTime = value; }
    public int BackSlotBoosterAmount { get => backSlotBoosterAmount; set => backSlotBoosterAmount = value; }
}
