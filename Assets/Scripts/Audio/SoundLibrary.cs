using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    public class SoundLibrary : MonoBehaviour
    {
        [SerializeField] SoundGroup[] soundGroups;

        private readonly Dictionary<string, AudioClip[]> _groupDictionary = new Dictionary<string, AudioClip[]>();

        private void Awake()
        {
            foreach (SoundGroup soundGroup in soundGroups)
                _groupDictionary.Add(soundGroup.groupID, soundGroup.group);
        }

        public AudioClip GetClipFromName(string clipName)
        {
            if(_groupDictionary.TryGetValue(clipName, out var sounds))
            {
                return sounds[Random.Range(0, sounds.Length)];
            }
            return null;
        }

        [Serializable]
        public class SoundGroup
        {
            public string groupID;
            public AudioClip[] group;
        }
    }
}