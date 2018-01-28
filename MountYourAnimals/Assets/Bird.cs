using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : Animal
{
    [Space(12)]
    [SerializeField]
    private float upForce;
    [SerializeField]
    private float horizontalForce;

    protected override void DoAbility()
    {
        base.DoAbility();

        if(humanHasBeenGrabbed)
        {
            myRb.AddForce(Vector2.up * upForce * 0.2f, ForceMode2D.Impulse);
        }
        else
        {
            myRb.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
        }
    }

    protected override void Update()
    {
        base.Update();

        if(beingControlled)
        {
            myRb.AddForce(Vector2.right * Input.GetAxis("Horizontal") * horizontalForce, ForceMode2D.Force);
        }
    }

    public override void Spawn(bool first)
    {
        base.Spawn(first);

        GameManager.Instance.freezePrompt.SetActive(true);
    }
    protected override void Freeze()
    {
        beingControlled = false;

        // freeze in mid-air
        myRb.bodyType = RigidbodyType2D.Static;

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

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //base.OnCollisionEnter2D(collision);
        if (beingControlled)
        {
            PlayMovementSound();
            CheckHumanCollision(collision);
        }

    }
    protected override void OnCollisionStay2D(Collision2D collision)
    {
        //base.OnCollisionStay2D(collision);
        if (beingControlled)
        {
            CheckHumanCollision(collision);
        }
    }
}
