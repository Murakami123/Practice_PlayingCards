using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

    // protected abstract async UniTask MainFlow();
}