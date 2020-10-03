using UnityEngine;

public class DontDestrouOnLoad : MonoBehaviour
{
    void Start() => DontDestroyOnLoad(gameObject);
}