using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Drowner : MonoBehaviour
{
    SpriteShapeController controller;
    public GrowthController growthController;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<SpriteShapeController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Your water capacity depends on how much root you've grown
        var capacity = 1000.0f;
        var allPlants = GameObject.FindGameObjectsWithTag("Plant");
        foreach(var plant in allPlants) {
            var type = plant.GetComponent<Growable>().rootType;
            if(type == PlantSectionType.MediumRoot || type == PlantSectionType.SkinnyRoot) {
                capacity += plant.GetComponent<SpriteShapeController>().spline.GetPointCount();
            }
        }
        if(growthController.water > capacity) {
            var deficit = growthController.water - capacity;
            var deathFraction = deficit / capacity;
            gameObject.GetComponent<Dying>().dying = Mathf.Clamp(deathFraction, 0, 1);
        } else {
            gameObject.GetComponent<Dying>().dying = 0.0f;
        }
    }
}
