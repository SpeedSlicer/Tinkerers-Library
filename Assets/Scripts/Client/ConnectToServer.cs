using Unity.Netcode;
using UnityEngine;

public class ConnectToServer : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "SampleScene";

    [SerializeField]
    GameObject netzwerkManager;
    public void StartHostAndLoad()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(mainSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void StartClientOnly()
    {
        NetworkManager.Singleton.StartClient();
    }
}
