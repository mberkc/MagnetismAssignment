using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour, IMagnetism {

    [SerializeField] Material material;
    [SerializeField] public int polarization = 0;
    [SerializeField] public float magneticChargeCapacity = 50;
    [SerializeField] int magnetismColliderRadius = 3;
    [SerializeField] bool isMetal;
    SphereCollider magnetismCollider;
    Rigidbody rb;

    void Awake () {
        // If gameobject has Rigidbody component => it will be metal
        rb = gameObject.GetComponent<Rigidbody> ();
        if (rb != null) {
            isMetal = true;

            // SphereCollider as Trigger => to detect other magnet/metals
            GameObject child = new GameObject ();
            child.name = Constants.nameCollider;
            magnetismCollider = child.AddComponent<SphereCollider> () as SphereCollider;
            magnetismCollider.radius = magnetismColliderRadius;
            magnetismCollider.isTrigger = true;
            child.transform.SetParent (transform);
            child.transform.localPosition = Vector3.zero;

            // Setting name and material
            gameObject.name = Constants.nameMetal;
            gameObject.GetComponent<MeshRenderer> ().material = material;
        } else {
            // Without Rigidbody
            gameObject.name = Constants.wOutRbName;
        }
    }

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