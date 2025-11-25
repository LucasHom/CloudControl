using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewageExplosion : MonoBehaviour
{
    private Animator animator;
    public bool isVolumeControlled;

    private void Awake()
    {
        isVolumeControlled = false;
    }

    void Start()
    {
        if (!isVolumeControlled)
        {
            SFXManager.Instance.PlaySFX("explosion");
        }

        animator = GetComponent<Animator>();

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animLength);
    }
}
