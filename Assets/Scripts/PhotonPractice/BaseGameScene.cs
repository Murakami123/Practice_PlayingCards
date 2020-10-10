using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseGameScene : MonoBehaviour
{
    // オンライン対戦かどうか
    // protected bool isOnLineMode
    // {
    //     get { return PhotonManager.Instance.isApplicationOnlineMode; }
    // }

    protected bool isOnLineMode => PhotonManager.Instance.isApplicationOnlineMode;

    protected abstract string randomRoomName { get; }

    protected async UniTask JoinOrCreateRoom()
    {
        if (isOnLineMode)
        {
            Debug.Log("randomRoomName:" + randomRoomName);
            await PhotonManager.Instance.JoinOrCreateRoom(randomRoomName);
            Debug.Log("部屋入った");
        }
    }

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonManager.Instance.GetNetworkClientState().ToString());
    }

    // protected abstract async UniTask MainFlow();
}