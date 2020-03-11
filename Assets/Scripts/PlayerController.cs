using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] int dragTargetsLayerIndex = 8, rayMask; // Layer & Mask for draggable objects
    [SerializeField] float maxDistance = 100f; // Max distance for Raycasting, for performance
    [SerializeField] float speed = 51.6f; // Force constant

    Camera cam;
    Ray ray;
    RaycastHit hit;

    Touch touch;

    Rigidbody controlledObjectRb; // If drag (of draggable object) did start it will be set to draggable objects rb, otherwise null

    Vector3 lastPos; // on last frame

    void Start () {
        // Ignores layers except DragTargets Layer
        rayMask = 1 << dragTargetsLayerIndex;

        cam = Camera.main;
    }

    void Update () {

        // Mouse Input - Click
        if (Input.GetMouseButtonDown (0)) {
            ray = cam.ScreenPointToRay (Input.mousePosition);

            // If the raycast is a success
            if (Physics.Raycast (ray, out hit, maxDistance, rayMask)) {

                // If the target has Draggable tag
                if (hit.transform.CompareTag (Constants.draggableTag)) {

                    // Converting Mouse Position to World Position
                    lastPos = cam.ScreenToWorldPoint (Input.mousePosition);

                    controlledObjectRb = hit.transform.GetComponent<Rigidbody> ();
                }
            }
        }

        // If Drag already started or continues
        if (controlledObjectRb != null) {

            // Drag
            if (Input.GetMouseButton (0)) {
                controlledObjectRb.velocity = Vector3.zero; // Resets the velocity

                // Converting Mouse Position to World Position
                Vector3 currentPos = cam.ScreenToWorldPoint (Input.mousePosition);
                controlledObjectRb.AddForce ((currentPos - lastPos) * speed, ForceMode.VelocityChange);
                lastPos = currentPos;
            }

            // Drag End
            if (Input.GetMouseButtonUp (0)) {
                controlledObjectRb.velocity = Vector3.zero; // Resets the velocity
                controlledObjectRb = null;
            }
        }
        // Mouse Input - End

        // Touch Input
        if (Input.touchCount > 0) {
            touch = Input.GetTouch (0);
            switch (touch.phase) {

                case TouchPhase.Began: // Touch Start
                    ray = cam.ScreenPointToRay (touch.position);

                    // If the raycast is a success
                    if (Physics.Raycast (ray, out hit, maxDistance, rayMask)) {

                        // If the target has Draggable tag
                        if (hit.transform.CompareTag (Constants.draggableTag)) {

                            // Converting Touch Position to World Position
                            lastPos = cam.ScreenToWorldPoint (touch.position);
                            controlledObjectRb = hit.transform.GetComponent<Rigidbody> ();
                        }
                    }
                    break;

                case TouchPhase.Moved: // Drag
                    if (controlledObjectRb != null) {
                        controlledObjectRb.velocity = Vector3.zero; // Resets the velocity

                        // Converting Touch Position to World Position
                        Vector3 currentPos = cam.ScreenToWorldPoint (touch.position);
                        controlledObjectRb.AddForce ((currentPos - lastPos) * speed, ForceMode.VelocityChange);
                        lastPos = currentPos;
                    }
                    break;

                case TouchPhase.Ended: // Touch End -- TouchPhase.Canceled
                    if (controlledObjectRb != null) {
                        controlledObjectRb.velocity = Vector3.zero; // Resets the velocity
                        controlledObjectRb = null;
                    }
                    break;
            }
        }
        // Touch Input - End
    }

}