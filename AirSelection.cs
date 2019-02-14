using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;

namespace NKMCore
{
	public class AirSelection
	{
		private readonly Game _game;

		public event Delegates.CellHashSet AfterEnable;
		public event Delegates.CellHashSet AfterCellsSet;
		
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
		/// <summary>
		/// Sets cells in respect to first cell in the list
		/// </summary>
		private HashSet<HexCell> _hexCells;
		public HashSet<HexCell> HexCells
		{
			get { return _hexCells; }
			set
			{
				_hexCells = value;
				if (_hexCells != null)
				{
					if (_shape == SelectionShape.Circle)
					{
						_hexCells.UnionWith(value.First().GetNeighbors(_game.Active.GamePlayer, _size));
					}
				}
				AfterCellsSet?.Invoke(_hexCells);
			}
		}

		public void Enable(SelectionShape shape, int size)
		{
			IsEnabled = true;
			_shape = shape;
			_size = size;
			AfterEnable?.Invoke(_game.Active.HexCells);
		}
		public void Disable()
		{
			IsEnabled = false;
			_shape = SelectionShape.None;
			HexCells = null;
		}

	}
}
