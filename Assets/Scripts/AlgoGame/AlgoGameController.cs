using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AlgoGameController : MonoBehaviour
{
    private bool isOnLineMode => PhotonManager.Instance.isApplicationOnlineMode; // オンライン対戦かどうか
    private readonly string randomRoomName = "randomRoom_Algo";
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
}
