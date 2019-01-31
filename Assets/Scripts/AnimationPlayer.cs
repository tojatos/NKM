using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Animations;
using Hex;
using NKMObjects.Templates;
using UnityEngine;

public class AnimationPlayer : SingletonMonoBehaviour<AnimationPlayer>
{
	private static readonly Queue<NkmAnimation> AnimationsToPlay = new Queue<NkmAnimation>();
	private static bool _canPlayNext = true;
	public static void Add(NkmAnimation animation)
	{
		try
		{
            AnimationsToPlay.Enqueue(animation);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	public void AddAnimationTriggers(Character character)
	{
        character.AfterAttack += (targetCharacter, damage) =>
            Add(new Tilt(HexMapDrawer.Instance.GetCharacterObject(targetCharacter).transform));
        character.AfterAttack += (targetCharacter, damage) =>
            Add(new ShowInfo(HexMapDrawer.Instance.GetCharacterObject(targetCharacter).transform, damage.Value.ToString(), Color.red));
        character.AfterHeal += (targetCharacter, valueHealed) =>
            Add(new ShowInfo(HexMapDrawer.Instance.GetCharacterObject(targetCharacter).transform, valueHealed.ToString(), Color.blue));
		character.OnDeath += () => Add(new Destroy(HexMapDrawer.Instance.GetCharacterObject(character)));

	}
//	public IEnumerator SharpenedForkEnumerator(Transform parentTransform, Transform targetTransform)
//	{
//		const float animationDuration = 0.5f;
//		var fork = Instantiate(Stuff.Prefabs.Single(o => o.name == "fork"), parentTransform);
//
//		//rotate fork towards target
//		var direction = (targetTransform.position - parentTransform.position);
//		var normalizedDirection = direction.normalized;
//		fork.transform.rotation = Quaternion.LookRotation(normalizedDirection);
//
//		fork.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f); //resize the fork
//
//		PositionParticle(fork);
//
//		if (direction.magnitude < 45) fork.transform.position -= normalizedDirection * 29; // throw the fork from behind if target is too close
//		var targetPosition = GetYPositioned(targetTransform.position) - normalizedDirection * 29; //subtraction to make the fork hit target by tip, not by middle
//
//		StartCoroutine(MoveToPosition(fork.transform, targetPosition, animationDuration-0.1f));
//		yield return new WaitForSeconds(animationDuration);
//
//		Destroy(fork);
//	}
//
//	private static Vector3 GetYPositioned(Vector3 position, float y = 20f)
//	{
//		return new Vector3(position.x, position.y + y, position.z);
//	}


//	private IEnumerator SonzaiNoChikaraEnumerator(List<Transform> targetTransforms)
//	{
//		const float animationTime = 4f;
//
//		var particles = new List<GameObject>();
//		foreach (var t in targetTransforms)
//		{
//			var particle = Instantiate(Stuff.Particles.Single(o => o.name == "Sonzai No Chikara"), t);
//
//			particles.Add(particle);
//
//			PositionParticle(particle);
//
//			var ps = particle.GetComponent<ParticleSystem>();
//			var shape = ps.shape;
//			StartCoroutine(DecreaseRadiusToZero(shape, animationTime));
//		}
//
//		yield return new WaitForSeconds(animationTime);
//
//		particles.ForEach(Destroy);
//		particles.Clear();
//		foreach (var t in targetTransforms)
//		{
//			var particle = Instantiate(Stuff.Particles.Single(o => o.name == "Sonzai No Chikara Boom"), t);
//			particles.Add(particle);
//			PositionParticle(particle);
//		}
//
//		yield return new WaitForSeconds(0.2f);
//
//		particles.ForEach(Destroy);
//	}
//	private static IEnumerator DecreaseRadiusToZero(ParticleSystem.ShapeModule shape, float time)
//	{
//		while (time > 0)
//		{
//			shape.radius -= shape.radius * 0.2f;
//			time -= 0.2f;
//			yield return new WaitForSeconds(0.2f);
//		}
//	}

	private async void Update()
	{
		if (AnimationsToPlay.Count <= 0 || !_canPlayNext) return;
		await PlayNextAnimation();
	}

	/// <summary>
	/// Dequeues and plays every animation part from the queue, consecutively.
	/// </summary>
	private static async Task PlayNextAnimation()
	{
		_canPlayNext = false;
		NkmAnimation a = AnimationsToPlay.Dequeue();
#pragma warning disable 4014
		if (a.AllowPlayingOtherAnimations) a.Play();
#pragma warning restore 4014
		else await a.Play();
		_canPlayNext = true;
	}
}