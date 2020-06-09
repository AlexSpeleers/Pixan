using UnityEngine.UI;
using UnityEngine;
public abstract class AbstractUnit : MonoBehaviour
{
    protected GameState gameState;
    protected Spawner spawner;
    [SerializeField] protected Canvas healthbarCanvas;
    [SerializeField] protected Image healthBar;
    protected float maxHealth;
    protected float currentHealth;
    protected float speed;
    protected WaitForSeconds waitOneSecond;
    public abstract void TakeDamage(int damage);
    public abstract void SetDefaultConfig(UnitData unitData, Camera camera, Player target = null);    
}