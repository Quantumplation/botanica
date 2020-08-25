using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterDrainer : MonoBehaviour
{
    public int requirement;
    public float tickTime;
    public GrowthController growthController;
    SpriteShapeController controller;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<SpriteShapeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tickTime == 0) {
            return;
        }
        var count = controller.spline.GetPointCount();
        var amount = requirement * (1 + (count / 10));
        growthController.waterDown += amount / tickTime;
        timer += Time.deltaTime;
        while(timer > tickTime) {
            if(growthController.water > amount) {
                growthController.water -= amount;
                gameObject.GetComponent<Dying>().dying = 0.0f;
            } else {
                var deficit = amount - growthController.water;
                growthController.water = 0;
                var deathFraction = deficit / amount;
                gameObject.GetComponent<Dying>().dying = deathFraction;
            }
            timer -= tickTime;
        }
    }
}
