using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] ADCard item;
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer character;
    [SerializeField] SpriteRenderer Country;
    [SerializeField] TMP_Text NameTMP;
    [SerializeField] TMP_Text CardNumTMP;
    [SerializeField] TMP_Text CardExplaneTMP;
    [SerializeField] TMP_Text defenceTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text ADNameTMP;
    [SerializeField] Sprite cardFront;
    [SerializeField] GameObject smileParticle;

    public int attack;
    public int defence;
    public int CardNum;
    public bool isMine;
    public bool isBossOrEmpty;
    public bool attackable;
    public Vector3 originPos;
    int liveCount;

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;     
    }
    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;    
    }

    void OnTurnStarted(bool myTurn)
    {
        if (isBossOrEmpty)
        {
            return;
        }
        if (isMine == myTurn)
        {
            liveCount++;

            smileParticle.SetActive(liveCount < 1);
        }
    }

    public void Setup(ADCard item)
    {
        attack = item.attack;
        defence = item.defence;
        CardNum = item.CardNum;

        character.sprite = item.character;
        Country.sprite = item.CountrySprite;  
        entity.sprite = item.cardFront;

        NameTMP.text = item.name;
        CardExplaneTMP.text = item.CardExplane;  
        ADNameTMP.text = item.ADname;       
        CardNumTMP.text = CardNum.ToString();
        attackTMP.text = attack.ToString();
        defenceTMP.text = defence.ToString();
    }

    void OnMouseDown()
    {
        if (isMine)
        {
            EntityManager.Inst.EntityMouseDown(this);
        }
    }

    void OnMouseUp()
    {
        if (isMine)
        {
            EntityManager.Inst.EntityMouseUp();
        }
    }

    void OnMouseDrag()
    {
        if (isMine)
        {
            EntityManager.Inst.EntityMouseDrag();
        }
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(pos, dotweenTime);
        }
        else
        {
            transform.position = pos;
        }
    }
}
