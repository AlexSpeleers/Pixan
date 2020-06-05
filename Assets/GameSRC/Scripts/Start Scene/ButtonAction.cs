using UnityEngine;
using DG.Tweening;
public class ButtonAction : MonoBehaviour
{
    [SerializeField] [Range(0.2f, 1f)] float animationDuration = 0.4f;
    [SerializeField] private Ease ease = Ease.Linear;

    public void OnPlayButtonClicked() 
    {
        foreach (Transform item in this.transform) 
        {
            item.DOScale(Vector3.zero, animationDuration).SetEase(ease);
        }
    }

    public void OnQuitButtonClicked() 
    {
        Application.Quit();
    }
}