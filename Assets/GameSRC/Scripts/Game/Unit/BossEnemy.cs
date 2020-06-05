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
    private Vector2 direction;
    [Inject]
    private void Init(GameState gameState, Spawner spawner, BulletBehaviour bullet)
    {
        this.gameState = gameState;
        this.spawner = spawner;
        this.bullet = bullet;
    }
    private void Start()
    {
        gameState.OnGameLost.AddListener(OnGameLost);
        waitOneSecond = new WaitForSeconds(1f);
        StateActions = new Dictionary<BossState, Action>()
        {
            { BossState.First, DoFirstState },
            { BossState.Second, DoSecondState },
            { BossState.Third, DoThirdState }
        };
        currentBossState = BossState.Cooldown;
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
        if (currentBossState.Equals(BossState.First)) 
        {
            this.transform.LookAt(target.transform);
            this.transform.position = transform.forward * Time.deltaTime * (speed + 2);
        }
        if (currentBossState.Equals(BossState.Second)) 
        {
            this.transform.position = direction * Time.deltaTime * speed;
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
        var time = 0f;
        while (time <= 2f) 
        {
            time += Time.deltaTime;
        }
        yield return new WaitForFixedUpdate();
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
            if (time >= 0.2f) 
            {
                var item = Instantiate(bullet, this.transform.position, Quaternion.identity);
                item.SetConfig(amoDamage, target.tag, Color.white, target.transform, amoSpeed);
                shots++;
                time = 0f;
            }
            time += Time.deltaTime;
        }
        currentBossState = BossState.Second;
        direction = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
        yield return new WaitForSeconds(2f);
        currentBossState = BossState.Cooldown;
        StartCoroutine(SelectState());
    }

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
        yield return new WaitForSeconds(1f);
        var k = UnityEngine.Random.Range(0, 3);
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