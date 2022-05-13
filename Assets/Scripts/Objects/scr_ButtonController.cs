using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class scr_ButtonController : MonoBehaviour
{
    public Animator animator;
    public bool pressed;

    [SerializeField] UnityEvent ButtonPressed;

    // Start is called before the first frame update
    public void Press()
    {
        animator.SetTrigger("Pressed");
        ButtonPressed.Invoke();
    }
    public void StopAnimation()
    {
        animator.SetBool("Finished", true);
    }
}
