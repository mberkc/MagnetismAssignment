using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Restarts the scene
    public void Restart () {
        SceneManager.LoadScene (0);
    }
}