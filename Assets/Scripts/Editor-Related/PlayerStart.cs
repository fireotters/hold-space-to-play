using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {
        // Spawns a player.
        if (player != null)
        {
            Instantiate(player, gameObject.transform.position, Quaternion.identity);
        }
        else // This error shouldn't appear, but it's better to be safe than sorry.
        {
            Debug.LogError("You must set the Player prefab in the Inspector!");
        }

        Destroy(gameObject);
    }

}
