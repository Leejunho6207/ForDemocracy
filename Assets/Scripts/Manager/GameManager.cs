using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] NotificationPanel notificationPanel;

    void Start()
    {
        StartGame();
    }
    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }
    void InputCheatKey()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TurnManager.OnAddCard?.Invoke(true);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            TurnManager.OnAddCard?.Invoke(false);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            TurnManager.Inst.EndTurn();
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            CardManager.Inst.TryPutCard(false);
        }
    }

    public void StartGame()
    {
        StartCoroutine(TurnManager.Inst.StartGameCo());
    }
    public void Notification(string message)
    {
        notificationPanel.Show(message);
    }
}
