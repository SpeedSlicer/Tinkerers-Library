using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerOnLoadSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return null;

        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawnpoint");

        Vector3 spawnPos;

        if (spawns.Length > 0)
        {
            spawnPos = spawns[Random.Range(0, spawns.Length)].transform.position;
        }
        else
        {
            spawnPos = new Vector3(0, 20, 0);
        }

        spawnPos += Vector3.up * 2f;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.position = spawnPos;
            rb.rotation = Quaternion.identity;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.Sleep();
            rb.WakeUp();
        }
        else
        {
            transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
        }
    }
}