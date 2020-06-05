using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName ="Arena Data №", menuName = "Create arena")]
public class ArenaData : ScriptableObject
{
    [Tooltip("Amount of flying enemies")] [SerializeField] private List<Vector2> flySpawnPoints;
    [Tooltip("Amount of ground enemies")] [SerializeField] private List<Vector2> groundSpawnPoints;
    [Tooltip("Fill this field only for level where boss is required.")]
    [SerializeField] private Vector2 bossSpawnPos;
    [SerializeField] private Vector2 playerSpawnPos;
    public List<Vector2> FlySpawnPoints => flySpawnPoints;
    public List<Vector2> GroundSpawnPoints => groundSpawnPoints;
    public Vector2 BossSpawnPos => bossSpawnPos;
    public Vector2 PlayerSpawnPos => playerSpawnPos;
}