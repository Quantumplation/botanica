using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public enum ResourceType {
    Mineral,
    Water,
    Energy
}

[Serializable]
public class CollectionStat {
    public ResourceType type;
    public int amount;
    public float tickTime;
    public float timer;
}

public class ResourceCollector : MonoBehaviour
{
    public CollectionStat[] stats;
    public GrowthController growthController;
    SpriteShapeController controller;
    GameObject[] resourcePatches;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<SpriteShapeController>();
        resourcePatches = GameObject.FindGameObjectsWithTag("Resource");
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var stat in stats) {
            if(stat.tickTime == 0) {
                continue;
            }
            stat.timer += Time.deltaTime;

            while(stat.timer >= stat.tickTime) {
                float amount = 0;
                var count = controller.spline.GetPointCount();
                var basePoint = transform.position;
                // TODO: inefficient, but oh well
                for(int i = 0; i < count; i++) {
                    var pt = basePoint + controller.spline.GetPosition(i);
                    foreach(var res in resourcePatches) {
                        var resPatch = res.GetComponent<ResourcePatch>();
                        var collider = res.GetComponent<PolygonCollider2D>();
                        if(resPatch && resPatch.type == stat.type && collider.OverlapPoint(pt)) {
                            var take = (float)stat.amount * resPatch.saturation;
                            amount += take;
                            resPatch.currentResources -= take; 
                        }
                    }
                }
                switch(stat.type) {
                    case ResourceType.Mineral: {
                        growthController.mineralUp += amount / stat.tickTime;
                    } break;
                    case ResourceType.Water: {
                        growthController.waterUp += amount / stat.tickTime;
                    } break;
                }
                switch(stat.type) {
                    case ResourceType.Mineral: {
                        growthController.minerals += (int)amount;
                    } break;
                    case ResourceType.Water: {
                        growthController.water += (int)amount;
                    } break;
                }
                stat.timer -= stat.tickTime;
            }
        }
    }
}
