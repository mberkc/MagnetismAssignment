using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizer : MonoBehaviour {
    [SerializeField] float baseWHRatio = 480f / 800f, baseOrthoSize;

    void Start () {
        // Resizes the camera (orthographic size) in relevance to the screen width/height
        baseOrthoSize = Camera.main.orthographicSize;
        float orthoSize = baseOrthoSize * (baseWHRatio / ((float) Screen.width / (float) Screen.height));
        Camera.main.orthographicSize = orthoSize;
    }
}