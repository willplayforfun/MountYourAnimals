using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Animal : MonoBehaviour
{
    protected Rigidbody2D myRb;
    protected HingeJoint2D myJoint;
    private FixedJoint2D myPermanentJoint;
    private FixedJoint2D myHumanJoint;

    [SerializeField]
    private float rotateForce = 10;
    [SerializeField]
    private float maximumAngularVelocity = 180;

    [Space(12)]

    [SerializeField]
    private AudioClip[] spawnSounds;
    [SerializeField]
    private AudioClip[] moveSounds;
    [SerializeField]
    private AudioClip[] freezeSounds;
    protected AudioSource myAudioSource;

    [Space(12)]

    public Sprite uiSprite;

    [Space(12)]

    [SerializeField]
    private GameObject[] gibs;
    [SerializeField]
    private int numberOfGibs;
    [SerializeField]
    private float explodeForce;
    [SerializeField]
    private float maximumGibSpin;

    private List<Animal> attachedAnimals = new List<Animal>();

    void Awake ()
    {
        myRb = GetComponent<Rigidbody2D>();
        myJoint = GetComponent<HingeJoint2D>();
        myJoint.enabled = false;
        myHumanJoint = this.gameObject.AddComponent<FixedJoint2D>();
        myHumanJoint.enabled = false;
        myPermanentJoint = GetComponent<FixedJoint2D>();
        //myPermanentJoint = this.gameObject.AddComponent<FixedJoint2D>();
        myPermanentJoint.enabled = false;
        myAudioSource = GetComponent<AudioSource>();
    }

    private bool isFirstAnimal;

    public virtual void Spawn(bool first)
    {
        isFirstAnimal = first;
        beingControlled = true;

        // TODO animate in

        // make the camera follow us
        EnableCameraFocus();

        PlaySpawnSound();

        movePromptCoroutine = StartCoroutine(MovePromptRoutine());
        GameManager.Instance.abilityPrompt.SetActive(true);

        if(isFirstAnimal)
        {
            GameManager.Instance.freezePrompt.SetActive(true);
        }

        //TODO set ability prompt to show specific animal ability
    }
    protected Coroutine movePromptCoroutine;
    private IEnumerator MovePromptRoutine()
    {
        yield return new WaitForSeconds(3);
        GameManager.Instance.movePrompt.SetActive(true);
        movePromptCoroutine = null;
    }

    protected virtual void Freeze()
    {
        if (latestHit != null || touchingGround)
        {
            beingControlled = false;

            // freeze the animal
            if (isFirstAnimal || (touchingGround && latestHit == null))
            {
                myRb.bodyType = RigidbodyType2D.Static;
            }
            // use fixed joint/spring joint instead for wobbly tower
            else
            {
                if(latestHit != null)
                {
                    myJoint.enabled = false;
                    myPermanentJoint.enabled = true;
                    myPermanentJoint.connectedBody = latestHit.GetComponentInParent<Rigidbody2D>();
                    myPermanentJoint.anchor = latestAnchor;
                    myPermanentJoint.connectedAnchor = latestHit.transform.InverseTransformPoint(this.transform.TransformPoint(latestAnchor));

                    if (latestHit.GetComponentInParent<Animal>() != null)
                    {
                        latestHit.GetComponentInParent<Animal>().attachedAnimals.Add(this);
                    }
                }
            }

            // camera should stop following us
            DisableCameraFocus();

            PlayFreezeSound();

            if (movePromptCoroutine != null)
            {
                StopCoroutine(movePromptCoroutine);
            }
            GameManager.Instance.movePrompt.SetActive(false);
            GameManager.Instance.freezePrompt.SetActive(false);
            GameManager.Instance.grabPrompt.SetActive(false);
            GameManager.Instance.abilityPrompt.SetActive(false);

            // let the game know the player has frozen the animal
            GameManager.Instance.CurrentAnimalFrozen();
        }
    }

    protected bool beingControlled;
    protected bool humanHasBeenGrabbed;

	protected virtual void Update ()
    {
        if (beingControlled)
        {
            // rotate based on key input
            myRb.AddTorque(-Input.GetAxis("Horizontal") * rotateForce);

            if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
            {
                if (movePromptCoroutine != null)
                {
                    StopCoroutine(movePromptCoroutine);
                }
                GameManager.Instance.movePrompt.SetActive(false);
            }

            // cap angular velocity (spin speed)
            myRb.angularVelocity = Mathf.Clamp(myRb.angularVelocity, -maximumAngularVelocity, maximumAngularVelocity);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.abilityPrompt.SetActive(false);

                DoAbility();
            }
            // check for grabbing human
            if(Input.GetKey(KeyCode.F) && !humanHasBeenGrabbed)
            {
                if (humanHit != null)
                {
                    humanHasBeenGrabbed = true;

                    GameManager.Instance.grabPrompt.SetActive(false);

                    // stick to human, unstick human from previous grab
                    StickToHuman();
                }
            }
            // check for freezing animal
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Freeze();
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                latestHit = null;
                myJoint.enabled = false;
            }
        }
    }

    private bool touchingGround;

    private GameObject humanHit;
    private Vector3 humanAnchor;

    protected GameObject latestHit;
    private Vector3 latestAnchor;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (beingControlled)
        {
            // when we run into a new grippable object, we anchor our joint in it
            if (collision.collider.GetComponentInParent<Animal>() != null || collision.collider.tag == "Grippable")
            {
                if (collision.contacts.Length > 0)
                {
                    latestHit = collision.collider.gameObject;

                    PlayMovementSound();
                    GameManager.Instance.freezePrompt.SetActive(true);

                    Debug.Log("New collision with " + latestHit.name);

                    Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + collision.contacts[0].normal * 0.5f, Color.red, 1);

                    latestAnchor = transform.InverseTransformPoint(collision.contacts[0].point);
                    myJoint.enabled = true;
                    myJoint.connectedBody = latestHit.GetComponent<Rigidbody2D>();
                    myJoint.anchor = latestAnchor;
                }
            }

            // if we run into the human, store it so we can attach when the player presses the attach button
            CheckHumanCollision(collision);

            if (!myJoint.enabled)
            {
                PlayMovementSound();
            }

            if(collision.collider.gameObject.tag == "Ground")
            {
                touchingGround = true;
                GameManager.Instance.freezePrompt.SetActive(true);

                myJoint.enabled = false;
                latestHit = null;
            }
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (beingControlled)
        {
            // keep anchoring to the latest object we ran into, even as we move around it
            if (collision.collider.gameObject == latestHit)
            {
                // TODO handle new/multiple contact points

                Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + collision.contacts[0].normal * 0.3f, Color.green);

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
                CheckHumanCollision(collision);

                Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + collision.contacts[0].normal * 0.4f, Color.black);
            }
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (beingControlled)
        {
            if (collision.collider.gameObject == latestHit)
            {
                Debug.LogWarning("Animal stopped colliding with latest hit, that shouldn't happen");
                //latestHit = null;
                //myJoint.enabled = false;
                //GameManager.Instance.freezePrompt.SetActive(false);
            }
            if (collision.collider.gameObject == humanHit && !humanHasBeenGrabbed)
            {
                humanHit = null;
                GameManager.Instance.grabPrompt.SetActive(false);
            }

            if (collision.collider.gameObject.tag == "Ground")
            {
                touchingGround = false;
            }
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
    protected void PlayMovementSound()
    {
        if (moveSounds.Length > 0)
        {
            AudioClip clip = moveSounds[Random.Range(0, moveSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }
    }
    protected void PlayFreezeSound()
    {
        if (freezeSounds.Length > 0)
        {
            AudioClip clip = freezeSounds[Random.Range(0, freezeSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }
    }

    protected void CheckHumanCollision(Collision2D collision)
    {
        if (!humanHasBeenGrabbed && collision.collider.tag == "Guy")
        {
            if (collision.contacts.Length > 0)
            {
                humanHit = collision.collider.gameObject;
                humanAnchor = transform.InverseTransformPoint(collision.contacts[0].point);

                GameManager.Instance.grabPrompt.SetActive(true);

                //Debug.Log("New collision with " + humanHit.name);
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

    public void EnableCameraFocus()
    {
        FindObjectOfType<Camera2D>().AddFocus(this.GetComponent<GameEye2D.Focus.Focus2D>());
    }
    public void DisableCameraFocus()
    {
        FindObjectOfType<Camera2D>().RemoveFocus(this.GetComponent<GameEye2D.Focus.Focus2D>());
    }

    public void SetMouseAbilityActivation(bool on)
    {
        allowMouseActivation = on;
    }

    protected virtual void DoAbility()
    {
        Debug.Log(this.gameObject.name + " did its ability");
    }

    private bool allowMouseActivation;
    private void OnMouseDown()
    {
        Debug.Log("clicked on " + this.gameObject.name);
        if(allowMouseActivation)
        {
            DoAbility();
        }
    }

    public void Explode()
    {
        Debug.Log(this.gameObject.name + " exploded");

        foreach(Animal a in attachedAnimals)
        {
            if(a != null)
            {
                DetachFromAnimal();
            }
        }

        for(int i = 0; i < numberOfGibs; i++)
        {
            GameObject gib = Instantiate(gibs[Random.Range(0, gibs.Length)], transform.position, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward));
            gib.GetComponent<Rigidbody2D>().velocity = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.left * explodeForce * Random.Range(0.8f, 1.2f);
            gib.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-maximumGibSpin, maximumGibSpin);
        }

        Destroy(this.gameObject);
    }

    private void DetachFromAnimal()
    {
        myPermanentJoint.enabled = false;
    }
}
