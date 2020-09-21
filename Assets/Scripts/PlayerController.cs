using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private RectTransform cardPosParent;
    private List<Card> cardList = new List<Card>();

    // カードの入手
    public void AddCard(Card card)
    {
        cardList.Add(card);
        AdjustCardPos(); // カードを入手するたびに、手札のカード全部の位置調整はよくあること
    }

    // 手もとのカードの位置調整
    private void AdjustCardPos()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            var card = cardList[i];
            var parent = cardPosParent;
            card.MoveCard(parent, parent.localPosition).Forget();
        }
    }

    // ユーザが選択したカードを返す。必ず await すること。
    public async UniTask<Card> PlayerChoiceCard()
    {
        // 所持カードのどれかが選択状態になるのを待つ
        // （大富豪とか複数枚選べるゲームはここで調整）
        await UniTask.WaitUntil(()=> cardList.Any(card => card.isChoiced));
        var choiceCard = cardList.First(card => card.isChoiced);
        
        return choiceCard;
    }
    
    // 全ての所持カードの選択状態解除
    public void ReleaseChoiceCard()
    {
        for (int i = 0; i < cardList.Count; i++)
            cardList[i].SetChoice(isChoice:false);
    }

    // カードを返す
    public Card GetCard(Card card)
    {
        // TODO : 指定したカードを返し、所持カードリストから削除
        return null;
    }
    
    ////////////////////////////////////////////
    /// WarGamePlayerController: PlayerControllerBase
    ////////////////////////////////////////////
    [SerializeField] private RectTransform WinGetCardParent;
    public List<Card> winGetCardList { get; private set; } = new List<Card>();
    public async UniTask SetWinCard(Card winGetCard)
    {
        winGetCardList.Add(winGetCard);

        // 勝った2枚のカードを雑に移動
        winGetCard.MoveCard(WinGetCardParent, WinGetCardParent.localPosition, isLittleShit:true);
    }


}
