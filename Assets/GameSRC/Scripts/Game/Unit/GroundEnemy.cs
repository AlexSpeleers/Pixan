using UnityEngine;
using System.Collections;
using Zenject;

public class GroundEnemy : AbstractUnit
{
    [SerializeField] private Animator groundEnemyAnimator;
    private Player target;
    private int collisionDamage;
    private bool canDamage = true;
    
    [Inject]
    private void Init(GameState gameState, Spawner spawner)
    {
        this.gameState = gameState;
        this.spawner = spawner;
    }
    private void Start()
    {
        groundEnemyAnimator.SetBool("IsMoving", true);
        waitOneSecond = new WaitForSeconds(1f);
    }
    public override void SetDefaultConfig(UnitData groundEnemyData, Camera camera, Player target)
    {
        this.target = target;
        healthbarCanvas.worldCamera = camera;
        maxHealth = currentHealth = groundEnemyData.Health;
        speed = groundEnemyData.UnitSpeed;
        collisionDamage = groundEnemyData.UnitColisionDmg;
        gameState.OnGameLost.AddListener(this.LostGame);
    }
    public override void TakeDamage(int damage) 
    {
        currentHealth = currentHealth + damage <= 0 ? 0 : currentHealth + damage;
        healthBar.fillAmount = currentHealth / maxHealth;
        if (currentHealth.Equals(0)) 
        {
            spawner.RemoveEnemy(this);
            gameState.OnGameLost.RemoveListener(this.LostGame);
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (gameState.State.Equals(State.InGame))
        {
            Vector3 targetDir = target.transform.position - transform.position;
            var angleBetween = Vector3.Angle(transform.forward, targetDir);
            float AngleDeg = (180 / Mathf.PI) * angleBetween;
            this.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, AngleDeg));
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, Time.deltaTime * speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Player") && canDamage) 
        {
            target?.TakeDamage(-collisionDamage);
            canDamage = false;
            StartCoroutine(CanDamageRoutine());
        }
    }

    IEnumerator CanDamageRoutine() 
    {
        yield return waitOneSecond;
        canDamage = true;
    }

    private void LostGame() 
    {        
        groundEnemyAnimator.SetBool("IsMoving", false);
    }
}