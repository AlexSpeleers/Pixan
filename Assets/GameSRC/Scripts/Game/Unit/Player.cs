﻿using UnityEngine;
using DG.Tweening;
using Zenject;
public class Player : AbstractUnit
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private SpriteRenderer playerSprite;
    private BulletBehaviour bullet;
    private float amoSpeed;
    private int amoDmg;
    private bool isMoving = false;
    private string animationFlag = "IsWalking";
    private AbstractUnit target;
    private float coolDown = 0f;
    public Camera HealthbarCanvas { set => healthbarCanvas.worldCamera = value; }
    
    [Inject]
    public void Init(GameState gameState, Spawner spawner, BulletBehaviour bullet)
    {
        this.gameState = gameState;
        this.spawner = spawner;
        this.bullet = bullet;
    }

    public void Clone(Player player)
    {
        this.currentHealth = player.currentHealth;
        this.maxHealth = player.maxHealth;
        this.amoDmg = player.amoDmg;
        this.amoSpeed = player.amoSpeed;
        this.speed = player.speed;
    }

    private void Start()
    {
        gameState.OnPlayerWinLvl.AddListener(DisableMove);
        gameState.OnGameLost.AddListener(DisableMove);
        gameState.OnGameLost.AddListener(Die);
    }

    public override void SetDefaultConfig(UnitData playerData, Camera camera, Player player = null)
    {        
        healthbarCanvas.worldCamera = camera;
        maxHealth = currentHealth = playerData.Health;
        speed = playerData.UnitSpeed;
        amoSpeed = playerData.AmoSpeed;
        amoDmg = playerData.AmoDmg;
    }

    private void Update()
    {
        coolDown += Time.deltaTime;
        if (gameState.State.Equals(State.InGame))
        {
            if (!isMoving)
            {
                if (target != null && coolDown > 0.34f)
                {
                    Shoot();
                }
                else if (target == null)
                {
                    FindClosestEnemy();
                }
            }
        }        
    }

    public void FindClosestEnemy()
    {
        float distance = 100000f;
        foreach (var item in spawner.AliveEnemies)
        {
            var tempDistance = Vector2.Distance(item.transform.position, this.transform.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                target = item;
            }
        }
    }

    private void Shoot()
    {
        coolDown = 0f;
        var item = Instantiate(bullet, this.transform.position, Quaternion.identity);
        item.SetConfig(amoDmg, target.tag, Color.blue, target.transform, amoSpeed);
    }

    public void DisableMove()
    {
        isMoving = false;        
        playerAnimator.SetBool(animationFlag, false);
        playerBody.velocity = Vector3.zero;
    }

    public void Move(Vector2 direction)
    {
        isMoving = true;
        playerAnimator.SetBool(animationFlag, true);
        playerAnimator.SetFloat("DirectionX", direction.x);
        playerAnimator.SetFloat("DirectionY", direction.y);        
        playerBody.velocity = direction * Time.deltaTime * speed;
    }

    public override void TakeDamage(int damage)
    {
        currentHealth = currentHealth + damage <= 0 ? 0 : currentHealth + damage;
        healthBar.fillAmount = currentHealth / maxHealth;        
        if (currentHealth.Equals(0)) 
        {
            gameState.OnGameLost?.Invoke();
        }
    }

    private void Die()
    {
        playerSprite.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() => {
            this.gameObject.SetActive(false);
            playerSprite.color = Color.white;
        });
    }
    private void OnDestroy()
    {
        gameState.OnPlayerWinLvl.RemoveListener(DisableMove);
        gameState.OnGameLost.RemoveListener(DisableMove);
        gameState.OnGameLost.RemoveListener(Die);
    }
}