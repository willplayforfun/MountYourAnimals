using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Animal : MonoBehaviour
{
    private Rigidbody2D myRb;
    private HingeJoint2D myJoint;
    private FixedJoint2D myHumanJoint;

    [SerializeField]
    private float rotateForce = 10;
    [SerializeField]
    private float maximumAngularVelocity = 180;

    private AudioSource myAudioSource;
    [SerializeField]
    private AudioClip[] spawnSounds;
    [SerializeField]
    private AudioClip[] moveSounds;
    [SerializeField]
    private AudioClip[] freezeSounds;

    void Awake ()
    {
        myRb = GetComponent<Rigidbody2D>();
        myJoint = GetComponent<HingeJoint2D>();
        myHumanJoint = GetComponent<FixedJoint2D>();
        myAudioSource = GetComponent<AudioSource>();
    }

    public virtual void Spawn()
    {
        beingControlled = true;

        // make the camera follow us
        FindObjectOfType<Camera2D>().AddFocus(this.GetComponent<GameEye2D.Focus.Focus2D>());

        PlaySpawnSound();
    }
    protected virtual void Freeze()
    {
        beingControlled = false;

        // freeze the animal
        myRb.bodyType = RigidbodyType2D.Static;
        // TODO use fixed joint/spring joint instead for wobbly tower

        // camera should stop following us
        FindObjectOfType<Camera2D>().RemoveFocus(this.GetComponent<GameEye2D.Focus.Focus2D>());

        PlayFreezeSound();

        // let the game know the player has frozen the animal
        GameManager.Instance.AnimalSpawner.CurrentAnimalFrozen();
    }

    private bool beingControlled;
    private bool humanHasBeenGrabbed;

	protected virtual void Update ()
    {
        if (beingControlled)
        {
            // rotate based on key input
            myRb.AddTorque(-Input.GetAxis("Horizontal") * rotateForce);

            // cap angular velocity (spin speed)
            myRb.angularVelocity = Mathf.Clamp(myRb.angularVelocity, -maximumAngularVelocity, maximumAngularVelocity);

            // check for grabbing human
            if(Input.GetKey(KeyCode.F) && !humanHasBeenGrabbed)
            {
                if (humanHit != null)
                {
                    humanHasBeenGrabbed = true;

                    // stick to human, unstick human from previous grab
                    StickToHuman();
                }
            }
            // check for freezing animal
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Freeze();
            }
        }
    }

    private GameObject humanHit;
    private Vector3 humanAnchor;

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

        // if we run into the human, store it so we can attach when the player presses the attach button
        CheckHumanCollision(collision);

        if (!myJoint.enabled)
        {
            PlayMovementSound();
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
        else if (!humanHasBeenGrabbed && collision.collider.tag == "Guy")
        {
            humanHit = collision.collider.gameObject;
            humanAnchor = transform.InverseTransformPoint(collision.contacts[0].point);
        }
        else
        {
            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].normal * 0.4f, Color.black);
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject == latestHit)
        {
            Debug.LogWarning("Animal stopped colliding with latest hit, that shouldn't happen");
            latestHit = null;
            myJoint.enabled = false;
        }
        if (collision.collider.gameObject == humanHit && !humanHasBeenGrabbed)
        {
            humanHit = null;
        }
    }

    private void PlaySpawnSound()
    {
        if (spawnSounds.Length > 0)
        {
            AudioClip clip = spawnSounds[Random.Range(0, spawnSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }
    }
    private void PlayMovementSound()
    {
        if (moveSounds.Length > 0)
        {
            AudioClip clip = moveSounds[Random.Range(0, moveSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }
    }
    private void PlayFreezeSound()
    {
        if (freezeSounds.Length > 0)
        {
            AudioClip clip = freezeSounds[Random.Range(0, freezeSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }
    }

    private void CheckHumanCollision(Collision2D collision)
    {
        if (!humanHasBeenGrabbed && collision.collider.tag == "Guy")
        {
            if (collision.contacts.Length > 0)
            {
                humanHit = collision.collider.gameObject;
                humanAnchor = transform.InverseTransformPoint(collision.contacts[0].point);

                Debug.Log("New collision with " + humanHit.name);
            }
            else
            {
                Debug.LogWarning("Collided with human, but no contact points existed?");
            }
        }
    }
    private void StickToHuman()
    {
        Debug.Log("sticking to human");

        // unstick human from previous animal
        humanHit.GetComponentInParent<Human>().UnstickFromCurrentAnimal();

        // stick to human
        myHumanJoint.enabled = true;
        myHumanJoint.anchor = humanAnchor;
        myHumanJoint.connectedBody = humanHit.GetComponentInParent<Rigidbody2D>();
        humanHit.GetComponentInParent<Human>().SetGrabbingAnimal(this);
    }
    public void UnstickFromHuman()
    {
        myHumanJoint.enabled = false;
    }
}
