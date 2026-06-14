using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ADCard
{
    public string name; //카드 이름
    public string ADname; // 공격/방어 인지 알려주는 텍스트 공간
    public string CardExplane; //카드 설명란
    public int CardNum; //카드 넘버링
    public int attack;
    public int defence;
    public Sprite cardFront; //카드 사진
    public Sprite CountrySprite; //카드의 국가 사진
    public Sprite character; // 캐릭터 사진
    public float percent;//나올 확율
    public Faction faction; //진영
}

public enum Faction
{
    Jorsen, //조르센 진영
    Cordia //코드리아 진영
}

[CreateAssetMenu(fileName = "CardSO" , menuName = "Scriptable Object/ADCardSO")]
public class ADCardSO : ScriptableObject
{
    public ADCard[] items;
}