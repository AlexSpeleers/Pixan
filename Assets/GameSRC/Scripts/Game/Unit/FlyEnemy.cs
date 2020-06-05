using UnityEngine;
using System.Collections;
using Zenject;

public class FlyEnemy : AbstractUnit
{    
    [SerializeField] private Rigidbody2D flyBody;
    private BulletBehaviour bullet;
    private Player target;
    private int amoDamage;
    private float amoSpeed;
    [Inject]
    private void Init(GameState gameState, Spawner spawner, BulletBehaviour bullet)
    {
        this.gameState = gameState;
        this.spawner = spawner;
        this.bullet = bullet;
    }

    private void Start()
    {
        waitOneSecond = new WaitForSeconds(1f);
        gameState.OnGameLost.AddListener(this.LostGame);
        StartCoroutine(MoveInRandomDiroutine());
    }

    public override void SetDefaultConfig(UnitData flyEnemyData, Camera camera, Player target)
    {
        this.target = target;
        healthbarCanvas.worldCamera = camera;
        maxHealth = currentHealth = flyEnemyData.Health;
        speed = flyEnemyData.UnitSpeed;
        amoDamage = flyEnemyData.AmoDmg;
        amoSpeed = flyEnemyData.AmoSpeed;
    }

    IEnumerator MoveInRandomDiroutine() 
    {
        var x = Random.Range(-1f, 1f);
        var y = Random.Range(-1f, 1f);
        var direction = new Vector2(x, y).normalized * speed;
        flyBody.AddForce(direction, ForceMode2D.Impulse);

        yield return waitOneSecond;
        flyBody.velocity = Vector2.zero;
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine() 
    {
        yield return waitOneSecond;
        var item = Instantiate(bullet, this.transform.position, Quaternion.identity);
        item.SetConfig(amoDamage, target.tag, Color.white, target.transform, amoSpeed);
        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        yield return waitOneSecond;
        StartCoroutine(MoveInRandomDiroutine());
    }

    public override void TakeDamage(int damage)
    {
        currentHealth = currentHealth + damage <= 0 ? 0 : currentHealth + damage;
        healthBar.fillAmount = currentHealth / maxHealth;
        if (currentHealth.Equals(0))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        spawner.RemoveEnemy(this);
        gameState.OnGameLost.RemoveListener(this.LostGame);
        StopAllCoroutines();
    }

    private void LostGame()
    {
        StopAllCoroutines();
        speed = 0f;
    }
}