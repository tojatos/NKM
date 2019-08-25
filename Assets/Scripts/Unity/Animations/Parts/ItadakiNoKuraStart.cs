using System.Collections;
using System.Linq;
using Unity.Extensions;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class ItadakiNoKuraStart : NkmAnimationPart
    {
        private const float ParticleStartSize = 20f;
        public readonly GameObject Particle;
        
        public ItadakiNoKuraStart(Transform targetTransform)
        {
            Particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Itadaki No Kura"), targetTransform.position, targetTransform.rotation);
            Particle.Hide();
        }
        
        public override IEnumerator Play()
        {
            Particle.transform.localPosition += new Vector3(0,20,0);
            ParticleSystem.MainModule main = Particle.GetComponent<ParticleSystem>().main;
            main.startSize = new ParticleSystem.MinMaxCurve(ParticleStartSize);
            Particle.Show();
            
            IsFinished = true;
            yield break; 
        }
    }
}
