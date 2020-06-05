using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public sealed class Scatterer : MonoBehaviour
{
    private Ease ease = Ease.Linear;
    [Range(0f, 3f)]
    [SerializeField] private float animationDuration = 0f;
    [Tooltip("Make sure you have a correspoding desination point for your element")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private List<Vector2> destinationPoints;
    [SerializeField] private Image image;
    private void OnEnable()
    {
        var i = 0;
        foreach (RectTransform item in panel.transform) 
        {
            item.DOLocalMove(destinationPoints[i],animationDuration).SetEase(ease);
            i++;
        }
        image.DOFade(0.5f, animationDuration).SetEase(ease);
    }
}