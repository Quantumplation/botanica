using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    public float timeToLive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if(timeToLive <= 0) {
            Destroy(this.gameObject);
        }
    }
}
