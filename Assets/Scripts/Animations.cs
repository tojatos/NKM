using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class Animations : SingletonMonoBehaviour<Animations>
{
	private bool IsAsterPlaying;
	public IEnumerator SharpenedForkEnumerator(Transform parentTransform, Transform targetTransform)
	{
		const float animationDuration = 0.5f;
		var fork = Instantiate(Stuff.Prefabs.Single(o => o.name == "fork"), parentTransform);

		//rotate fork towards target
		var direction = (targetTransform.position - parentTransform.position);
		var normalizedDirection = direction.normalized;
		fork.transform.rotation = Quaternion.LookRotation(normalizedDirection);

		fork.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f); //resize the fork

		PositionParticle(fork);

		if (direction.magnitude < 45) fork.transform.position -= normalizedDirection * 29; // throw the fork from behind if target is too close
		var targetPosition = GetYPositioned(targetTransform.position) - normalizedDirection * 29; //subtraction to make the fork hit target by tip, not by middle

		StartCoroutine(MoveToPosition(fork.transform, targetPosition, animationDuration-0.1f));
		yield return new WaitForSeconds(animationDuration);

		Destroy(fork);
	}

	private static Vector3 GetYPositioned(Vector3 position, float y = 20f)
	{
		return new Vector3(position.x, position.y + y, position.z);
	}
	private IEnumerator ItadakiNoKuraEnumerator(Transform parentTransform, Transform targetTransform)
	{
		if(IsAsterPlaying) yield return new WaitUntil(()=>!IsAsterPlaying);

		var particleStartSize = 20f;
		var animationDuration = 3.5f;

		var particle = Instantiate(Stuff.Particles.Single(o => o.name == "Itadaki No Kura"), targetTransform);

		var pos = particle.transform.localPosition;
		pos.z = -20;
		particle.transform.localPosition = pos;

		var main = particle.GetComponent<ParticleSystem>().main;
		main.startSize = new ParticleSystem.MinMaxCurve(particleStartSize);
		StartCoroutine(MoveToPosition(particle.transform, parentTransform.position, animationDuration-1f));
		yield return new WaitForSeconds(animationDuration);

		Destroy(particle);
	}
	public void ItadakiNoKura(Transform parentTransform, Transform targetTransform)
	{
		StartCoroutine(ItadakiNoKuraEnumerator(parentTransform, targetTransform));
	}

	private IEnumerator AsterYoEnumerator(Transform parentTransform, List<Transform> targetTransforms)
	{
		IsAsterPlaying = true;
		const float particleSecondSize = 10f;

		var particlesWithTargets = new Dictionary<GameObject, Transform>();
		foreach (var t in targetTransforms)
		{
			var particle = Instantiate(Stuff.Particles.Single(o => o.name == "Aster Yo"), parentTransform);

			particlesWithTargets.Add(particle, t);
			PositionParticle(particle);
		}

		yield return new WaitForSeconds(2f);

		foreach (var pair in particlesWithTargets)
		{
			var particle = pair.Key;
			var t = pair.Value;

			var main = particle.GetComponent<ParticleSystem>().main;
			main.startSize = new ParticleSystem.MinMaxCurve(particleSecondSize);
			StartCoroutine(MoveToPosition(particle.transform, t.position, 0.1f));
		}

		yield return new WaitForSeconds(1.5f);

		particlesWithTargets.ToList().ForEach(pair => Destroy(pair.Key));
		IsAsterPlaying = false;
	}
	public void AsterYo(Transform parentTransform, List<Transform> targetTransforms)
	{
		StartCoroutine(AsterYoEnumerator(parentTransform, targetTransforms));
	}
	private IEnumerator SonzaiNoChikaraEnumerator(List<Transform> targetTransforms)
	{
		const float animationTime = 4f;

		var particles = new List<GameObject>();
		foreach (var t in targetTransforms)
		{
			var particle = Instantiate(Stuff.Particles.Single(o => o.name == "Sonzai No Chikara"), t);

			particles.Add(particle);

			PositionParticle(particle);

			var ps = particle.GetComponent<ParticleSystem>();
			var shape = ps.shape;
			StartCoroutine(DecreaseRadiusToZero(shape, animationTime));
		}

		yield return new WaitForSeconds(animationTime);

		particles.ForEach(Destroy);
		particles.Clear();
		foreach (var t in targetTransforms)
		{
			var particle = Instantiate(Stuff.Particles.Single(o => o.name == "Sonzai No Chikara Boom"), t);
			particles.Add(particle);
			PositionParticle(particle);
		}

		yield return new WaitForSeconds(0.2f);

		particles.ForEach(Destroy);
	}
	//public void SonzaiNoChikara(List<Transform> targetTransforms)
	//{
	//	StartCoroutine(SonzaiNoChikaraEnumerator(targetTransforms));
	//	//StartCoroutine("SonzaiNoChikaraEnumerator", targetTransforms);
	//}
	private static void PositionParticle(GameObject particle)
	{
		var pos = particle.transform.localPosition;
		pos.z = -20;
		particle.transform.localPosition = pos;
	}
	private static IEnumerator DecreaseRadiusToZero(ParticleSystem.ShapeModule shape, float time)
	{
		while (time > 0)
		{
			shape.radius -= shape.radius * 0.2f;
			time -= 0.2f;
			yield return new WaitForSeconds(0.2f);
		}
	}
	private static IEnumerator MoveToPosition(Transform trans, Vector3 endPos, float timeToMove)
	{
		var currentPos = trans.position;
		var t = 0f;
		while (t < 1)
		{
			t += Time.deltaTime / timeToMove;
			trans.position = Vector3.Lerp(currentPos, endPos, t);
			yield return null;
		}
	}

	//public void Enqueue(IEnumerator routine)
	//{
	//	//StartCoroutine(animationName, args);
	//	StartCoroutine()
	//}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			AsterYo(Active.Instance.CharacterOnMap.CharacterObject.transform, GameManager.Players.SelectMany(p => p.Characters.Select(c => c.CharacterObject.transform)).ToList());
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ItadakiNoKura(Active.Instance.CharacterOnMap.CharacterObject.transform, GameManager.Players.SelectMany(p => p.Characters).Where(c => c.Owner != Active.Instance.CharacterOnMap.Owner).Select(c => c.CharacterObject.transform).First());
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			StartCoroutine(SharpenedForkEnumerator(Active.Instance.CharacterOnMap.CharacterObject.transform, GameManager.Players.SelectMany(p => p.Characters).Where(c => c.Owner != Active.Instance.CharacterOnMap.Owner).Select(c => c.CharacterObject.transform).First()));
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			StartCoroutine(SonzaiNoChikaraEnumerator(GameManager.Players.SelectMany(p => p.Characters.Select(c => c.CharacterObject.transform)).ToList()));
		}
	}
}