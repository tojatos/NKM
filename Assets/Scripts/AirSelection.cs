using System.Collections.Generic;
using System.Linq;
using Hex;

public class AirSelection
{
	private readonly Game _game;
	public bool IsEnabled { get; private set; }
	public enum SelectionShape
	{
		None,
		Circle
	}
	public AirSelection(Game game)
	{
		_game = game;
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
					_hexCells.AddRange(value[0].GetNeighbors(_size));
				}
			}
			_game.HexMapDrawer.RemoveAllHighlights();
			if (_game.Active.HexCells != null && _hexCells != null)
			{
				_game.Active.HexCells.ForEach(c =>
				{
					if (_hexCells.All(ac => ac != c))
					{
						c.ToggleHighlight(HiglightColor.WhiteOrange);
					}
				});
			}
			_hexCells?.ForEach(c => c.ToggleHighlight(HiglightColor.Red));
		}
	}

	public void Enable(SelectionShape shape, int size)
	{
		_game.Active.HexCells.ForEach(c => c.ToggleHighlight(HiglightColor.WhiteOrange));
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
