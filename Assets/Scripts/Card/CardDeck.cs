using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class CardDeck : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer character;
    [SerializeField] SpriteRenderer Country;
    [SerializeField] TMP_Text NameTMP;
    [SerializeField] TMP_Text CardNumTMP;
    [SerializeField] TMP_Text CardExplaneTMP;
    [SerializeField] TMP_Text defenceTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text ADNameTMP;
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;
    [SerializeField] GameObject FrontSide;
    [SerializeField] GameObject BackSide;

    public ADCard item;
    bool isFront;
    public PRS originsPRS;

    public void Setup(ADCard item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        character.gameObject.SetActive(isFront);
        Country.gameObject.SetActive(isFront);
        NameTMP.gameObject.SetActive(isFront);
        ADNameTMP.gameObject.SetActive(isFront);
        CardExplaneTMP.gameObject.SetActive(isFront);
        CardNumTMP.gameObject.SetActive(isFront);
        attackTMP.gameObject.SetActive(isFront);
        defenceTMP.gameObject.SetActive(isFront);

        if (this.isFront)
        {
            character.sprite =this.item.character;
            Country.sprite = this.item.CountrySprite;
            NameTMP.text = this.item.name;
            ADNameTMP.text = this.item.ADname;
            CardExplaneTMP.text = this.item.CardExplane;
            CardNumTMP.text = this.item.CardNum.ToString();
            attackTMP.text = this.item.CardNum.ToString();
            defenceTMP.text = this.item.CardNum.ToString();

            FrontSide.SetActive(true);
            BackSide.SetActive(false);
        }
        else
        {
            card.sprite = cardBack;
            FrontSide.SetActive(false);
            BackSide.SetActive(true);
        }
    }
    void OnMouseOver()
    {
        if (isFront)
        {
            CardManager.Inst.CardMouseOver(this);
        }
    }
    void OnMouseExit()
    {
        if (isFront)
        {
            CardManager.Inst.CardMouseExit(this);
        }
    }

    void OnMouseDown()
    {
        if (isFront)
        {
            CardManager.Inst.CardMouseDown();
        }
    }

    void OnMouseUp()
    {
        if (isFront)
        {
            CardManager.Inst.CardMouseUp();
        }
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }
   
}
