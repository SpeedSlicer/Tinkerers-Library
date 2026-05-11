using Unity.Netcode;
using UnityEngine;

public class NetworkController : MonoBehaviour 
{   
    [SerializeField]
    private Transform spawnpoint;
    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        
        response.Position = spawnpoint.position;
        response.Rotation = spawnpoint.rotation;

        response.Pending = false;
    }
}
