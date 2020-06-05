using System;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    [SerializeField] private State state;
    public State State => state;
    public UnityEvent OnGameLost, OnPlayerWinLvl;
    private void Awake()
    {
        state = State.InGame;
        if (OnGameLost == null) 
        {
            OnGameLost = new UnityEvent();
        }
        if (OnPlayerWinLvl == null) 
        {
            OnPlayerWinLvl = new UnityEvent();
        }
    }
    private void Start()
    {
        OnGameLost.AddListener(LostGame);
        OnPlayerWinLvl.AddListener(WonLvl);        
    }

    private void LostGame()
    {
        state = State.Lost;
    }
    private void WonLvl()
    {
        state = State.Win;
    }

    private void OnDestroy()
    {
        OnGameLost.RemoveAllListeners();
        OnPlayerWinLvl.RemoveAllListeners();
    }
}

public enum State 
{
    InGame,
    Lost,
    Win
}