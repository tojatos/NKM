using System.Collections.Generic;
using System.Linq;
using Hex;
using Managers;

public class AirSelection
{
	//private static Game Game => GameStarter.Instance.Game;
	private Game Game;
	public bool IsEnabled { get; private set; }
	public enum SelectionShape
	{
		None,
		Circle
	}
	public AirSelection(Game game)
	{
		Game = game;
		IsEnabled = false;
		_shape = SelectionShape.None;
		_size = 0;
	}
	private SelectionShape _shape;
	private int _size;
	private List<HexCell> _hexCells;
	public List<HexCell> HexCells
	{
		get { return _hexCells; }
		set
		{
			_hexCells = value;
			if (_hexCells != null)
			{
				if (_shape == SelectionShape.Circle)
				{
					_hexCells.AddRange(value[0].GetNeighbors(Game.Active.GamePlayer, _size));
				}
			}
			Game.HexMapDrawer.RemoveHighlights();
			if (Game.Active.HexCells != null && _hexCells != null)
			{
				Game.Active.HexCells.ForEach(c =>
				{
					if (_hexCells.All(ac => ac != c))
					{
						Active.SelectDrawnCell(c).AddHighlight(Highlights.BlueTransparent);
					}
				});
			}

			_hexCells?.ForEach(c => Active.SelectDrawnCell(c).AddHighlight(Highlights.RedTransparent));
		}
	}

	public void Enable(SelectionShape shape, int size)
	{
		Game.Active.HexCells.ForEach(c => Active.SelectDrawnCell(c).AddHighlight(Highlights.BlueTransparent));
		IsEnabled = true;
		_shape = shape;
		_size = size;
	}
	public void Disable()
	{
		IsEnabled = false;
		_shape = SelectionShape.None;
		HexCells = null;
	}

}
