using Unity.Netcode;
using UnityEngine;

public class PlayerOnLoadSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        var spawns = GameObject.FindGameObjectsWithTag("Spawnpoint");
        if (spawns.Length != 0)
        {
            this.gameObject.transform.position = spawns[UnityEngine.Random.Range(0, spawns.Length)].transform.position;
        }
        else
        {
            this.gameObject.transform.position = new Vector3(0, 5, 0);
        }

        Destroy(this);
    }
}
