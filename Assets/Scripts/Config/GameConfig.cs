using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [SerializeField] private float maxForce = 15f;
    [SerializeField] private float minForce = 2f;
    [SerializeField] private float spawnTime = 0.05f;
    [SerializeField] private float hatFlyTime = 0.5f;
    [SerializeField] private float ratio = 0.3f;
    [SerializeField] private float comboTime = 5f;

    public float MaxForce { get => maxForce; set => maxForce = value; }
    public float MinForce { get => minForce; set => minForce = value; }
    public float SpawnTime { get => spawnTime; set => spawnTime = value; }
    public float HatFlyTime { get => hatFlyTime; set => hatFlyTime = value; }
    public float Ratio { get => ratio; set => ratio = value; }
    public float ComboTime { get => comboTime; set => comboTime = value; }
}
