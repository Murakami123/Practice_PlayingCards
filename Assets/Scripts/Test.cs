using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Hoge().Forget();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private async UniTask Hoge()
    {
        Debug.Log("あああ");
        await UniTask.Delay(5000);
        Debug.Log("3秒待って実行");
    }
}
