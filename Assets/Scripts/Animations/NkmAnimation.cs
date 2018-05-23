using System.Collections;
using UnityEngine;

namespace Animations
{
    public abstract class NkmAnimation  
    {
        public abstract IEnumerator Play();

//        protected static IEnumerator MoveToPosition(Transform trans, Vector3 endPos, float timeToMove)
//        {
//            var currentPos = trans.position;
//            var t = 0f;
//            while (t < 1)
//            {
//                t += Time.deltaTime / timeToMove;
//                trans.position = Vector3.Lerp(currentPos, endPos, t);
//                yield return null;
//            }
//        }
//        protected static void PositionParticle(GameObject particle)
//	{
//		var pos = particle.transform.localPosition;
//		pos.z = -20;
//		particle.transform.localPosition = pos;
//	}
    }
}