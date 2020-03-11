using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour, IMagnetism {

    public enum Polarization {
        Negative = -1,
        Positive = 1
    }

    [SerializeField] Material negativeMaterial;
    [SerializeField] Material positiveMaterial;
    [SerializeField] public Polarization polarization = Polarization.Positive;
    [SerializeField] public float magOfMagneticCharge = 80;
    [SerializeField] bool isMagnet;
    Rigidbody rb;
    List<GameObject> magnetismObjects = new List<GameObject> ();

    //[SerializeField] int magnetismColliderRadius = 3; // Magnetism Collider Radius
    //SphereCollider magnetismCollider; // Magnetism Collider that detects other magnet/metals

    void Awake () {
        // If gameobject has Rigidbody component => it will be magnet
        rb = gameObject.GetComponent<Rigidbody> ();
        if (rb != null) {
            isMagnet = true;

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
            switch (polarization) {
                case Polarization.Negative:
                    gameObject.name = Constants.nameNegative;
                    gameObject.GetComponent<MeshRenderer> ().material = negativeMaterial;
                    break;
                case Polarization.Positive:
                    gameObject.name = Constants.namePositive;
                    gameObject.GetComponent<MeshRenderer> ().material = positiveMaterial;
                    break;
            }
        } else {
            // Without Rigidbody
            gameObject.name = Constants.wOutRbName;
        }
    }

    void Start () {
        if (magnetismObjects.Count == 0) {
            // Caching magnet/metals for performance
            foreach (GameObject go in GameObject.FindGameObjectsWithTag (Constants.draggableTag)) {
                // every magnet/metal is added except self
                if (this.gameObject != go) {
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
    // SphereCollider(trigger) under child object will detect other magnet/metals
    void OnTriggerStay (Collider other) {
        switch (other.name) {
            case Constants.nameMetal:
                ApplyMagneticForce (Constants.permeability, magOfMagneticCharge, other.GetComponent<Metal> ().magneticChargeCapacity, other.transform.position, other.GetComponent<Metal> ().polarization);
                break;
            case Constants.nameNegative:
                ApplyMagneticForce (Constants.permeability, magOfMagneticCharge, other.GetComponent<Magnet> ().magOfMagneticCharge, other.transform.position, (int) other.GetComponent<Magnet> ().polarization);
                break;
            case Constants.namePositive:
                ApplyMagneticForce (Constants.permeability, magOfMagneticCharge, other.GetComponent<Magnet> ().magOfMagneticCharge, other.transform.position, (int) other.GetComponent<Magnet> ().polarization);
                break;
        }
    }
    */

    // Applies every available Magnetic Force
    public void ApplyMagneticForces (List<GameObject> goList) {
        foreach (GameObject magnetismObject in goList) {
            switch (magnetismObject.name) {
                case Constants.nameMetal:
                    ApplyMagneticForce (Constants.permeability, magOfMagneticCharge, magnetismObject.GetComponent<Metal> ().magneticChargeCapacity, magnetismObject.transform.position, magnetismObject.GetComponent<Metal> ().polarization);
                    break;
                case Constants.nameNegative:
                    ApplyMagneticForce (Constants.permeability, magOfMagneticCharge, magnetismObject.GetComponent<Magnet> ().magOfMagneticCharge, magnetismObject.transform.position, (int) magnetismObject.GetComponent<Magnet> ().polarization);
                    break;
                case Constants.namePositive:
                    ApplyMagneticForce (Constants.permeability, magOfMagneticCharge, magnetismObject.GetComponent<Magnet> ().magOfMagneticCharge, magnetismObject.transform.position, (int) magnetismObject.GetComponent<Magnet> ().polarization);
                    break;
            }
        }
    }

    // Applies Magnetic Force to it's own Rigidbody with Force determined by the Gilbert Model
    public void ApplyMagneticForce (float permeability, float magOfMagneticCharge1, float magOfMagneticCharge2, Vector3 otherObjectsPos, int polarization) {
        float distance = Vector3.Distance (transform.position, otherObjectsPos);
        Vector3 direction = (otherObjectsPos - transform.position).normalized;
        // If Magnets have same polarization they will repel, otherwise attract
        if ((int) this.polarization == polarization)
            direction *= -1;
        rb.AddForce (direction *
            permeability * magOfMagneticCharge1 * magOfMagneticCharge2 /
            (4 * Mathf.PI * Mathf.Pow (distance, 2)));
    }
}