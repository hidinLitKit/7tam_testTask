using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CoinSpawner : NetworkBehaviour
{
    public GameObject moneyPrefab;
    [SerializeField] private Transform uppperLeftBorder;
    [SerializeField] private Transform lowerRightBorder;
    private Collider2D[] colliders;
    private float collidersCheckRadius = 100f;
    [Server]
    public void Spawn(string matchId)
    {
            for(int i=0;i<10;i++)
            {
                bool canSpawnHere = false;
                Vector2 position = new Vector2(0f,0f);
                int safetyNet = 0;
                while(!canSpawnHere)
                {
                    position = new Vector2(Random.Range(uppperLeftBorder.position.x,lowerRightBorder.position.x),Random.Range(uppperLeftBorder.position.y,lowerRightBorder.position.y));
                    canSpawnHere = PreventSpawnOverlap(position);
                    if (canSpawnHere) break;
                    safetyNet++;
                    if(safetyNet>50) break;
                }
                
                GameObject prefab = Instantiate(moneyPrefab, position, Quaternion.identity);
                NetworkServer.Spawn(prefab);
                prefab.GetComponent<NetworkMatch>().matchId = matchId.ToGuid();
        }
    }
    bool PreventSpawnOverlap(Vector2 spawnPos)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position,collidersCheckRadius);
        foreach(Collider2D col in colliders)
        {
            Vector2 centerPoint = col.bounds.center;
            float width = col.bounds.extents.x;
            float height = col.bounds.extents.y;

            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;
            float lowerExtent = centerPoint.y - height;
            float upperExtent = centerPoint.y + height;

            if (spawnPos.x>=leftExtent && spawnPos.x<=rightExtent)
            {
                if(spawnPos.y>=lowerExtent && spawnPos.y <=upperExtent)
                {
                    return false;
                }
            }
        }
        return true;

    }
    

}