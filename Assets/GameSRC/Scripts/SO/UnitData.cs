using Zenject;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitConfig", menuName = "Create unit")]
public class UnitData : ScriptableObject
{
    [SerializeField] [Range(400, 5000)] private int health;
    [SerializeField] [Range(0, 150f)] private float unitSpeed;
    [SerializeField] [Range(0, 10f)] private float amoSpeed;
    [SerializeField] [Range(-1000, 400f)] private int amoDmg;
    [SerializeField] [Range(-1000, 200)] private int unitColisionDmg;
    public int Health => health;
    public float UnitSpeed => unitSpeed;
    public float AmoSpeed => amoSpeed;
    public int AmoDmg => amoDmg;
    public int UnitColisionDmg => unitColisionDmg;
}