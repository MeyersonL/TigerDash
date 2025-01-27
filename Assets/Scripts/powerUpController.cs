using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUpController : MonoBehaviour {
    public AudioClip collectSound;
    public string type;
    private float rotation;

    void Start() {
        rotation = 0f;
    }

    void Update() {
        if (type == "Speed") transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        else if (type == "Jump") transform.rotation = Quaternion.Euler(-90f, rotation, 0f);
        else if (type == "Health") transform.rotation = Quaternion.Euler(0f, rotation, -90f);
        rotation += 0.5f;
    }

    private void OnTriggerEnter(Collider other) { 
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        Destroy(gameObject, 0f);
    }
}
