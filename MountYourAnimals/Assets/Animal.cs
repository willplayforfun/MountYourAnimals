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
    [SerializeField]
    private float maximumAngularVelocity = 180;

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

    public virtual void Spawn()
    {
        PlaySpawnSound();
    }
	
	protected virtual void Update ()
    {
        // rotate based on key input
        myRb.AddTorque(-Input.GetAxis("Horizontal") * rotateForce);

        // cap angular velocity (spin speed)
        myRb.angularVelocity = Mathf.Clamp(myRb.angularVelocity, -maximumAngularVelocity, maximumAngularVelocity);
    }

    private GameObject latestHit;
    private Vector3 latestAnchor;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // when we run into a new grippable object, we anchor our joint in it
        if (collision.collider.GetComponentInParent<Animal>() != null || collision.collider.tag == "Grippable")
        {
            latestHit = collision.collider.gameObject;

            PlayMovementSound();

            Debug.Log("New collision with " + latestHit.name);
            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.5f, Color.red, 1);

            latestAnchor = transform.InverseTransformPoint(collision.contacts[0].point);
            myJoint.enabled = true;
            myJoint.anchor = latestAnchor;
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        // keep anchoring to the latest object we ran into, even as we move around it
        if (collision.collider.gameObject == latestHit)
        {
            // TODO handle new/multiple contact points

            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.3f, Color.green);

            Vector3 newAnchor = transform.InverseTransformPoint(collision.contacts[0].point);

            // play movement sound as we move
            if (Vector3.Distance(newAnchor, latestAnchor) > 0.3f)
            {
                Debug.DrawLine(newAnchor, latestAnchor, Color.magenta);
                PlayMovementSound();
            }

            // update anchor
            latestAnchor = newAnchor;
            myJoint.anchor = latestAnchor;

        }
        else
        {
            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.4f, Color.black);
        }
    }

    private void PlaySpawnSound()
    {
        AudioClip clip = spawnSounds[Random.Range(0, spawnSounds.Length)];
        myAudioSource.PlayOneShot(clip);
    }
    private void PlayMovementSound()
    {
        AudioClip clip = moveSounds[Random.Range(0, moveSounds.Length)];
        myAudioSource.PlayOneShot(clip);
    }
}
