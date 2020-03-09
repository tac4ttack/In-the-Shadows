using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ITS.ParticleGarbageCollector
{
    public class ParticleGarbageCollector : MonoBehaviour
    {
        private List<ParticleSystem> _Emitters;

        void Awake()
        {
            // DEBUG
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"PARTICLE GARBAGE COLLECTOR - {this.name} - Awake()");
            #endif
            
            _Emitters = new List<ParticleSystem>(this.gameObject.GetComponentsInChildren<ParticleSystem>());
            Assert.IsNotNull(_Emitters, "No particle system found in children!");
            Assert.IsTrue((_Emitters.Count > 0), "Particle system array is empty!");
        }

        void Update()
        {
            for (int i = 0; i < _Emitters.Count; i++)
            {
                if (_Emitters[i] != null && !_Emitters[i].IsAlive(true))
                {
                    Destroy(_Emitters[i].gameObject);
                    _Emitters.RemoveAt(i);
                }
            }
            if (_Emitters.Count == 0)
                Destroy(this.gameObject);
        }
    }
}