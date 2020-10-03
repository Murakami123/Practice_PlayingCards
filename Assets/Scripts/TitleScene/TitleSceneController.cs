using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    /// <summary>
    /// オンラインオフライン切り替え
    /// </summary>
    public void SetOnlineMode(bool isOnline)
        => PhotonManager.Instance.SetAppricationOnlineMode(isOnline);

    /// <summary>
    /// シーン変更
    /// </summary>
    private readonly string warGameSceneName = "WarGameScene";

    private readonly string algoGameSceneame = "AlgoGameScene";
    public void LoadScene_WarGame() => LoadScene(warGameSceneName);
    public void LoadScene_AlgoGame() => LoadScene(algoGameSceneame);
    private void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
}