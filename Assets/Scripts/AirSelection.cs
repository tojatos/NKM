using System.Collections.Generic;
using System.Linq;
using Hex;

public class AirSelection
{
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
		Shape = SelectionShape.None;
		Size = 0;
	}
	private SelectionShape Shape;
	private int Size;
	private List<HexCell> _hexCells;
	public List<HexCell> HexCells
	{
		get { return _hexCells; }
		set
		{
			_hexCells = value;
			if (_hexCells != null)
			{
				if (Shape == SelectionShape.Circle)
				{
					_hexCells.AddRange(value[0].GetNeighbors(Size));
				}
			}
			Game.HexMapDrawer.RemoveAllHighlights();
			if (Game.Active.HexCells != null && _hexCells != null)
			{
				Game.Active.HexCells.ForEach(c =>
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
		Game.Active.HexCells.ForEach(c => c.ToggleHighlight(HiglightColor.WhiteOrange));
		IsEnabled = true;
		Shape = shape;
		Size = size;
	}
	public void Disable()
	{
		IsEnabled = false;
		Shape = SelectionShape.None;
		HexCells = null;
	}

}
