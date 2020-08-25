using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class Dying : MonoBehaviour
{
    public float dying = 0f;

    public Color dyingColor = new Color(202,168,65);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var ssr = gameObject.GetComponent<SpriteShapeRenderer>();
        if(dying > 0) {
            ssr.color = dyingColor;
            var spline = gameObject.GetComponent<SpriteShapeController>().spline;
            var count = spline.GetPointCount();
            var last = spline.GetPosition(count - 1);
            var prev = spline.GetPosition(count - 2);

            var prevDelta = prev - last;
            var shrinkDir = prevDelta.normalized;
            var delta = shrinkDir * dying * Time.deltaTime;
            if(delta.sqrMagnitude > prevDelta.sqrMagnitude) {
                delta = prevDelta;
            }
            var newPosition = last + delta;
            if((newPosition - prev).sqrMagnitude < 0.04) {
                if(spline.GetPointCount() == 2) {
                    Destroy(this.gameObject);
                    return;
                }

                spline.RemovePointAt(count - 1);
                // Get the count again, because it might have updated
                for(var i = 0; i < count - 1; i++) {
                    var minH = .1f;
                    var h = spline.GetHeight(i);
                    var dH = (h - minH) / 5f;
                    var newH = h - dH;
                    spline.SetHeight(i, newH);
                }
            } else {
                spline.SetPosition(count - 1, newPosition);
            }
        } else {
            ssr.color = Color.white;
        }
    }
}
