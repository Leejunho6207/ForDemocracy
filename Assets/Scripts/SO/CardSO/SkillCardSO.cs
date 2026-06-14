using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SkillCard
{
    public string name;
    public int attack;
    public int buf;
    public Sprite sprite;

}

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Object/SkillCardSO")]
public class SkillCardSO : ScriptableObject
{
    public SkillCard[] items;
}