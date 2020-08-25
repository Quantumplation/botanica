using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourcePatch : MonoBehaviour
{
    public ResourceType type;
    public float totalResources;
    public float currentResources;
    public float saturation => currentResources / totalResources;
    SpriteShapeRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = this.gameObject.GetComponent<SpriteShapeRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(type == ResourceType.Mineral) {
            float H,S,V;
            Color.RGBToHSV(renderer.color, out H, out S, out V);
            renderer.color = Color.HSVToRGB(H, saturation, V);
        } else if(type == ResourceType.Water) {
            renderer.color = new Color(
                renderer.color.r,
                renderer.color.g,
                renderer.color.b,
                saturation
            );
        }
    }
}
