using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public abstract class Cardbase : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite frontSpr, backSpr;
    public void Init()
    {

    }

    public async UniTask ReturnCard(bool isShowFront)
    {
        transform.Dorotate();
        await UniTask.Delay(1);
        await transform.DOMove(transform.position + Vector3.up, 1.0f);
    }


}
