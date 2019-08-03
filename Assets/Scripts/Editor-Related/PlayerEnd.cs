using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnd : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            TriggerEndLevel();
    }

    // TODO: Do something when level finishes (
    void TriggerEndLevel()
    {
        print("End of level reached.");
    }
}
