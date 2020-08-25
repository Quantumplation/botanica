using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyby : MonoBehaviour
{
    int frame = 0;
    public float speed = 0;
    public float stretchSpeed = 0.000000001f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =
            transform.position +
            new Vector3(speed, 0, 0) * Time.deltaTime;

    }
}
