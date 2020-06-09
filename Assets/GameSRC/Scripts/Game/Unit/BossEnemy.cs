using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BossEnemy : AbstractUnit
{
    private BulletBehaviour bullet;
    private Player target;
    private Dictionary<BossState, Action> StateActions;
    private BossState currentBossState;
    private int amoDamage;
    private float amoSpeed;
    private int collisionDamage;

    private bool canDamage = true;
    private Vector3 direction;
    [Inject]
    private void Init(GameState gameState, Spawner spawner, BulletBehaviour bullet)
    {
        this.gameState = gameState;
        this.spawner = spawner;
        this.bullet = bullet;
    }
    private void Start()
    {
        currentBossState = BossState.Cooldown;
        gameState.OnGameLost.AddListener(OnGameLost);
        waitOneSecond = new WaitForSeconds(1f);
        StateActions = new Dictionary<BossState, Action>()
        {
            { BossState.First, DoFirstState },
            { BossState.Second, DoSecondState },
            { BossState.Third, DoThirdState }
        };
    }
    public override void SetDefaultConfig(UnitData unitData, Camera camera, Player target)
    {
        this.target = target;
        healthbarCanvas.worldCamera = camera;
        maxHealth = currentHealth = unitData.Health;
        speed = unitData.UnitSpeed;
        collisionDamage = unitData.UnitColisionDmg;
        amoDamage = unitData.AmoDmg;
        amoSpeed = unitData.AmoSpeed;
        StartCoroutine(SelectState());
    }
    
    private void FixedUpdate()
    {
        if (currentBossState == BossState.First) 
        {
            this.transform.position += (target.transform.position - this.transform.position).normalized * Time.fixedDeltaTime * speed * 2;
        }
        if (currentBossState == BossState.Second) 
        {
            this.transform.position += direction.normalized * Time.fixedDeltaTime * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Player") && canDamage)
        {
            target?.TakeDamage(collisionDamage);
            canDamage = false;
            StartCoroutine(CanDamageRoutine());
        }
    }
    IEnumerator CanDamageRoutine()
    {
        yield return waitOneSecond;
        canDamage = true;
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

    private void DoFirstState()
    {
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        currentBossState = BossState.First;
        float duration = Time.time + 2.0f;
        while (Time.time <= duration) 
        {            
            yield return null;
        }
        currentBossState = BossState.Cooldown;
        StartCoroutine(SelectState());
    }

    private void DoSecondState()
    {
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        var time = 0.2f;
        var shots = 0;
        while (shots < 3) 
        {
            if (time >= 0.4f) 
            {
                var item = Instantiate(bullet, this.transform.position, Quaternion.identity);
                item.SetConfig(amoDamage, target.tag, Color.white, target.transform, amoSpeed);
                shots++;
                time = 0f;
            }
            time += Time.deltaTime;
        }
        direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        currentBossState = BossState.Second;
        yield return new WaitForSeconds(2f);
        currentBossState = BossState.Cooldown;
        StartCoroutine(SelectState());
    }

    //TODO: make an arc shoot mechanic
    private void DoThirdState() 
    {
        for (int i = 0; i < 3; i++) 
        {
            var item = Instantiate(bullet, this.transform.position, Quaternion.identity);
            if (i == 0)
            {
                item.SetConfig(amoDamage, target.tag, Color.white, target.transform, amoSpeed);
            }
            else if (i == 1)
            {
                item.SetConfig(amoDamage, target.tag, Color.white, target.transform, amoSpeed, 30);
            }
            else
                item.SetConfig(amoDamage, target.tag, Color.white, target.transform, amoSpeed, -30);

        }
        StartCoroutine(SelectState());
    }

    IEnumerator SelectState() 
    {
        var k = UnityEngine.Random.Range(0, 3);
        yield return waitOneSecond;
        var action = StateActions[(BossState)k] as Action;
        currentBossState = (BossState)k;
        action();
    }

    private void OnGameLost()
    {
        StopAllCoroutines();
        this.enabled = false;
    }

    private void OnDestroy()
    {
        gameState.OnGameLost.RemoveListener(OnGameLost);
        spawner.RemoveEnemy(this);
        StopAllCoroutines();
    }
}
enum BossState 
{
    First,
    Second,
    Third,
    Cooldown
}