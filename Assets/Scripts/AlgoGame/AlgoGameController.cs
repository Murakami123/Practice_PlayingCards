using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

public class AlgoGameController : BaseGameScene
{
    /////////////////////////////////////////////////////
    /// 基本形
    /////////////////////////////////////////////////////        
    protected override string randomRoomName => "randomRoom_Algo";

    protected override int gamePlayerCount => 2;
    private async UniTask Start() => MainFlow().Forget();
    protected override async UniTask MasterPlayerFlow()
    {
        // マスター以外処理不要
        if (!PhotonNetwork.IsMasterClient) return;

    }
    
    /////////////////////////////////////////////////////
    /// シーンのごとの処理
    /////////////////////////////////////////////////////
    private async UniTask MainFlow()
    {
        
    }
    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonManager.Instance.GetNetworkClientState().ToString());
    }

    [SerializeField] private Transform prefabParent;

    private void Update()
    {
        Debug.Log("aaa");
        if (Input.GetKey("up"))
        {
            Debug.Log("bbb");
            var posX = UnityEngine.Random.Range(-5f, 5f);
            var posY = UnityEngine.Random.Range(-5f, 5f);
            var obj = PhotonManager.Instance.Instantiate("Photon_Cube", new Vector3(posX, posY, 0),
                (Quaternion) default);
        }
    }
}

//