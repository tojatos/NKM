using UIManagers;

namespace MyGameObjects.MyGameObject_templates
{
	public abstract class Effect : MyGameObject
	{
		protected Effect(int cooldown, Character parentCharacter, string name = null)
		{
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCharacter = parentCharacter;
			if (name != null) Name = name;
			if (Active.CharacterOnMap == ParentCharacter) CharacterEffects.Instance.UpdateButtons();
			Active.Turn.TurnFinished += () => {
                if (CurrentCooldown > 0) CurrentCooldown--; 
				if (CurrentCooldown != 0) return;
				RemoveFromParent();
				
				if (Active.CharacterOnMap == ParentCharacter) CharacterEffects.Instance.UpdateButtons();
			};

		}
		public delegate void OnRemoveHandler();
		public event OnRemoveHandler OnRemove;

		protected int CurrentCooldown { get; private set; }
		protected Character ParentCharacter { get; }
		public EffectType Type { get; protected set; }

		public abstract string GetDescription();
		public virtual int Modifier(StatType statType)
		{
			return 0;
		}

		public void RemoveFromParent()
		{
			ParentCharacter.Effects.Remove(this);
			OnRemove?.Invoke();
		}
		
	}

	public enum EffectType
	{
		Positive,
		Negative,
		Neutral
	}

}
