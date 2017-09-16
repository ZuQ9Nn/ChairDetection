using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanIK : MonoBehaviour
{
    public Transform leftLeg = null;
    public Transform rihttLeg = null;

    private Animator animator = null;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

        animator.SetIKPosition(AvatarIKGoal.RightFoot, leftLeg.position);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, leftLeg.rotation);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, rihttLeg.position);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, rihttLeg.rotation);
    }
}
