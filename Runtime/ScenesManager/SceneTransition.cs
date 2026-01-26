using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace TG.Core {
    /// <summary>
    /// Defines a base for scene transitions. Automatically handled from Scenes Manager
    /// </summary>
    public abstract class SceneTransition : MonoBehaviour {
        [SerializeField] private float transitionDuration = .5f;
        [SerializeField] private float beforeFadeOutStallDuration = 0f;

        protected Tween fadeTween;

        public float TransitionDuration => transitionDuration;
        public float BeforeFadeStallDuration => beforeFadeOutStallDuration;

        protected virtual void OnEnable()
        {
            ScenesManager.OnTransitionFadedIn += OnFadedIn;
            ScenesManager.OnTransitionIsGoingToFadeOut += BeforeFadeOut;
            ScenesManager.OnSceneProgressUpdated += OnSceneProgressUpdated;
        }

        protected virtual void OnDisable()
        {
            ScenesManager.OnTransitionFadedIn -= OnFadedIn;
            ScenesManager.OnTransitionIsGoingToFadeOut -= BeforeFadeOut;
            ScenesManager.OnSceneProgressUpdated -= OnSceneProgressUpdated;
        }

        protected abstract void OnFadedIn();

        protected abstract void BeforeFadeOut();

        protected virtual void OnSceneProgressUpdated(float progress) { }

        public abstract void FadeIn();
        public abstract void FadeOut();
    }
}