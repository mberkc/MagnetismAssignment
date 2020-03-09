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
    [SerializeField] public float magOfMagneticCharge = 200;
    [SerializeField] int magnetismColliderRadius = 3;
    [SerializeField] bool isMagnet;
    SphereCollider magnetismCollider;
    Rigidbody rb;

    void Awake () {
        // If gameobject has Rigidbody component => it will be magnet
        rb = gameObject.GetComponent<Rigidbody> ();
        if (rb != null) {
            isMagnet = true;

            // SphereCollider as Trigger => to detect other magnet/metals
            GameObject child = new GameObject ();
            child.name = Constants.nameCollider;
            magnetismCollider = child.AddComponent<SphereCollider> () as SphereCollider;
            magnetismCollider.radius = magnetismColliderRadius;
            magnetismCollider.isTrigger = true;
            child.transform.SetParent (transform);
            child.transform.localPosition = Vector3.zero;

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