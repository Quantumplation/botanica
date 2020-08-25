using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScroll : MonoBehaviour
{
    public float startupDelay = 3f;
    public bool startupLock = true;
    public int topMargin = 250;
    public float maxSpeed = 10;
    public float bottomMargin = 250;

    public float absoluteMaxDepth = -19;
    public float plantDepth = 0;
    public float plantHeight = 0;

    public float startHeight = 1;
    public float startDepth = -3;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if(startupDelay > 0) {
            startupDelay -= Time.deltaTime;
            return;
        }
        if(startupLock) {
            var delta = transform.position.y + 3f;
            var step = delta * Time.deltaTime / 5f;
            if(step > 0.0001f) {
                transform.Translate(new Vector3(
                    0,
                    -step,
                    0
                ));
            } else {
                transform.position = new Vector3(
                    0, -3, 0
                );
            }

            return;
        }

        float mouseY = Input.mousePosition.y;
        float cameraY = this.transform.position.y;
        float newY = cameraY;
        if(mouseY < topMargin) {
            var scrollSpeed = mouseY.Remap(
               0, topMargin,
               maxSpeed, 0.0f
            );
            newY -= scrollSpeed * Time.deltaTime;
        } else if(mouseY > Screen.height - bottomMargin) {
            var scrollSpeed = mouseY.Remap(
                Screen.height - bottomMargin, Screen.height,
                0, maxSpeed
            );
            newY += scrollSpeed * Time.deltaTime;
        }
        var maxD = Mathf.Max(absoluteMaxDepth, Mathf.Min(startDepth, plantDepth));
        if(newY < maxD) {
            newY = maxD;
        }
        var maxH = Mathf.Max(startHeight, plantHeight);
        if(newY > maxH) {
            newY = maxH;
        }
        this.transform.position = new Vector3(
            this.transform.position.x,
            newY,
            this.transform.position.z
        );
    }
}
