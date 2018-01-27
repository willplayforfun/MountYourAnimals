using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Animal : MonoBehaviour
{
    private Rigidbody2D myRb;
    private HingeJoint2D myJoint;

    [SerializeField]
    private float rotateForce = 10;

    private AudioSource myAudioSource;
    [SerializeField]
    private AudioClip[] spawnSounds;
    [SerializeField]
    private AudioClip[] moveSounds;

    void Start ()
    {
        myRb = GetComponent<Rigidbody2D>();
        myJoint = GetComponent<HingeJoint2D>();
        myAudioSource = GetComponent<AudioSource>();

        // make the camera follow us
        FindObjectOfType<Camera2D>().AddFocus(this.GetComponent<GameEye2D.Focus.Focus2D>());
    }

    public void Spawn()
    {
        PlaySpawnSound();
    }
	
	void Update ()
    {
        // rotate based on key input
        myRb.AddTorque(-Input.GetAxis("Horizontal") * rotateForce);
    }

    private GameObject latestHit;
    private Vector3 latestAnchor;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // when we run into a new grippable object, we anchor our joint in it
        if (collision.collider.GetComponentInParent<Animal>() != null || collision.collider.tag == "Grippable")
        {
            latestHit = collision.collider.gameObject;

            Debug.Log("New collision with " + latestHit.name);
            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.5f, Color.red, 1);

            myJoint.anchor = transform.InverseTransformPoint(collision.contacts[0].point);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // keep anchoring to the latest object we ran into, even as we move around it
        if (collision.collider.gameObject == latestHit)
        {
            // TODO handle new/multiple contact points

            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.3f, Color.green);

            // play movement sound as we move
            if (Vector3.Distance(collision.contacts[0].point, latestAnchor) > 0.1f)
            {
                PlayMovementSound();
            }

            // update anchor
            latestAnchor = transform.InverseTransformPoint(collision.contacts[0].point);
            myJoint.anchor = latestAnchor;

        }
        else
        {
            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.4f, Color.black);
        }
    }

    private void PlaySpawnSound()
    {
        // TODO
    }
    private void PlayMovementSound()
    {
        // TODO
    }
}
