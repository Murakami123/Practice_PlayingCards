﻿using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseGameScene : MonoBehaviour
{
    protected bool isOnLineMode => PhotonManager.Instance.isApplicationOnlineMode;
    protected abstract string randomRoomName { get; }
    protected abstract int gamePlayerCount { get; } // このゲームに何人参加するか

    protected async UniTask JoinOrCreateRoom()
    {
        if (isOnLineMode)
        {
            Debug.Log("部屋入るの待ち");
            await PhotonManager.Instance.JoinOrCreateRoom(randomRoomName);
            Debug.Log("部屋入った");
        }
    }

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonManager.Instance.GetNetworkClientState().ToString());
    }

    // 1Pだけ実行するメソッド（山札の生成とか）
    protected abstract UniTask MasterPlayerFlow();
    
    
    // /////////////////////////////////////////////////////////////////////////////////////////
    // // PUN メソッド（Pun メソッドを呼ぶインスタンスが、その Pun メソッドを持っていけない気がした）
    // /////////////////////////////////////////////////////////////////////////////////////////
    // // string だけで SetParent する天才的なメソッド
    // [PunRPC]
    // public void SetParentStr(string cn, string ct, string pn, string pt) => PhotonExtension.SetParent(cn, ct, pn, pt);    
}