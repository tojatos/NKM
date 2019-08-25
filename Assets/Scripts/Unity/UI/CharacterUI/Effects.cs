using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Templates;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.UI.CharacterUI
{
    public class Effects : SingletonMonoBehaviour<Effects>
    {
        private Game _game;
        public GameObject EffectButtonPrefab;
        private List<GameObject> Buttons { get; } = new List<GameObject>();
        public void Init(Game game) => _game = game;

        public void UpdateButtons()
        {
            if (_game?.Active.Character == null) return;

            Character character = _game.Active.Character;
            RemoveButtons();
            character.Effects.ForEach(effect => CreateEffectButton(character, effect));
        }

        private static void SetButtonSprite(GameObject button, Effect effect)
        {
            Sprite effectSprite = Stuff.Sprites.Effects.SingleOrDefault(s => s.name == effect.ToString().Split('.').Last()) ?? Stuff.Sprites.Effects.Single(s => s.name == effect.GetEffectTypeName());
            button.GetComponent<Image>().sprite = effectSprite;
        }
        private void RemoveButtons()
        {
            if(Buttons == null) return;
            Buttons.ForEach(Destroy);
            Buttons.Clear();
        }
        private void CreateEffectButton(Character character, Effect effect)
        {
            GameObject button = Instantiate(EffectButtonPrefab, transform);

            button.name = character.Effects.IndexOf(effect).ToString();

            SetButtonSprite(button, effect);
            button.AddDefaultTooltip("<b>" + effect.Name + "</b>\n" + effect.GetDescription(), Tooltip.CharacterPosition);
            Buttons.Add(button);
        }
    }
}