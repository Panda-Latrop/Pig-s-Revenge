using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationComponent : ActorComponent, ISaveableComponent
{
    [SerializeField]
    protected Animator animator;
    public Animator Animator { get => animator; }

    protected void OnEnable()
    {
        animator.enabled = true;
    }
    protected void OnDisable()
    {
        animator.enabled = false;
    }

    public void ChanegeController(RuntimeAnimatorController animatorController)
    {
        animator.runtimeAnimatorController = animatorController;
    }
    public void Play(string animation)
    {
        animator.Play(animation);
    }
    public void Play(int animationHash, int layer = 0)
    {
        animator.Play(animationHash, layer);     
    }
    public float GetLayerWeight(int layer)
    {
      return animator.GetLayerWeight(layer);
    }
    public void SetLayerWeight(int layer,float weight)
    {
        animator.SetLayerWeight(layer, weight);
    }
    public abstract void AnimatorMessage(string message, int value = 0);

}
