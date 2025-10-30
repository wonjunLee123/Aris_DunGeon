using UnityEngine;

public class Anima : MonoBehaviour
   
{
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()

    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetBool("Run", true);
        }

        else
        {
            anim.SetBool("Run", false);
        }

        }
    }
