using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [InjectOptional] private Player player;
    [SerializeField] private BulletBehaviour bullet;
    [SerializeField] private Spawner spawner;
    [SerializeField] private GameState gameState;

    public override void InstallBindings()
    {        
        Container.Bind<Spawner>().FromInstance(spawner).AsSingle();
        Container.Bind<GameState>().FromInstance(gameState).AsSingle();
        Container.Bind<BulletBehaviour>().FromInstance(bullet);
        if (player != null)
            Container.BindInstance(player);        
    }    
}