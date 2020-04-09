using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core {
    public abstract class SceneTransition : MonoBehaviour {
        [SerializeField] float transitionDuration = .5f;
        [SerializeField] float beforeFadeOutStallDuration = 0f;

        public float TransitionDuration => transitionDuration;
        public float BeforeFadeStallDuration => beforeFadeOutStallDuration;

        public abstract void FadeIn();
        public abstract void FadeOut();
    }
}
