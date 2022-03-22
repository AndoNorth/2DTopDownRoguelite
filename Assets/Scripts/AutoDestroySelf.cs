using UnityEngine;

// attach this to particle effects which you instantiate as gameObjects,
// this will destroy the gameObject once all the particles are gone
public class AutoDestroySelf : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
