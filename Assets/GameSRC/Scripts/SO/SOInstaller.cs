using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SOInstaller", menuName = "Installers/SOInstaller")]
public class SOInstaller : ScriptableObjectInstaller<SOInstaller>
{
    [SerializeField] private ArenaData arenaData;
    [SerializeField] private UnitData playerData;
    [SerializeField] private UnitData groundEnemyData;
    [SerializeField] private UnitData flyEnemyData;
    [SerializeField] private UnitData bossData;
    public override void InstallBindings()
    {
        Container.BindInstance(arenaData);
        Container.BindInstance(playerData).WithId("Player");
        Container.BindInstance(groundEnemyData).WithId("Ground Enemy");
        Container.BindInstance(flyEnemyData).WithId("Fly Enemy");
        Container.BindInstance(bossData).WithId("Boss Enemy");
    }
}