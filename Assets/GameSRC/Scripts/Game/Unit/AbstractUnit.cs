using UnityEngine.UI;
using UnityEngine;
using Zenject;
public abstract class AbstractUnit : MonoBehaviour
{
     protected GameState gameState;
     protected Spawner spawner;
    [SerializeField] protected Canvas healthbarCanvas;
    [SerializeField] protected Image healthBar;
    protected int maxHealth;
    protected float speed;
    protected int currentHealth;
    protected WaitForSeconds waitOneSecond;
    public abstract void TakeDamage(int damage);
    public abstract void SetDefaultConfig(UnitData unitData, Camera camera, Player target = null);    
}