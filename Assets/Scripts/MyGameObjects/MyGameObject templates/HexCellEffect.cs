using Hex;

namespace MyGameObjects.MyGameObject_templates
{
	public abstract class HexCellEffect : MyGameObject
	{
		protected HexCellEffect(int cooldown, HexCell parentCell, string name = null)
		{
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCell = parentCell;
			if (name != null) Name = name;
		}
		
		protected int CurrentCooldown { get; private set; }
		protected HexCell ParentCell { get; }

		public abstract string GetDescription();

		public void OnPhaseFinish()
		{
			if (CurrentCooldown > 0) CurrentCooldown--; 
			if (CurrentCooldown == 0)
			{
				ParentCell.Effects.Remove(this);
			}
		}
	}

}
