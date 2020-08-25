using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UpdateIndicatorPosition : MonoBehaviour
{
    Growable growable;
    SpriteShapeController spriteShape;
    // Start is called before the first frame update
    void Start()
    {
        growable = transform.parent.gameObject.GetComponent<Growable>();
        spriteShape = transform.parent.gameObject.GetComponent<SpriteShapeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteShape) {
            var count = spriteShape.spline.GetPointCount();
            var last = spriteShape.spline.GetPosition(count - 1);
            var basePos = spriteShape.gameObject.transform.position;
            transform.position = basePos + last;
        }
        if(growable) {
            if(!growable.growable) {
                Destroy(this.gameObject);
            }
        }
    }
}
