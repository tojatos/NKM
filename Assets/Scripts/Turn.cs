using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UIManagers;

public class Turn
{
	public Active Active;
	public bool IsDone { get; private set; }
	//private int Number;
	public bool WasCharacterPlaced { get; set; }
	public Character CharacterThatTookActionInTurn { get; set; }

	public Turn()
	{
		IsDone = false;
		WasCharacterPlaced = false;
		CharacterThatTookActionInTurn = null;
		TurnFinished += () =>
		{
			if (CharacterThatTookActionInTurn != null)
			{
				CharacterThatTookActionInTurn.TookActionInPhaseBefore = true;
			}
			IsDone = true;
			WasCharacterPlaced = false;
			CharacterThatTookActionInTurn = null;
			Active.Reset();
			HexMapDrawer.RemoveAllHighlights();
		};
	}
	public void Start(Player player)
	{
		Active.Player = player;
		Active.Turn.IsDone = false;
		Active.Reset();
		UIManager.Instance.UpdateActivePlayerUI();
		if (Active.Phase.Number == 0)
		{
			if (Active.Player.HasSelectedCharacters == false)
			{
				UIManager.Instance.StartSelectAndInitializeThings();
			}
			else if (Active.Player.Characters.Any(c => !c.IsOnMap))
			{
				UIManager.Instance.ForcePlacingChampions = true;
			}
		}
	}

	public delegate void OnTurnFinish();
	public event OnTurnFinish TurnFinished;

	public void Finish()
	{
		TurnFinished?.Invoke();
	}
	//public void TurnFinished()
	//{
	//	if (CharacterThatTookActionInTurn != null)
	//	{
	//		CharacterThatTookActionInTurn.TookActionInPhaseBefore = true;
	//	}
	//	IsDone = true;
	//	//Number++;
	//	WasCharacterPlaced = false;
	//	CharacterThatTookActionInTurn = null;
	//	Active.Reset();
	//	HexMapDrawer.RemoveAllHighlights();
	//}
}
