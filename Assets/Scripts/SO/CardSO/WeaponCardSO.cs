using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WeaponCard
{
    public string name;
    public int attack;
    public int defend;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Object/WeaponCardSO")]
public class WeaponCardSO : ScriptableObject
{
    public WeaponCard[] items;
}