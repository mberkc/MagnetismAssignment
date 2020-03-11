using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMagnetism {

    // Applies every available Magnetic Force
    void ApplyMagneticForces (List<GameObject> goList);

    // Applies Magnetic Force to Metal/Magnet
    void ApplyMagneticForce (float permeability, float magOfMagneticCharge1, float magOfMagneticCharge2, Vector3 otherObjectsPos, int polarization);

}