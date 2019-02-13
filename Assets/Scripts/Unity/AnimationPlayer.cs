﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore;
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

		public void AddAnimationTriggers(Ability ability)
		{
			if (ability is NKMCore.Abilities.Hecate.AsterYo)
			{
				((NKMCore.Abilities.Hecate.AsterYo) ability).BeforeAsterBlaster += (character, characters) => 
                    Add(new AsterYo(
                        HexMapDrawer.Instance.GetCharacterObject(character).transform,
                        characters.Select(c => HexMapDrawer.Instance.GetCharacterObject(c).transform).ToList()
                    ));
			}

			if (ability is NKMCore.Abilities.Itsuka_Kotori.CamaelMegiddo)
			{
				((NKMCore.Abilities.Itsuka_Kotori.CamaelMegiddo) ability).BeforeFlamewave += (lineCells, conflargationCells) => 
                    Add(new CamaelMegiddo(
                        lineCells.Select(c => Active.SelectDrawnCell(c).transform).ToList(),
                        conflargationCells.Select(c => Active.SelectDrawnCell(c).transform).ToList()
                    ));
			}
			if (ability is NKMCore.Abilities.Levi.SwordVieldingTechnique)
			{
				((NKMCore.Abilities.Levi.SwordVieldingTechnique) ability).OnSwing += (character, cell) => 
                    Add(new MoveTo(
	                    HexMapDrawer.Instance.GetCharacterObject(character).transform,
	                    Active.SelectDrawnCell(cell).transform.position, 0.13f
                    ));
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