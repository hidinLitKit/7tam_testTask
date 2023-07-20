using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Bullet : NetworkBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        GetComponent<Rigidbody2D>().velocity = transform.up*bulletSpeed;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag=="Money")
        {
            return;
        }
        if(col.gameObject.tag=="Player")
        {
            Debug.Log("Hit success");
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
