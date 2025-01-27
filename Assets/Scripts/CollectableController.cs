using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour {
    public AudioClip collectSound;
    public string type;
    private float rotation;

    void Start() {
        rotation = 0f;
    }

    void Update() {
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        rotation += 0.5f;
    }

    private void OnTriggerEnter(Collider other) { 
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        Destroy(gameObject, 0f);
    }
}
