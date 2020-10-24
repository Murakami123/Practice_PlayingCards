using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PhotonMonobehaviour : MonoBehaviour, IPunObservable
{
    private PhotonView _pv;
    private PhotonView photonView => (_pv != null) ? _pv : _pv = PhotonView.Get(this);
    public int photonViewId => photonView.ViewID;

    //////////////////////////////////////////////////////////////
    // Tag 変更 
    //////////////////////////////////////////////////////////////

    public void SetTag(ObjTag tag)
    {
        photonView.RPC(nameof(Photon_SetTag), RpcTarget.AllViaServer, tag.ToString());
    }

    [PunRPC] // (Photonで呼ぶため直接呼ばないこと！)    
    public void Photon_SetTag(string tagName)
    {
        gameObject.tag = tagName;
    }

    //////////////////////////////////////////////////////////////
    // SetParent 
    //////////////////////////////////////////////////////////////

    // 親に唯一のタグをつけてから使う。
    public void SetParent(ObjTag tag)
    {
        Debug.LogError("SetParent。tag:" + tag);
        photonView.RPC(nameof(Photon_SetParent_WithTag), RpcTarget.AllViaServer, tag.ToString());
    }

    // 親が PhotonView を持ってる場合、こっちを使う。
    public  void SetParent(PhotonView parentPV)
    {
        var parentViewId = parentPV.ViewID.ToString();
        var parentTag = parentPV.gameObject.tag;
        photonView.RPC(nameof(Photon_SetParent_WithPhotonView), RpcTarget.AllViaServer, parentViewId, parentTag);
    }

    // 親が PhotonView を持ってない場合用
    public  void SetParent(Transform parent)
    {
        var parentPath = GetHierarchyPath(parent.transform);
        var parentTag = parent.gameObject.tag;
        photonView.RPC(nameof(Photon_SetParent), RpcTarget.AllViaServer, parentPath, parentTag);
    }

    public void SetChild(Transform child)
    {
        // SetParent では重すぎたりうまく動かなかったら選択肢の一つになるかも
    }

    [PunRPC] // (Photonで呼ぶため直接呼ばないこと！)    
    public void Photon_SetParent_WithTag(string tag)
    {
        var parent = GetTransform_WithTag(tag);
        transform.SetParent(parent, false);
    }

    [PunRPC] // (Photonで呼ぶため直接呼ばないこと！)    
    public void Photon_SetParent_WithPhotonView(string parentViewId, string parentTag)
    {
        var parent = GetTransform_WithId(parentViewId, parentTag);
        transform.SetParent(parent, false);
    }

    [PunRPC] // (Photonで呼ぶため直接呼ばないこと！)    
    public void Photon_SetParent(string parentPath, string parentTag)
    {
        var parent = GetTransform_WithPath(parentPath, parentTag);
        transform.SetParent(parent, false);
    }

    // 一つの obj しか割り当ててないタグ名から transform 取得
    private Transform GetTransform_WithTag(string tagName)
    {
        if (tagName == "Untagged") Debug.Log("tag が設定されていません");
        var tagObjs = GameObject.FindGameObjectsWithTag(tagName);
        if (tagObjs.Count() > 1) Debug.LogError("同じタグの obj が複数ある。唯一のタグにしてください");
        if (!tagObjs.Any()) Debug.LogError("見つからない。objPath:" + tagObjs + ", tagName:" + tagName);
        return tagObjs.First().transform;
    }

    // PhotonViewId から transform 取得
    private Transform GetTransform_WithId(string photonViewId, string tagName)
    {
        if (tagName == "Untagged") Debug.Log("tag が設定されていません。photonViewId:" + photonViewId);
        var tagObjs = GameObject.FindGameObjectsWithTag(tagName);
        var objs = tagObjs.Where(obj => obj.GetComponent<PhotonView>().ViewID.ToString() == photonViewId);
        if (objs.Count() > 1) Debug.LogError("同じタグの同名同じパスの obj が複数あります。NW上で唯一のタグとパスにしてください:" + objs.First());
        if (!objs.Any()) Debug.LogError("見つからない。photonViewId:" + photonViewId + ", tagName:" + tagName);
        return objs.First().transform;
    }

    // パスとタグ名から transform 取得
    private Transform GetTransform_WithPath(string objPath, string tagName)
    {
        if (tagName == "Untagged") Debug.Log("tag が設定されていません。objPath:" + objPath);
        var tagObjs = GameObject.FindGameObjectsWithTag(tagName);
        var objs = tagObjs.Where(obj => GetHierarchyPath(obj.transform) == objPath);
        if (objs.Count() > 1) Debug.LogError("同じタグの同名同じパスの obj が複数あります。NW上で唯一のタグとパスにしてください:" + objs.First());
        if (!objs.Any()) Debug.LogError("見つからない。objPath:" + objPath + ", tagName:" + tagName);
        return objs.First().transform;
    }

    // Hierarcy 上のフルパス取得
    private string GetHierarchyPath(Transform self)
    {
        string path = self.gameObject.name;
        Transform parent = self.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return "/" + path;
    }

    //////////////////////////////////////////////////////////////
    // 名前変更
    //////////////////////////////////////////////////////////////
    public void ChangeName(string changeName)
    {
        photonView.RPC(nameof(Photon_ChangeName), RpcTarget.AllViaServer, changeName);
    }

    [PunRPC] // (Photonで呼ぶため直接呼ばないこと！)    
    public void Photon_ChangeName(string changeName) => gameObject.name = changeName;

    //////////////////////////////////////////////////////////////
    // その他　
    //////////////////////////////////////////////////////////////

    // 同期したい内容。override して使う。
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
        }
        else
        {
        }
    }
}


public enum ObjTag
{
    PlaerParent,
    DeckParent,
    PlayerParent,
    Photon_Deck,
    Photon_Player,
    Photon_Card,
    Photon_CardParent,
    Photon_WinCardParent,

    HikiwakeCardParent,
}