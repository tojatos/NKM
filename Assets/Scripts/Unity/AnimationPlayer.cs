using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NKMCore;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Animations;
using Unity.Hex;
using UnityEngine;

namespace Unity
{
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

		public void AddAnimationTriggers(HexMap map)
		{
			map.AfterSwap += tuple =>
			{
				Character firstCharacterToSwap = tuple.Item1;
				Character secondCharacterToSwap = tuple.Item2;
				Add(new MoveTo(HexMapDrawer.Instance.GetCharacterObject(firstCharacterToSwap).transform,
					Active.SelectDrawnCell(secondCharacterToSwap.ParentCell).transform.position, 0.4f));
				Add(new MoveTo(HexMapDrawer.Instance.GetCharacterObject(secondCharacterToSwap).transform,
					Active.SelectDrawnCell(firstCharacterToSwap.ParentCell).transform.position, 0.4f));
			};
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
}