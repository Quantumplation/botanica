using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Parallax : MonoBehaviour
{
    public GameObject target;
    public float depth = 1;
    public float offset = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            target.transform.position.y / depth + offset,
            transform.position.z
        );
    }
}
