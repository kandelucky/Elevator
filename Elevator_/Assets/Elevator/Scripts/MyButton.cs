using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyButton : MonoBehaviour
{
    public enum ButtonResets { ByClick, ByTimer, ByBool };
    public ButtonResets buttonResets;

    bool buttonIsActive = false;

    public float timeForReset = 0.5f;

    
    public Camera cam;
    [SerializeField]
    private float minInteractionDistance;

    [SerializeField]
    private Transform buttonOnObj;
    [SerializeField]
    private Transform buttonOffObj;
    
    [SerializeField]
    private AudioSource sound;
    [SerializeField]
    private AudioClip buttonSound;
    [SerializeField]
    private AudioClip buttonErrorSound;

    [HideInInspector]
    public bool buttonEventIsDisabled = false;

    private Animator anim;

    public UnityEvent myEvent = new UnityEvent();


    private void Awake()
    {
        anim = GetComponent<Animator>();
        DeactivateButton();
    }

    private void OnMouseDown()
    {
        if (Mathf.Abs(Vector3.Distance(this.transform.position, cam.transform.position)) <= minInteractionDistance)
        {
            if (!buttonIsActive) ButtonIsDown();
        }
    }

    public void ButtonIsDown()
    {
        anim.Play("Activated");
        switch (buttonResets)
        {
            case ButtonResets.ByClick:
                GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
                if (!buttonIsActive) ActivateButton();
                else DeactivateButton();
                break;
            case ButtonResets.ByTimer:
                if (!buttonIsActive )
                {
                    if (!buttonEventIsDisabled) GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
                    else GetComponent<AudioSource>().PlayOneShot(buttonErrorSound, 0.7f);
                    StartCoroutine(OnOffByTimer());
                }
                else GetComponent<AudioSource>().PlayOneShot(buttonErrorSound, 0.7f);
                break;
            case ButtonResets.ByBool:
                if (!buttonIsActive)
                {
                    GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
                    ActivateButton();
                }
                else GetComponent<AudioSource>().PlayOneShot(buttonErrorSound, 0.7f);
                break;
        }
    }
   
    IEnumerator OnOffByTimer()
    {
        ActivateButton();
        yield return new WaitForSeconds (timeForReset);
        DeactivateButton();
    }
    public void ActivateButton()
    {
        buttonOnObj.gameObject.SetActive(true);
        buttonOffObj.gameObject.SetActive(false);
        buttonIsActive = true;
        if (!buttonEventIsDisabled) myEvent.Invoke();
    }

    public void DeactivateButton()
    {
        buttonOnObj.gameObject.SetActive(false);
        buttonOffObj.gameObject.SetActive(true);
        buttonIsActive = false;
    }
    public void ResetBool()
    {
        DeactivateButton();
    }
}
