using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Boxfriend.Utils
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        private ObjectPoolCircular<AudioSource> _sources;

        [SerializeField] private AudioMixerGroup _audioMixer;
        [SerializeField] private AudioSource _sourcePrefab;

        private const string _inWaitingName = "AudioManager - Ready";
        private void Awake () => _sources = new ObjectPoolCircular<AudioSource>(Create, x => x.enabled = true, ReturnSource, DestroySource, 32);

        private AudioSource Create ()
        {
            AudioSource source;
            if (_sourcePrefab == null)
            {
                var go = new GameObject
                {
                    name = _inWaitingName
                };
                go.transform.parent = transform;
                source = go.AddComponent<AudioSource>();
            } else
            {
                source = Instantiate(_sourcePrefab, Vector3.zero, Quaternion.identity);
            }

            source.outputAudioMixerGroup = _audioMixer;
            source.enabled = false;
            return source;
        }

        private AudioSource GetSource (string clipName, Vector3 position)
        {
            var source = _sources.FromPool();
            source.name = $"AudioManager - Playing: {clipName}";
            source.transform.position = position;

            return source;
        }
        private void ReturnSource (AudioSource source)
        {
            source.name = _inWaitingName;
            source.clip = null;
#if UNITY_2023_2_OR_NEWER
            source.resource = null;
#endif
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

#if !UNITY_2023_2_OR_NEWER
        public void Play (AudioClip clip) => Play(clip, Vector3.zero);
        public void Play (AudioClip clip, Vector3 position)
        {
            var source = GetSource(clip.name, position);
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnWhenDone(source));
        }
#else
        public void Play (AudioResource resource) => Play(resource, Vector3.zero);
        public void Play(AudioResource resource, Vector3 position)
        {
            var source = GetSource(resource.name, position);
            source.resource = resource;
            source.Play();
            StartCoroutine(ReturnWhenDone(source));
        }
#endif

        private IEnumerator ReturnWhenDone (AudioSource source)
        {
            yield return new WaitUntil(() => !source.isPlaying);
            _sources.ToPool(source);
        }
    }

}
