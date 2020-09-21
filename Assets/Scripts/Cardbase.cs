using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public abstract class Cardbase : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] Sprite backSpr;
    private Sprite frontSpr;
    private RectTransform _rect;
    private RectTransform rectTransform => (_rect) ? _rect : _rect = GetComponent<RectTransform>();
    public CardSoot soot { get; private set; }
    public int cardNo { get; private set; }
    public bool isShowFront { get; private set; }
    private  readonly Vector3 rotation90 = new Vector3(0f, 90f, 0f);
    
    public virtual void  Init( Vector3 anchorPos, CardSoot soot, int cardNo, bool isShowFront = false)
    {
        this.soot = soot;  
        this.cardNo = cardNo;
        this.isShowFront = isShowFront;
        this.frontSpr = ResourceManager.Instance.GetSprite(soot, cardNo);
        rectTransform.anchoredPosition = anchorPos;
        gameObject.SetActive(true);
    }

    public async UniTask MoveCard(Transform cardParent, Vector3 movePos, bool isLittleShit = false)
    {
        // Hierarcy上の親変更
        transform.SetParent(cardParent);
        
        // 位置の移動
        if (isLittleShit)
        {
            var randomPosX = UnityEngine.Random.Range(-30f, 30f);
            var randomPosY = UnityEngine.Random.Range(-30f, 30f);
            movePos += new Vector3(randomPosX, randomPosY,0f);
        }

        await UniTask.Delay(200);
        rectTransform.DOLocalMove(movePos, 0.2f);
    }

    public async UniTask ReturnCard(bool isShowFront, bool isImmidiate = false)
    {
        // 表のカードをひっくり返して表、裏のカードをひっくり返して裏の挙動は必要なさそう
        if(this.isShowFront == isShowFront) return;

        // 裏返す
        {
            if (isImmidiate)
            {
                // 画像差し替え
                cardImage.sprite = (isShowFront) ? frontSpr : backSpr;
            }
            else
            {
                // 半分回転して見えなくする
                await UniTask.Delay(100);
                transform.DORotate(rotation90, 0.1f);

                // 画像差し替え
                cardImage.sprite = (isShowFront) ? frontSpr : backSpr;

                // 半分回転して戻す
                await UniTask.Delay(100);
                transform.DORotate(Vector3.zero, 0.1f);
            }
        }
    }

    /////////////////////////////////////////////////////////////////////
    /// タップや長押し関連
    /////////////////////////////////////////////////////////////////////

    [SerializeField] private GameObject sentakuzumiObj;
    public bool isChoiced { get; private set; } // カードが選択されてるかどうか
    public void SetChoice(bool isChoice)
    {
        isChoiced = isChoice;
        sentakuzumiObj.SetActive(isChoiced);
    }

    public void OnTap() => SetChoice(isChoice: true);

    // private UnityAction tapCallBack;
    // public void ResetTapCallback() => tapCallBack = null;

    // public async UniTask<Card> SetTapCallBack(UnityAction callback)

    // {
    //     tapCallBack = callback;
    // }
    //
    
    

}

public enum CardSoot
{
    Joker = -1,
    Spade = 1,
    Heart = 2,
    Diamond = 3,
    Clover = 4,
}