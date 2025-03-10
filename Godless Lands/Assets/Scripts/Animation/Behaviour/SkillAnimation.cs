﻿using Animation;
using Animation.Data;
using Protocol.Data.Replicated.Animation;
using UnityEngine;
using Zenject;

public class SkillAnimation : StateMachineBehaviour
{
    [SerializeField]
    private AnimationID _animationID;
    [SerializeField]
    private string _multipler;
    private AnimationPlaybackTimeBuffer _animationPlaybackTimeBuffer;
    private AnimationPriorityData _animationPriorityData;
    private float _startPlayTime;


    [Inject]
    private void Construct(AnimationPriorityData animationPriorityData)
    {
        _animationPriorityData = animationPriorityData;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(string.IsNullOrEmpty(_multipler)) return;

        _animationPlaybackTimeBuffer = animator.GetComponent<AnimationPlaybackTimeBuffer>();

        if (_animationPlaybackTimeBuffer != null && _animationPlaybackTimeBuffer.TryPullTime(_animationID, out float playTime))
        {
            Debug.Log($"[{_animationID}] OnStateEnter: " + playTime);
            _startPlayTime = Time.time;
            float animationMultiplier = stateInfo.length / playTime;
            animator.SetFloat(_multipler, animator.GetFloat(_multipler) * animationMultiplier);
        }
        else animator.SetFloat(_multipler, 1f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"[{_animationID}] OnStateExit:{Time.time - _startPlayTime}");
        int priority = _animationPriorityData.GetPriority(_animationID);
        if(priority >= animator.GetInteger("Priority"))
        {
            animator.SetInteger("Priority", 0);
        }
    }
}
