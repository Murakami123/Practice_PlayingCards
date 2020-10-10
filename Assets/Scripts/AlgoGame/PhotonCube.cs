using Photon.Pun;
using UnityEngine;

/// 相手に自作クラスの内容を同期したい場合のクラス
[RequireComponent(typeof(SpriteRenderer))]
public class PhotonCube : MonoBehaviour, IPunObservable
{
    private SpriteRenderer spriteRenderer;
    private void Start() => spriteRenderer = GetComponent<SpriteRenderer>();

    // 自分（送信側（stream.IsWriting == true））、
    // 相手（受信側（stream.IsWriting == false））両方で呼ばれるメソッド
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 送信側。
            // 同期したいデータを集めて SendNext() で送る。
            var red = spriteRenderer.color.r;
            var gre = spriteRenderer.color.g;
            var blu = spriteRenderer.color.b;
            var isSpriteEnabled = spriteRenderer.enabled;
            stream.SendNext(red);
            stream.SendNext(gre);
            stream.SendNext(blu);
            stream.SendNext(isSpriteEnabled);
        }
        else
        {
            // 受信側。
            var red = (float) stream.ReceiveNext();
            var gre = (float) stream.ReceiveNext();
            var blu = (float) stream.ReceiveNext();
            spriteRenderer.color = new Color(red, gre, blu, 1f);
            spriteRenderer.enabled = (bool) stream.ReceiveNext();
        }
    }
}