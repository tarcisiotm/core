using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TG.Core.Audio{
    public class AudioManager : Singleton<AudioManager>{
        PoolingManager poolingManager;

        [SerializeField] GameObject audioPrefab;

        PlayAudioAndDisable plauAudioAndDisable;

        List<AudioBase> audioList = new List<AudioBase>();

        bool isReady = false;

        IEnumerator Start()
        {
            while(PoolingManager.I  == null){
                yield return null;
            }
            poolingManager = PoolingManager.I;
            isReady = true;
        }

        /// <summary>
        /// Creates a one shot sound that will follow target transform
        /// </summary>
        /// <param name="clip">The audio clip</param>
        /// <param name="targetTransform">The transform</param>
        /// <param name="vol">The volume</param>
        public void CreateOneShotFollowTarget(AudioClip clip, Transform targetTransform, float vol) {
            plauAudioAndDisable = poolingManager.GetPooledObject(audioPrefab).GetComponent<PlayAudioAndDisable>();

            if (plauAudioAndDisable == null) {
                Debug.LogWarning("No component source was found...");
                return;
            }

            //Add it to list in order to properly pause the game
            plauAudioAndDisable.gameObject.SetActive(true);

            plauAudioAndDisable.PlayAndDisable(clip, vol, targetTransform);
        }

        /// <summary>
        /// Creates a one shot sound that will be stationary
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="pos"></param>
        /// <param name="vol"></param>
        public void CreateOneShot(AudioClip clip, Vector3 pos, float vol = 1f){
            plauAudioAndDisable = poolingManager.GetPooledObject(audioPrefab).GetComponent<PlayAudioAndDisable>();

            if(plauAudioAndDisable == null){
                Debug.LogWarning("No component source was found...");
                return;
            }

            plauAudioAndDisable.transform.position = pos;

            //Add it to list in order to properly pause the game
            plauAudioAndDisable.gameObject.SetActive(true);
            plauAudioAndDisable.PlayAndDisable(clip, vol);
        }

        //TODO future sprint to add pause all, etc
        public void AddToAudioList(AudioBase p_audioObj) {
            audioList.Remove(p_audioObj);
        }

        public void RemoveFromAudioList(AudioBase p_audioObj){
            audioList.Remove(p_audioObj);
        }

    }
}