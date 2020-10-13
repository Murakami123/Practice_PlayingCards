using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;


// public class PhotonManager : MonoBehaviourPunCallbacks
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    /// <summary>
    /// オンラインONOFF切り替え
    /// </summary>
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

    /////////////////////////////////////////////////////
    /// ロビー、ルーム関連
    /////////////////////////////////////////////////////

    // // ロビーに入る機能。
    // public async UniTask JoinLobby()
    // {
    //     Debug.Log("ロビーに入る");
    //     PhotonNetwork.JoinLobby();
    //     await UniTask.WaitUntil(() => GetNetworkClientState() == ClientState.JoinedLobby); // ルーム入室待ち
    //     Debug.Log("ロビーに入った");
    // }

    /// 部屋入出
    public async UniTask JoinOrCreateRoom(string roomName)
    {
        // isWaitingRoomJoin = true;
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
        Debug.Log("部屋入るの待ち");

        // ◆ ルーム入室待ち_A案
        // await UniTask.WaitUntil(() => !isWaitingRoomJoin); 

        // ◆ ルーム入室待ち_B案
        await UniTask.WaitUntil(() => GetNetworkClientState() == ClientState.Joined); // ルーム入室待ち

        Debug.Log("部屋入るの待ち_終わり");
    }

    private void OnJoinedRoom()
    {
        // Debug.Log("部屋入った！");
        // isWaitingRoomJoin = false;
    }

    // クライアントの現在の情報を返す
    public ClientState GetNetworkClientState()
    {
        return PhotonNetwork.NetworkClientState;
    }

    /////////////////////////////////////////////////////
    /// 各種メソッド
    /////////////////////////////////////////////////////
    // Photon 的な生成（世界に生成）
    public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation = default, byte group = 0,
        object[] data = null)
    {
        // =>
        Debug.Log("生成");
        return PhotonNetwork.Instantiate(prefabName, position, rotation, group, data);
    }

    // プレイヤーの人数が揃うの待ち
    public async UniTask WaitPlayerGetTogether(string roomName, int gamePlayerCount)
    {
        // ◆案1_入室中の部屋
        var room = PhotonNetwork.CurrentRoom;

        // ◆案2_部屋一覧からこの部屋の情報を取得（ルーム情報更新が呼ばれないから取れない）
        // var room = roomInfoList.Find(roomInfo => roomInfo.Name == roomName);

        if (room == null)
            Debug.LogError("おかしい。roomName" + roomName + ", roomInfoList.Count:" + roomInfoList.Count);
        else Debug.Log("現在の人数。ルーム名:" + room.Name + ", 人数:" + room.PlayerCount);

        await UniTask.WaitUntil(() => room.PlayerCount == gamePlayerCount);
        Debug.Log("人数揃った");
    }

    // タグ名の obj の子にする
    public static void SetAnchoredPos(RectTransform rect, Vector3 anchoredPos)
    {
        // var parentObj = GameObject.FindWithTag(parentTagName);
        // rect.SetParent(parentObj.transform);
        rect.anchoredPosition = anchoredPos;
    }

    public static void SetAnchoredPos(RectTransform rect, Transform parent)
    {
        // var parentObj = GameObject.FindWithTag(parentTagName);
        rect.SetParent(parent, false);
    }

    // GetRoomListは一定時間ごとに更新され、その更新のタイミングで実行する処理
    // ルームリストに更新があった時
    private List<RoomInfo> roomInfoList = new List<RoomInfo>(); // 最新のルーム一覧情報
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    { 
        Debug.Log("ルーム情報更新。ルーム数:" + roomList.Count);
        // =>
        roomInfoList = roomList;
    }   
}

public class PhotonProperty : MonoBehaviour
{
    // ルームプロパティ用のハッシュ(Dictionary)
    private static ExitGames.Client.Photon.Hashtable roomHash;

    private void Start()
    {
        // ルームプロパティ用のこのプレイヤーのハッシュ(Dictionary)を生成
        roomHash = new ExitGames.Client.Photon.Hashtable();
    }

    // ルームプロパティ ===========================================

    // RoomPropertyが更新された時に呼ばれる
    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable changedRoomHash)
    {
        // 更新したプレイヤーが保持しているハッシュを入れる
        roomHash = changedRoomHash;
    }

    // ルームプロパティのセット -----------------------------------
    // キーが既に存在していたら上書き
    public static void SetRoomProperty<T>(string key, T value)
    {
        roomHash[key] = value;

        // 自身のハッシュをネット上に送信
        // PhotonNetwork.room.SetCustomProperties(roomHash);
    }

    // 一次元配列用
    public static void SetRoomPropertyArray<T>(string key, T[] value)
    {
        // 要素数256を超える場合はここをshortなどに変更してください
        for (byte i = 0; i < value.Length; i++)
        {
            roomHash[key + i] = value[i];
        }

        // 自身のハッシュをネット上に送信
        // PhotonNetwork.room.SetCustomProperties(roomHash);
    }

    public static void SetRoomPropertyArray<T>(string key, byte arrayNum, T value)
    {
        roomHash[key + arrayNum] = value;

        // 自身のハッシュをネット上に送信
        // PhotonNetwork.room.SetCustomProperties(roomHash);
    }

    // キーが既に存在している場合エラーが出る
    public static void SetRoomPropertyAdd<T>(string key, T value)
    {
        roomHash.Add(key, value);

        // 自身のハッシュをネット上に送信
        // PhotonNetwork.room.SetCustomProperties(roomHash);
    }

    // ルームプロパティをゲット -----------------------------------
    public static T GetRoomProperty<T>(string key)
    {
        // outを受け取るための変数を用意
        object value;

        // 指定したキーがあれば返す
        if (roomHash.TryGetValue(key, out value))
        {
            // ボックス化解除
            return (T) value;
        }

        // 無かったらnullが返る
        return (T) value;
    }

    // 一次元配列用
    public static T GetRoomPropertyArray<T>(string key, byte arrayNum)
    {
        // outを受け取るための変数を用意
        object value;

        // 指定したキーがあれば返す
        if (roomHash.TryGetValue(key + arrayNum, out value))
        {
            // ボックス化解除
            return (T) value;
        }

        return (T) value;
    }

    // ============================================================
}