using System.Collections;
using UnityEngine;
using DG.Tweening;
using Zenject;

public class BulletBehaviour : MonoBehaviour
{
    [Inject] private GameState gameState;
    [SerializeField] private SpriteRenderer bulletCol;
    private string targetTag;
    private int damage;    
    void Start()
    {
        gameState.OnGameLost.AddListener(OnGameFinished);
        gameState.OnPlayerWinLvl.AddListener(OnGameFinished);
        StartCoroutine(DestroyRoutine());
    }

    public void SetConfig(int damage, string targetTag, Color bulletCollor, Transform target, float amoSpeed, float offsetZ = 0) 
    {        
        //var direction = target.transform.position - this.transform.position;
        //var x = direction.magnitude * Mathf.Cos(Mathf.Atan2(direction.y, direction.x) + offsetZ);
        //var y = direction.magnitude * Mathf.Sin(Mathf.Atan2(direction.y,direction.x) + offsetZ);
        //var destination = this.transform.position + (this.transform.eulerAngles.normalized * distance);
        var distance = Vector3.Distance(target.position, this.transform.position);
        this.bulletCol.color = bulletCollor;
        this.targetTag = targetTag;
        this.damage = damage;
        transform.DOMove(target.position, distance / amoSpeed).SetEase(Ease.Linear)
            .OnComplete(() => Destroy(this.gameObject));
    }

    IEnumerator DestroyRoutine() 
    {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(targetTag)) 
        {            
            collision.gameObject.GetComponent<AbstractUnit>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        if (collision.tag.Equals("Wall")) 
        {
            Destroy(this.gameObject);
        }
    }

    private void OnGameFinished()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        this.StopCoroutine(DestroyRoutine());
        gameState.OnGameLost.RemoveListener(this.OnGameFinished);
        gameState.OnPlayerWinLvl.RemoveListener(this.OnGameFinished);
    }
}