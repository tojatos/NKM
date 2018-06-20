namespace NKMObjects.Templates
{
	public abstract class Effect : NKMObject
	{
		protected Effect(int cooldown, Character parentCharacter, string name = null)
		{
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCharacter = parentCharacter;
			if (name != null) Name = name;
			if (Active.CharacterOnMap == ParentCharacter) UI.CharacterUI.Effects.Instance.UpdateButtons();
			Active.Turn.TurnFinished += (character) => {
				if(character != ParentCharacter) return;
				if(CurrentCooldown == int.MaxValue) return;
                if (CurrentCooldown > 0) CurrentCooldown--; 
				if (CurrentCooldown != 0) return;
				RemoveFromParent();
				
				if (Active.CharacterOnMap == ParentCharacter) UI.CharacterUI.Effects.Instance.UpdateButtons();
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
