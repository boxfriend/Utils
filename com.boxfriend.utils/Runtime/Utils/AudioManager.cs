using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Boxfriend.Utils
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        private ObjectPoolCircular<AudioSource> _sources;

        [SerializeField] private AudioMixerGroup _audioMixer;

        private const string _inWaitingName = "AudioManager - Ready";
        private void Awake() => _sources = new ObjectPoolCircular<AudioSource>(Create, x => x.enabled = true, ReturnSource, DestroySource, 32);

        private AudioSource Create()
        {
            var go = new GameObject
            {
                name = _inWaitingName
            };
            go.transform.parent = transform;
            var source = go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _audioMixer;
            source.enabled = false;
            return source;
        }

        private AudioSource GetSource(string clipName, Vector3 position)
        {
            var source = _sources.FromPool();
            source.name = $"AudioManager - Playing: {clipName}";
            source.transform.position = position;

            return source;
        }
        private void ReturnSource(AudioSource source)
        {
            source.name = _inWaitingName;
            source.clip = null;
            source.enabled = false;
        }
        private void DestroySource (AudioSource source) => Destroy(source.gameObject);

        public void PlayOneShot (AudioClip clip, float volume = 1f) => PlayOneShot(clip, Vector3.zero, volume);
        public void PlayOneShot (AudioClip clip, Vector3 position, float volume = 1f)
        {
            var source = GetSource(clip.name, position);
            source.PlayOneShot(clip, volume);
            StartCoroutine(ReturnWhenDone(source));
        }

        public void Play (AudioClip clip) => Play(clip, Vector3.zero);
        public void Play (AudioClip clip, Vector3 position)
        {
            var source = GetSource(clip.name, position);
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnWhenDone(source));
        }

        private IEnumerator ReturnWhenDone(AudioSource source)
        {
            yield return new WaitUntil(() => !source.isPlaying);
            _sources.ToPool(source);
        }
    }

}
