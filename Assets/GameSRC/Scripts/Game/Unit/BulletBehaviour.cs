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
        this.transform.LookAt(target);
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z + offsetZ);
        var distance = Vector3.Distance(target.position, this.transform.position);
        var destination = this.transform.position + (this.transform.eulerAngles.normalized * distance);
        this.bulletCol.color = bulletCollor;
        this.targetTag = targetTag;
        this.damage = damage;
        transform.DOMove(destination, distance / amoSpeed).SetEase(Ease.Linear)
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
        Destroy(this);
    }

    private void OnDestroy()
    {
        this.StopCoroutine(DestroyRoutine());
        gameState.OnGameLost.RemoveListener(this.OnGameFinished);
        gameState.OnPlayerWinLvl.RemoveListener(this.OnGameFinished);
    }
}