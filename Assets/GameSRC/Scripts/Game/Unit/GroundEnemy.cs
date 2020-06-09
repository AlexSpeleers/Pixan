using UnityEngine;
using System.Collections;
using Zenject;
using Pathfinding;

public class GroundEnemy : AbstractUnit
{
    [SerializeField] private Animator groundEnemyAnimator;
    [SerializeField] private AIDestinationSetter aIDestinationSetter;
    public AIPath aIPath;
    private Player target;
    private int collisionDamage;
    private bool canDamage = true;
    private float cashedSpeed;
    
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
        aIDestinationSetter.target = target.transform;
        this.target = target;
        healthbarCanvas.worldCamera = camera;
        maxHealth = currentHealth = groundEnemyData.Health;
        cashedSpeed = speed = groundEnemyData.UnitSpeed;
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

    //private void FixedUpdate()
    //{
    //    if (gameState.State.Equals(State.InGame))
    //    {
    //        //Vector3 targetDir = target.transform.position - transform.position;
    //        //float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
    //        //Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //        //this.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 4f * Time.deltaTime);
    //        //this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, Time.deltaTime * speed);           
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals(target.tag) && canDamage)
        {
            target?.TakeDamage(collisionDamage);
            canDamage = false;
            StartCoroutine(CanDamageRoutine());
            speed = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        speed = collision.transform.tag.Equals("Player") ? cashedSpeed : speed;
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