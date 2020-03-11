using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour, IMagnetism {

    [SerializeField] Material material;
    [SerializeField] public int polarization = 0;
    [SerializeField] public float magneticChargeCapacity = 40;
    [SerializeField] bool isMetal;
    Rigidbody rb;
    List<GameObject> magnetismObjects = new List<GameObject> ();

    //[SerializeField] int magnetismColliderRadius = 3; // Magnetism Collider Radius
    //SphereCollider magnetismCollider; // Magnetism Collider that detects other magnet/metals

    void Awake () {
        // If gameobject has Rigidbody component => it will be metal
        rb = gameObject.GetComponent<Rigidbody> ();
        if (rb != null) {
            isMetal = true;

            // Freezing Rotation on X and Z Axis (objects with Box Collider)
            if (GetComponent<Collider> ().GetType () == typeof (BoxCollider))
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            /* Magnetism Collider Related            
            // SphereCollider as Trigger => to detect other magnet/metals
            GameObject child = new GameObject ();
            child.name = Constants.nameCollider;
            magnetismCollider = child.AddComponent<SphereCollider> () as SphereCollider;
            magnetismCollider.radius = magnetismColliderRadius;
            magnetismCollider.isTrigger = true;
            child.transform.SetParent (transform);
            child.transform.localPosition = Vector3.zero;
            */

            // Setting name and material
            gameObject.name = Constants.nameMetal;
            gameObject.GetComponent<MeshRenderer> ().material = material;
        } else {
            // Without Rigidbody
            gameObject.name = Constants.wOutRbName;
        }
    }

    void Start () {
        if (magnetismObjects.Count == 0) {
            // Caching magnets for performance, because metals wont attract/repel each other for this simulation
            foreach (GameObject go in GameObject.FindGameObjectsWithTag (Constants.draggableTag)) {
                // every magnet is added
                if (go.name != Constants.nameMetal) {
                    magnetismObjects.Add (go);
                }
            }
        }
    }

    // Every Fixed Update applying magnetic force to the rigidbody
    void FixedUpdate () {
        ApplyMagneticForces (magnetismObjects);
    }

    /* Magnetism Collider Related
    // SphereCollider(trigger) under child object will detect magnets
    void OnTriggerStay (Collider other) {
        switch (other.name) {
            case Constants.nameNegative:
                ApplyMagneticForce (Constants.permeability, magneticChargeCapacity, other.GetComponent<Magnet> ().magOfMagneticCharge, other.transform.position, (int) other.GetComponent<Magnet> ().polarization);
                break;
            case Constants.namePositive:
                ApplyMagneticForce (Constants.permeability, magneticChargeCapacity, other.GetComponent<Magnet> ().magOfMagneticCharge, other.transform.position, (int) other.GetComponent<Magnet> ().polarization);
                break;
        }
    }
    */

    // Applies every available Magnetic Force
    public void ApplyMagneticForces (List<GameObject> goList) {
        foreach (GameObject magnetismObject in goList) {
            switch (magnetismObject.name) {
                case Constants.nameNegative:
                    ApplyMagneticForce (Constants.permeability, magneticChargeCapacity, magnetismObject.GetComponent<Magnet> ().magOfMagneticCharge, magnetismObject.transform.position, (int) magnetismObject.GetComponent<Magnet> ().polarization);
                    break;
                case Constants.namePositive:
                    ApplyMagneticForce (Constants.permeability, magneticChargeCapacity, magnetismObject.GetComponent<Magnet> ().magOfMagneticCharge, magnetismObject.transform.position, (int) magnetismObject.GetComponent<Magnet> ().polarization);
                    break;
            }
        }
    }

    // Applies Magnetic Force to it's own Rigidbody with Force determined by the Gilbert Model
    public void ApplyMagneticForce (float permeability, float magOfMagneticCharge1, float magOfMagneticCharge2, Vector3 otherObjectsPos, int polarization) {
        float distance = Vector3.Distance (transform.position, otherObjectsPos);
        // Metal is being pulled towards the magnet
        Vector3 direction = (otherObjectsPos - transform.position).normalized;
        rb.AddForce (direction *
            permeability * magOfMagneticCharge1 * magOfMagneticCharge2 /
            (4 * Mathf.PI * Mathf.Pow (distance, 2)));
    }
}