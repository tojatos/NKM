using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Abilities.Levi;
using NKMCore.Abilities.Sakai_Yuuji;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Animations;
using Unity.Hex;
using UnityEngine;
using AsterYo = NKMCore.Abilities.Hecate.AsterYo;
using Check = NKMCore.Abilities.Bezimienni.Check;
using ItadakiNoKura = NKMCore.Abilities.Hecate.ItadakiNoKura;

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

        public static void AddTriggers(Ability ability)
        {
            GameObject Gco(Character z) => HexMapDrawer.Instance.GetCharacterObject(z);
            DrawnHexCell Gdc(HexCell cell) => HexMapDrawer.Instance.SelectDrawnCell(cell);
            switch (ability)
            {
                case Check check:
                    check.AfterCheck += character => Add(new Animations.Check(character));
                    break;
                case AsterYo yo:
                    yo.BeforeAsterBlaster += (character, characters) =>
                        Add(new Animations.AsterYo(
                                Gco(character).transform,
                                characters.Select(c => HexMapDrawer.Instance.GetCharacterObject(c).transform).ToList()
                            ));
                    break;
                case ItadakiNoKura kura:
                    kura.AfterCollectingEnergy += (parent, target) =>
                        Add(new Animations.ItadakiNoKura(Gco(parent).transform, Gco(target).transform));
                    break;
                case NKMCore.Abilities.Itsuka_Kotori.CamaelMegiddo megiddo:
                    megiddo.BeforeFlamewave += (lineCells, conflagrationCells) =>
                        Add(new CamaelMegiddo(
                                lineCells.Select(c => Gdc(c).transform).ToList(),
                                conflagrationCells.Select(c => Gdc(c).transform).ToList()
                            ));
                    break;
                case SwordVieldingTechnique technique:
                    technique.OnSwing += (character, cell) =>
                        Add(new MoveTo(
                                Gco(character).transform,
                                Gdc(cell).transform.position,
                                0.13f
                            ));
                    break;
                case VerticalManeuveringEquipment equipment:
                    equipment.OnSwing += (character, cell) =>
                        Add(new MoveTo(
                                Gco(character).transform,
                                Gdc(cell).transform.position,
                                0.13f
                            ));
                    break;
                case Grammatica grammatica:
                {
                    grammatica.BeforeGrammatica += (parentCharacter, targetCharacter) => Add(new GrammaticaStart(parentCharacter, targetCharacter));
                    grammatica.AfterGrammatica += (parentCharacter, targetCharacter, targetCell) =>
                        Add(new GrammaticaFinish(
                                Gco(parentCharacter).transform,
                                Gco(targetCharacter).transform,
                                Gdc(targetCell).transform.TransformPoint(0,10,0)
                            ));
                    break;
                }
            }
        }
        public static void AddTriggers(Character character)
        {
            character.AfterAttack += (targetCharacter, damage) =>
                Add(new Tilt(HexMapDrawer.Instance.GetCharacterObject(targetCharacter).transform));
            character.AfterAttack += (targetCharacter, damage) =>
                Add(new ShowInfo(HexMapDrawer.Instance.GetCharacterObject(targetCharacter).transform, damage.Value.ToString(), Color.red));
            character.AfterHeal += (targetCharacter, valueHealed) =>
                Add(new ShowInfo(HexMapDrawer.Instance.GetCharacterObject(targetCharacter).transform, valueHealed.ToString(), Color.blue));
            character.OnDeath += () => Add(new Destroy(HexMapDrawer.Instance.GetCharacterObject(character)));
            character.AfterRefresh += () => Add(new Undim(character));
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