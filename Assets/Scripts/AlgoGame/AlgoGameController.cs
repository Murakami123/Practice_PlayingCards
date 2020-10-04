using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

public class AlgoGameController : MonoBehaviour
{
    private bool isOnLineMode => PhotonManager.Instance.isApplicationOnlineMode; // オンライン対戦かどうか
    private readonly string randomRoomName = "randomRoom_Algo_";
    private async UniTask Start()
    {
        if (isOnLineMode)
        {
            await PhotonManager.Instance.JoinOrCreateRoom(randomRoomName);
        }

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
            var obj = PhotonManager.Instance.Instantiate("Photon_Cube", new Vector3(posX, posY,0), (Quaternion)default );
        }
    }
}

//



