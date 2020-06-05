using ModestTree;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [InjectOptional] private Player injectedPlayer;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private CameraTracker camTracker;    
    [SerializeField] private Joystick joystick;
    [Inject(Id = "Player")] private UnitData playerEntity;
    [Inject(Id = "Fly Enemy")] private UnitData flyEntity;
    [Inject(Id = "Ground Enemy")] private UnitData groundEntity;
    [Inject(Id = "Boss Enemy")] private UnitData bossEntity;
    [Inject] private ArenaData arenaData;
    [Inject] private GameState gameState;
    [Header("Prefabs")]
    [SerializeField] private Player playerPrefab;
    [SerializeField] private FlyEnemy flyEnemyPrefab;
    [SerializeField] private GroundEnemy groundEnemyPrefab;
    [SerializeField] private BossEnemy bossEnemyPrefab;
    [HideInInspector] public List<AbstractUnit> AliveEnemies = new List<AbstractUnit>();
    private Player cashedPlayer;
    private void Start()
    {
        InstantiateEntities();
    }

    private void InstantiateEntities()
    {
        if (injectedPlayer != null)
        {
            var player = Instantiate(injectedPlayer, arenaData.PlayerSpawnPos, Quaternion.identity);
            player.HealthbarCanvas = camTracker.PriorCamera;
            ExtraSetup(player);
        }
        else
        {
            var player = Instantiate(playerPrefab, arenaData.PlayerSpawnPos, Quaternion.identity);
            player.SetDefaultConfig(playerEntity, camTracker.PriorCamera);
            ExtraSetup(player);
        }

        if (arenaData.FlySpawnPoints.Count != 0)
        {
            for (int i = 0; i < arenaData.FlySpawnPoints.Count; i++)
            {
                var entity = Instantiate(flyEnemyPrefab, arenaData.FlySpawnPoints[i], Quaternion.identity);
                AliveEnemies.Add(entity);
                entity.SetDefaultConfig(flyEntity, camTracker.PriorCamera, cashedPlayer);
            }
        }

        if (arenaData.GroundSpawnPoints.Count != 0)
        {
            for (int i = 0; i < arenaData.GroundSpawnPoints.Count; i++)
            {
                var entity = Instantiate(groundEnemyPrefab, arenaData.GroundSpawnPoints[i], Quaternion.identity);
                AliveEnemies.Add(entity);
                entity.SetDefaultConfig(groundEntity, camTracker.PriorCamera, cashedPlayer);
            }
        }

        if (arenaData.BossSpawnPos != Vector2.zero)
        {
            var entity = Instantiate(bossEnemyPrefab, arenaData.BossSpawnPos, Quaternion.identity);
            AliveEnemies.Add(entity);
            entity.SetDefaultConfig(bossEntity, camTracker.PriorCamera, cashedPlayer);
        }
    }

    private void ExtraSetup(Player player) 
    {
        camTracker.SetTarget(player.transform);
        joystick.player = player;
        joystick.gameObject.SetActive(true);
        cashedPlayer = player;
    }

    public void RemoveEnemy(AbstractUnit abstractUnit)
    {
        AliveEnemies.Remove(abstractUnit);
        if (AliveEnemies.IsEmpty())
        {
            gameState.OnPlayerWinLvl?.Invoke();
            if (SceneManager.GetActiveScene().buildIndex + 1 != SceneManager.sceneCount)
            {
                StartCoroutine(levelLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, cashedPlayer));
            }
        }
    }
}