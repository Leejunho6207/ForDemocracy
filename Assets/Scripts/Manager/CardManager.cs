using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] ADCardSO cardSO;
    [SerializeField] GameObject CardPrefab;
    [SerializeField] List<CardDeck> myCards;
    [SerializeField] List<CardDeck> otherCards;
    [SerializeField] Transform cardSpwanPoint;
    [SerializeField] Transform otherCardSpawnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherCardLeft;
    [SerializeField] Transform otherCardRight;
    [SerializeField] ECardState eCardState;

    List<ADCard> myItemBuffer;
    List<ADCard> enemyItemBuffer;
    CardDeck selectedCard;
    bool isMyCardDrag;
    bool onMyCardArea;
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag }
    int myPutCount;

    Faction myFaction;
    Faction enemyFaction;

    public void SetFaction(Faction selectedFaction)
    {
        myFaction = selectedFaction;
        enemyFaction = selectedFaction == Faction.Jorsen ? Faction.Cordia : Faction.Jorsen;

        SetupItemBuffer();  // 선택 후 버퍼 재구성
    }

    public ADCard PopItem(bool isMine)
    {
        var buffer = isMine ? myItemBuffer : enemyItemBuffer;

        if (buffer.Count == 0)
        {
            SetupItemBuffer();
        }

        ADCard item = buffer[0];
        buffer.RemoveAt(0);
        return item;
    }

    void SetupItemBuffer()
    {
        myItemBuffer = new List<ADCard>();
        enemyItemBuffer = new List<ADCard>();

        for (int i = 0; i < cardSO.items.Length; i++)
        {
            ADCard item = cardSO.items[i];

            if (item.faction == myFaction)
            {
                for (int j = 0; j < item.percent; j++)
                    myItemBuffer.Add(item);
            }
            else if (item.faction == enemyFaction)
            {
                for (int j = 0; j < item.percent; j++)
                    enemyItemBuffer.Add(item);
            }
        }

        Shuffle(myItemBuffer);
        Shuffle(enemyItemBuffer);
    }

    void Shuffle(List<ADCard> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            ADCard temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    void Start()
    {
        SetupItemBuffer();
        TurnManager.OnAddCard += AddCard;
        TurnManager.OnTurnStarted += OnTurnStarted;
        string selected = PlayerPrefs.GetString("SelectedFaction", "Jorsen"); // 기본값 Jorsen
        Faction myFaction = (Faction)System.Enum.Parse(typeof(Faction), selected);

        SetFaction(myFaction);
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (myTurn)
        {
            myPutCount = 0;
        }
    }

    void Update()
    {
        if(isMyCardDrag)
        {
            CardDrag();

        }
        DetectCardArea();
        SetECardState();
    }

    void AddCard(bool isMine)
    {
        var CardObject = Instantiate(CardPrefab, cardSpwanPoint.position, Utils.QI);
        var card = CardObject.GetComponent<CardDeck>();
        card.Setup(PopItem(isMine), isMine);
        (isMine ? myCards : otherCards).Add(card);

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    void SetOriginOrder(bool isMine)
    {
        int count = isMine ? myCards.Count : otherCards.Count;
        for (int i = 0; i < count; i++)
        {
            var targetCard = isMine ? myCards[i] : otherCards[i];
            targetCard?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    void CardAlignment(bool isMine)
    {
        List<PRS> originCardPRSs = new List<PRS>();
        if (isMine)
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 0.5f);
        else
            originCardPRSs = RoundAlignment(otherCardLeft, otherCardRight, otherCards.Count, -0.5f, Vector3.one * 0.5f);

        var targetCards = isMine ? myCards : otherCards;
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originsPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originsPRS, true, 0.75f);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Quaternion.identity;
            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }

    public bool TryPutCard(bool isMine)
    {
        if (isMine && myPutCount >= 3)
        {
            return false;
        }
        if (!isMine && otherCards.Count <= 0)
        {
            return false;
        }
        CardDeck card = isMine ? selectedCard : otherCards[Random.Range(0, otherCards.Count)];
        var spawnPos = isMine ? Utils.MousePos : otherCardSpawnPoint.position;
        var targetCards = isMine ? myCards : otherCards;

        if (EntityManager.Inst.SpawnEntity(isMine, card.item, spawnPos))
        {
            targetCards.Remove(card);
            card.transform.DOKill();
            DestroyImmediate(card.gameObject);
            if (isMine)
            {
                selectedCard = null;
                myPutCount++;
            }
            CardAlignment(isMine);
            return true;
        }
        else
        {
            targetCards.ForEach(x => x.GetComponent<Order>().SetMostFrontOrder(false));
            CardAlignment(isMine);
            return false;
        }
        
    }

    #region MyCard

    public void CardMouseOver(CardDeck card)
    {
        if (eCardState == ECardState.Nothing)
        {
            return;
        }
        selectedCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(CardDeck card)
    {
        EnlargeCard(false, card);
    }

    public void CardMouseDown()
    {
        if (eCardState != ECardState.CanMouseDrag)
        {
            return;
        }
        isMyCardDrag = true;
    }

    public void CardMouseUp()
    {
        isMyCardDrag = false;
        if (eCardState != ECardState.CanMouseDrag)
        {
            return;
        }
        if (onMyCardArea)
        {
            EntityManager.Inst.RemoveMyEmptyentity();
        }
        else
        {
            TryPutCard(true);
        }
    }

    void CardDrag()
    {
        if (eCardState != ECardState.CanMouseDrag)
        {
            return;
        }
        if (!onMyCardArea)
        {
            selectedCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectedCard.originsPRS.scale), false);
            EntityManager.Inst.InsertMyEmptyEntity(Utils.MousePos.x);
        }
    }

    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyCardArea = Array.Exists(hits, X => X.collider.gameObject.layer == layer);
    }
    void EnlargeCard(bool isEnlarge, CardDeck card)
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originsPRS.pos.x, -3.8f, -10f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 1.0f), false);
        }
        else
        {
            card.MoveTransform(card.originsPRS, false);
        }
        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void SetECardState()
    {
        if (TurnManager.Inst.isLoading)
        {
            eCardState = ECardState.Nothing;
        }
        else if (!TurnManager.Inst.myTurn || myPutCount == 3 || EntityManager.Inst.IsFullMyEntities)
        {
            eCardState = ECardState.CanMouseOver;
        }
        else if (TurnManager.Inst.myTurn && myPutCount == 0)
        {
            eCardState = ECardState.CanMouseDrag;
        }
    }

    #endregion
}
