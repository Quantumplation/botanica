using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerFade : MonoBehaviour
{
    public float fadeTimer = 5;
    public bool fadeOut = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(fadeTimer < 0) {
            if(fadeOut) {
                Destroy(this.gameObject);
            } else {
                Destroy(this);
            }
        }
        fadeTimer -= Time.deltaTime;
        if(fadeTimer < 1) {
            var newValue = fadeOut ? fadeTimer : 1 - fadeTimer;
            var txt = gameObject.GetComponent<Text>();
            var txtTMP = gameObject.GetComponent<TextMeshProUGUI>();
            if(txtTMP) {
                txtTMP.color = new Color(
                    txtTMP.color.r,
                    txtTMP.color.g,
                    txtTMP.color.b,
                    newValue
                );
            } else if(txt) {
                txt.color = new Color(
                    txt.color.r,
                    txt.color.g,
                    txt.color.b,
                    newValue
                );
            } else {
                var sprite = gameObject.GetComponent<SpriteRenderer>();
                sprite.color = new Color(
                    sprite.color.r,
                    sprite.color.g,
                    sprite.color.b,
                    newValue
                );
            }
        }
    }
}
