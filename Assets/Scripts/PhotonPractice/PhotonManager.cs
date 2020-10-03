using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

// public class PhotonManager : MonoBehaviourPunCallbacks
public class PhotonManager : MonoBehaviour
{
    public static PhotonManager Instance { get; private set; }
    public bool isApplicationOnlineMode { get; private set; }

    public async UniTask SetAppricationOnlineMode(bool isOnline)
    {
        isApplicationOnlineMode = isOnline;
        // オフラインモードにする時、接続中だったら切断。
        if (!isOnline)
        {
            Debug.Log("接続中だったら切断する処理");
            await UniTask.Delay(1000);
        }
    }


    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        // シングルトン
        if (Instance == null) Instance = this;

        // 初期化。アプリ起動時1回呼べばよい？
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 部屋入出
    /// </summary>
    private bool isWaitingRoomJoin;

    public async UniTask JoinOrCreateRoom(string roomName)
    {
        isWaitingRoomJoin = true;
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
        await UniTask.WaitUntil(() => !isWaitingRoomJoin); // ルーム入室待ち
    }

    private async UniTask OnJoinedRoom()
    {
        isWaitingRoomJoin = false;
    }

    // クライアントの現在の情報を返す
    public ClientState GetNetworkClientState()
    {
        return PhotonNetwork.NetworkClientState;
    }
}