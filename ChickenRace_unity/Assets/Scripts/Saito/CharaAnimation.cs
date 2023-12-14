using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaAnimation : MonoBehaviour
{
    public static CharaAnimation Instance;

    /// <summary>
    /// キャラクターのアニメーション用
    /// </summary>
    public enum Animations
    {
        IDLE,
        MOVE,
        JUMP,
        FALL,
        WALLSLIDING,
        DANCE,
        WIN,
    }

    [SerializeField] Animator animator;
    [SerializeField] Animations nowAnimations;

    void Start()
    {
        
    }

    void Update()
    {
        //StartAnimation(nowAnimations);  // テスト用.
    }

    /// <summary>
    /// キャラクターのアニメーション再生.
    /// </summary>
    public void StartAnimation(Animations animation)
    {
        switch (animation)
        {
            case Animations.IDLE:
                animator.SetTrigger("IdleTrigger");
                break;
            case Animations.MOVE:
                animator.SetTrigger("MoveTrigger");
                break;
            case Animations.JUMP:
                animator.SetTrigger("JumpTrigger");
                break;
            case Animations.FALL:
                animator.SetTrigger("FallTrigger");
                break;
            case Animations.WALLSLIDING:
                animator.SetTrigger("WallSlidingTrigger");
                break;
            case Animations.DANCE:

                break;
        }
    }
}
