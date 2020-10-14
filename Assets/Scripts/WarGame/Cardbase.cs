using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine.Events;

public abstract class Cardbase : MonoBehaviour, IPunObservable
{
    [SerializeField] Image cardImage;
    [SerializeField] Sprite backSpr;
    private Sprite frontSpr;
    private RectTransform _rect;
    private RectTransform rectTransform => (_rect) ? _rect : _rect = GetComponent<RectTransform>();
    private  readonly Vector3 rotation90 = new Vector3(0f, 90f, 0f);
    
    public virtual void Init(CardSoot soot, int cardNo, bool isShowFront = false)
    {
        this.soot = soot;  
        this.cardNo = cardNo;
        this.isShowFront = isShowFront;
        this.frontSpr = ResourceManager.Instance.GetSprite(soot, cardNo);
        gameObject.SetActive(true);
        // position の調整は PhotonManager.Init 方で行ってください
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
    
    /////////////////////////////////////////////////////////////////////
    /// 同期したい内容
    /////////////////////////////////////////////////////////////////////
    public CardSoot soot { get; private set; }
    public int cardNo { get; private set; }
    public bool isShowFront { get; private set; }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 送信側。
            // 同期したいデータを集めて SendNext() で送る。
            stream.SendNext((int)soot);
            stream.SendNext(cardNo);
            stream.SendNext(isShowFront);
        }
        else
        {
            // 受信側。
            soot = (CardSoot)stream.ReceiveNext();
            cardNo = (int)stream.ReceiveNext();
            isShowFront = (bool)stream.ReceiveNext();
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