using Unity.Cinemachine;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{


    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab;

    [Header("Dependencies")]
    [SerializeField] private CinemachineCamera cinemachineCamera;


    private void Awake()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        cinemachineCamera.Follow = player.transform;
        cinemachineCamera.LookAt = player.transform;
    }
}
