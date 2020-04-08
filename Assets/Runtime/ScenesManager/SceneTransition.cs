using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core {
    public abstract class SceneTransition : MonoBehaviour {
        [SerializeField] float transitionDuration = .5f;

        public float TransitionDuration => transitionDuration;

        public abstract void FadeIn();
        public abstract void FadeOut();
    }
}
