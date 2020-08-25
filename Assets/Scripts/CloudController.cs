using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private float timer;
    public float timeBetweenClouds = 1;
    public float timeVariance = 1;
    public Sprite[] sprites;
    public float[] spriteOffsets;
    public bool[] isWiggly;
    public GameObject wigglyCloud;
    public GameObject sleepyCloud;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeBetweenClouds) {
            var cloudType = Random.Range(0, sprites.Length - 1);
            float y = Random.Range(-0.5f, 4.0f);
            float xv = Random.Range(-3.0f, -0.5f);
            Vector3 position = new Vector3(spriteOffsets[cloudType], y, 0);
            GameObject prefab = isWiggly[cloudType] ? wigglyCloud : sleepyCloud;
            var cloud = Instantiate(
                prefab,
                position,
                Quaternion.identity
            );
            cloud.GetComponent<SpriteRenderer>().sprite = sprites[cloudType];
            cloud.GetComponent<Flyby>().speed = xv;
            float variance = Random.Range(-timeVariance / 2, timeVariance / 2);
            timer -= timeBetweenClouds + timeVariance;
        }
    }
}
