using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;

public class PhotonManager :
    MonoBehaviourPunCallbacks, // ロビー、ルームに入退室のコールバックを呼べる。
    IPunObservable // [PunRPC] でメソッドを実行するために必要
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

    // GetRoomListは一定時間ごとに更新され、その更新のタイミングで実行する処理
    // ルームリストに更新があった時
    private List<RoomInfo> roomInfoList = new List<RoomInfo>(); // 最新のルーム一覧情報

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("ルーム情報更新。ルーム数:" + roomList.Count);
        // =>
        roomInfoList = roomList;
    }

    //////////////////////////////////////////////////////////////
    // PUN メソッド
    //////////////////////////////////////////////////////////////

    private PhotonView _pv;
    private PhotonView photonView => (_pv != null) ? _pv : _pv = PhotonView.Get(this);

    // スクリプトから呼ぶ奴
    // public void SetParent(PhotonView pv, Transform parent)
    // {
    //     var viewIdStr = pv.ViewID.ToString();
    //     var childTag = pv.gameObject.tag;
    //     var parentPath = GetHierarchyPath(parent.transform);
    //     var parentTag = parent.gameObject.tag;
    //     Debug.Log("viewIdStr:" + viewIdStr);
    //     photonView.RPC(nameof(Photon_SetParent_WithId), RpcTarget.AllViaServer, viewIdStr, childTag, parentPath, parentTag);
    // }
    //
    // // スクリプトから呼ぶ奴
    // public void SetParent(Transform child, Transform parent)
    // {
    //     var childTag = child.gameObject.tag;
    //     var parentPath = GetHierarchyPath(parent.transform);
    //     var parentTag = parent.gameObject.tag;
    //     photonView.RPC(nameof(Photon_SetParent_WithPath), RpcTarget.AllViaServer, child.name, childTag, parentPath, parentTag);
    // }
    //
    // private Transform GetTransform(int viewId, string tagName)
    // {
    //     if (tagName == "Untagged") Debug.LogError("tag が設定されていません。objName:" + tagName);
    //     GameObject obj = null;
    //     var tagObjs = GameObject.FindGameObjectsWithTag(tagName);
    //     for (int i = 0; i < tagObjs.Length; i++)
    //     {
    //         var view = tagObjs[i].GetComponent<PhotonView>();
    //         if ((view != null) && (view.ViewID == viewId))
    //             obj = tagObjs[i];
    //     }
    //
    //     if (obj == null) Debug.LogError("見つからない。viewId:" + viewId + ", tagName:" + tagName);
    //     return obj.transform;
    // }

    // private Transform GetTransform(string objName, string tagName)
    // {
    //     if (tagName == "Untagged") Debug.Log("tag が設定されていません。objName:" + objName);
    //     var tagObjs = GameObject.FindGameObjectsWithTag(tagName);
    //     var objs = tagObjs.Where(obj => obj.name == objName);
    //     if (objs.Count() > 1) Debug.LogError("同じタグの同名 obj が複数あります。NW上で唯一の名前にしてください:" + objs.First());
    //     if (!objs.Any()) Debug.LogError("見つからない。objName:" + objName + ", tagName:" + tagName);
    //     return objs.First().transform;
    // }

    // private Transform GetTransform(string objPath, string tagName)
    // {
    //     if (tagName == "Untagged") Debug.Log("tag が設定されていません。objPath:" + objPath);
    //     var tagObjs = GameObject.FindGameObjectsWithTag(tagName);
    //     var objs = tagObjs.Where(obj => GetHierarchyPath(obj.transform) == objPath);
    //     if (objs.Count() > 1) Debug.LogError("同じタグの、同名同じパスの obj が複数あります。NW上で唯一のタグとパスにしてください:" + objs.First());
    //     if (!objs.Any()) Debug.LogError("見つからない。objPath:" + objPath + ", tagName:" + tagName);
    //     return objs.First().transform;
    // }



    /////////////////////////////////////////////////////////////////////////////////////////
    // Photonから呼ばれる奴
    /////////////////////////////////////////////////////////////////////////////////////////

    #region Photonから呼ばれる奴

    //
    //
    // [PunRPC] // (Photonで呼ぶため直接呼ばないこと！)    
    // public void Photon_SetParent_WithPath(string childPath, string childTag, string parentPath, string parentTag)
    // {
    //     var child = GetTransform(childPath, childTag);
    //     var parent = GetTransform(parentPath, parentTag);
    //     child.SetParent(parent, false);
    // }

    #endregion


    /////////////////////////////////////////////////////////////////////////////////////////
    // 別世界の PhotonManager と同期する内容
    /////////////////////////////////////////////////////////////////////////////////////////
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}

/////////////////////////////////////////////////////////////////////
/// 通常のメソッドと Photonのメソッド の違いを吸収する拡張メソッド。
// /////////////////////////////////////////////////////////////////////
// public static class PhotonExtension
// {
//     public static void PhotonSetParent(this Transform trans, Transform parent)
//     {
//         PhotonView photonView = PhotonView.Get(trans);
//         if (photonView == null) Debug.LogError("trans に PhotonView がついてない");
//
//         var childTag = trans.gameObject.tag;
//         var parentTag = parent.gameObject.tag;
//
//         photonView.RPC("SetParentStr", RpcTarget.AllViaServer, trans.name, childTag, parent.name, parentTag);
//     }
//
//     // string だけで SetParent する天才的なメソッド
//     public static void SetParent(string childName, string childTag, string parentName, string parentTag)
//     {
//         var childTags = GameObject.FindGameObjectsWithTag(childTag);
//         var childNames = childTags.Where(obj => obj.name == childName);
//         if (childNames.Count() > 1) Debug.LogError("同じタグの同名 obj が複数あります。NW上で唯一の名前にしてください");
//         var child = childNames.First();
//
//         var parentTags = GameObject.FindGameObjectsWithTag(parentTag);
//         var parentNames = parentTags.Where(obj => obj.name == parentName);
//         if (parentNames.Count() > 1) Debug.LogError("同じタグの同名 obj が複数あります。NW上で唯一の名前にしてください");
//         var parent = parentNames.First();
//
//         child.transform.SetParent(parent.transform, false);
//     }
// }
public static class PhotonViewExtension
{
    public static void SetName(this PhotonView photonView, string changeName)
    {
        photonView.gameObject.name = changeName;
    }
}

// Photon カスタムプロパティ
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