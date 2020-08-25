using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UndergroundFade : MonoBehaviour
{
    public Camera theCamera;
    public float fadeStart = 0;
    public float fadeEnd = -1;
    public GameObject dirt;
    public Text[] gameText;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        var cameraY = theCamera.transform.position.y;
        float opacity = cameraY.Remap(
            fadeStart, fadeEnd, 0.0f, 1.0f
        );
        Color color = Color.white;
        var renderer = dirt.GetComponent<SpriteShapeRenderer>();
        // TODO: super hacky
        var sp = dirt.GetComponent<SpriteRenderer>();
        if(renderer) {
            color = renderer.color;
        } else {
            color = sp.color;
        }
        var newColor = new Color(
            color.r,
            color.g,
            color.b,
            Mathf.Clamp(opacity, 0.0f, 1.0f)
        );

        if(renderer) {
            renderer.color = newColor;
        } else {
            sp.color = newColor;
        }

        var textFade = cameraY.Remap(
            -4, -5,
            .2f, .8f
        );

        if(textFade < .2f) {
            textFade = .2f;
        } else if(textFade > .8f) {
            textFade = .8f;
        }
        foreach(var text in gameText) {
            var txtColor = Color.HSVToRGB(0,0,textFade);
            txtColor.a = text.color.a;
            text.color = txtColor;
        }
    }
}
