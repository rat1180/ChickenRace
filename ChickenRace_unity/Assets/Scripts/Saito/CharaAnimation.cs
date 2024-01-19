using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaAnimation : MonoBehaviour
{
    public static CharaAnimation instance;

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
        DEATH,
        DANCE,
        WIN,
    }

    [SerializeField] Animator animator;
    public Animations nowAnimations;

    void Start()
    {
        
    }

    void Update()
    {
        StartAnimation(nowAnimations);  // テスト用.
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
            case Animations.DEATH:
                animator.SetTrigger("DeathTrigger");
                break;
            case Animations.DANCE:

                break;
            case Animations.WIN:
                animator.SetTrigger("WinTrigger");
                break;
        }
    }
}
