using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CharacterCard
{
    public string name;
    public int attack;
    public int health;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Object/CharacterCardSO")]
public class CharacterCardSO : ScriptableObject
{
    public CharacterCard[] items;
}