
using System.Collections.Generic;
using UnityEngine;



public class PlayerBasicAttack : MonoBehaviour
{
    private PlayerBasicMovement playerBasicMovement;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerBasicMovement = GetComponent<PlayerBasicMovement>();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (Input.GetMouseButtonDown(0) && playerBasicMovement.CanAttack()) // Left mouse click
        {
            anim.SetTrigger("BasicAttack");
        }
    }

}
