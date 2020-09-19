using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public abstract class Cardbase : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] Sprite frontSpr;
    [SerializeField] Sprite backSpr;
    public CardSoot soot { get; private set; }    
    public int cardNo { get; private set; }    
    public bool isShowFront { get; private set; }

    RectTransform _rect;
    private RectTransform rectTransform => (_rect) ? _rect : _rect = GetComponent<RectTransform>(); 
    
    public virtual void  Init( Vector3 anchorPos, CardSoot soot, int cardNo, bool isShowFront = false)
    {
        this.soot = soot;  
        this.cardNo = cardNo;
        this.isShowFront = isShowFront;
        rectTransform.anchoredPosition = anchorPos;
        gameObject.SetActive(true);
    }

    private  readonly Vector3 rotation90 = new Vector3(0f, 90f, 0f);
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
}

public enum CardSoot
{
    Joker = -1,
    Spade = 1,
    Heart = 2,
    Diamond = 3,
    Clover = 4,
}