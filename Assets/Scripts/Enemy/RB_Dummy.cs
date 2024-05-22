using UnityEngine;
using UnityEngine.Events;

public class RB_Dummy : RB_Enemy
{
    [SerializeField] private GameObject DeathSpawnPrefab;
    protected override void Death()
    {
        EventDead?.Invoke();
        Instantiate(DeathSpawnPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}