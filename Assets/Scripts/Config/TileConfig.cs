using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileInfo
{
    [SerializeField] private TileType type;
    [SerializeField] private Sprite sprite;

    public TileType Type { get => type; set => type = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
}
[CreateAssetMenu(fileName = "TileConfig", menuName = "Config/TileConfig", order = 3)]
public class TileConfig : ScriptableObject
{
    [SerializeField] private float moveToSlotTime = 0.5f;
    [SerializeField] private float scaleInSlot = 0.5f;
    [SerializeField] private float windForce = 7f;
    [SerializeField] private float windRotation = 2f;
    [SerializeField] private float lockTime = 0.1f;
    [SerializeField] private float rotateSpeed = 3f;
    [SerializeField] private List<TileInfo> tiles;

    public List<TileInfo> TileInfos { get => tiles; set => tiles = value; }
    public float MoveToSlotTime { get => moveToSlotTime; set => moveToSlotTime = value; }
    public float ScaleInSlot { get => scaleInSlot; set => scaleInSlot = value; }
    public float WindForce { get => windForce; set => windForce = value; }
    public float WindRotation { get => windRotation; set => windRotation = value; }
    public float LockTime { get => lockTime; set => lockTime = value; }
    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }
}
