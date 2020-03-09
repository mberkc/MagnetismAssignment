using UnityEngine;

public interface IMagnetism {

    // Applies Magnetic Force to Metal/Magnet
    void ApplyMagneticForce (float permeability, float magOfMagneticCharge1, float magOfMagneticCharge2, Vector3 otherObjectsPos, int polarization);

}