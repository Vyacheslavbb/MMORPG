﻿using Network.Replication;
using Protocol.Data.Replicated;
using Protocol.Data.Replicated.Animation;
using System;
using UnityEngine;

namespace Animation
{
    internal class AnimationStateDataHandler : MonoBehaviour, INetworkDataHandler
    {
        public AnimationStateID ActiveStateID { get; private set; }
        public bool IsInitialized { get; private set; } = false;

        public event Action<AnimationStateID> OnAnimationStateChange;

        public void UpdateData(IReplicationData updatedData)
        {
            AnimationStateData data = (AnimationStateData)updatedData;
            ActiveStateID = data.AnimationStateID;
            OnAnimationStateChange?.Invoke(data.AnimationStateID);
            IsInitialized = true;
        }
    }
}
