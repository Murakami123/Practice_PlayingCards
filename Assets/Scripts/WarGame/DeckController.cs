using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    private List<Card> cardList = new List<Card>();
    private readonly float cardThickness = 0.65f;
    private readonly string cardPrefabName = "CardPrefab";
    [SerializeField] private Transform cardPrefabParent;

    public void ResetDeck(int useJokerCount)
    {
        Debug.Log("DeckController.ResetDeck");

        // 全カードリセット
        var sootAndNoList = DefaltSootAndNoList(useJokerCount);
        var shuffledCardList = CardShuffle(sootAndNoList); // シャッフル
        var instancePosY = 0.0f;

        // カード生成
        cardList.Clear();
        Debug.Log("shuffledCardList.Count:" + shuffledCardList.Count);

        for (int i = 0; i < shuffledCardList.Count; i++)
        {
            var cardObj = PhotonManager.Instance.Instantiate(cardPrefabName, new Vector3(0f, instancePosY, 0f),
                (Quaternion) default);
            var card = cardObj.GetComponent<Card>();
            card.Init(parent:transform, shuffledCardList[i].Item1, shuffledCardList[i].Item2);
            cardList.Add(card);
            instancePosY += cardThickness;
        }
    }

    // public Card GetCard()
    // {
    //     if (cardList.Count > 0) return cardList[cardList.Count-1];
    //     return null;
    // }

    public Card GetCard()
    {
        Card card = null;
        if (cardList.Count > 0)
        {
            card = cardList[cardList.Count - 1];
            cardList.RemoveAt(cardList.Count - 1);
        }

        return card;
    }

    public int GetDeckCardCount => cardList.Count;
    //      ☑ シャッフルすることができる。
    //      ・ カードを山札に戻すことができる。
    //      ・ 全てのカードを山札に戻すことができる（ゲームリトライ）。
    //      ・ タイミングで、2枚を両プレイヤーの手元のカードにする。
    //      ・ 残りの枚数が0枚の状態で、タイミングになったらゲーム終了
    //      ☑ カードを引くことできる。
    //      ☑ 残り枚数がだいたいわかるぐらいの見た目。

    // デフォルトの全スート、全カードのリスト
    private List<(CardSoot, int)> DefaltSootAndNoList(int jokerCount)
    {
        var list = new List<(CardSoot, int)>()
        {
            (CardSoot.Spade, 1),
            (CardSoot.Spade, 2),
            (CardSoot.Spade, 3),
            (CardSoot.Spade, 4),
            (CardSoot.Spade, 5),
            (CardSoot.Spade, 6),
            (CardSoot.Spade, 7),
            (CardSoot.Spade, 8),
            (CardSoot.Spade, 9),
            (CardSoot.Spade, 10),
            (CardSoot.Spade, 11),
            (CardSoot.Spade, 12),
            (CardSoot.Spade, 13),
            (CardSoot.Heart, 1),
            (CardSoot.Heart, 2),
            (CardSoot.Heart, 3),
            (CardSoot.Heart, 4),
            (CardSoot.Heart, 5),
            (CardSoot.Heart, 6),
            (CardSoot.Heart, 7),
            (CardSoot.Heart, 8),
            (CardSoot.Heart, 9),
            (CardSoot.Heart, 10),
            (CardSoot.Heart, 11),
            (CardSoot.Heart, 12),
            (CardSoot.Heart, 13),
            (CardSoot.Diamond, 1),
            (CardSoot.Diamond, 2),
            (CardSoot.Diamond, 3),
            (CardSoot.Diamond, 4),
            (CardSoot.Diamond, 5),
            (CardSoot.Diamond, 6),
            (CardSoot.Diamond, 7),
            (CardSoot.Diamond, 8),
            (CardSoot.Diamond, 9),
            (CardSoot.Diamond, 10),
            (CardSoot.Diamond, 11),
            (CardSoot.Diamond, 12),
            (CardSoot.Diamond, 13),
            (CardSoot.Clover, 1),
            (CardSoot.Clover, 2),
            (CardSoot.Clover, 3),
            (CardSoot.Clover, 4),
            (CardSoot.Clover, 5),
            (CardSoot.Clover, 6),
            (CardSoot.Clover, 7),
            (CardSoot.Clover, 8),
            (CardSoot.Clover, 9),
            (CardSoot.Clover, 10),
            (CardSoot.Clover, 11),
            (CardSoot.Clover, 12),
            (CardSoot.Clover, 13),
        };

        for (int i = 0; i < jokerCount; i++)
        {
            list.Add((CardSoot.Joker, 14));
        }

        return list;
    }

    private List<(CardSoot, int)> CardShuffle(List<(CardSoot, int)> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }
}