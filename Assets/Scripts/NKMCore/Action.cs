﻿using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore
{
    /// <summary>
    /// Class that manages actions from players
    /// </summary>
    public class Action
    {
        private readonly Game _game;
        public Action(Game game)
        {
            _game = game;
        }

        public event Delegates.String AfterAction;
        public event Delegates.String MultiplayerAction;
        public void Make(string actionType, string[] args)//TODO
        {
            switch (actionType)
            {
                case Types.PlaceCharacter:
                    break;
                case Types.FinishTurn:
                    break;
                case Types.TakeAction:
                    break;
                case Types.BasicMove:
                    Character characterToMove = _game.Characters.First(c => c.Name == args[0]);
                    List<HexCell> cellsToMove = args.Skip(1)
                        .Select(coords => _game.HexMap.Cells.First(c => c.Coordinates.ToString() == coords)).ToList();
                    BasicMove(characterToMove, cellsToMove, true);
                    break;
                case Types.BasicAttack:
                    break;
                case Types.ClickAbility:
                    break;
                case Types.UseAbility:
                    break;
                case Types.CancelAbility:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }
        }

        public void PlaceCharacter(Character character, HexCell targetCell)
        {
            _game.HexMap.Place(character, targetCell);
            AfterAction?.Invoke(Types.PlaceCharacter);
        }

        public void FinishTurn()
        {
            _game.Active.Turn.Finish();
            AfterAction?.Invoke(Types.FinishTurn);
        }

        public void TakeTurn(Character character)
        {
            character.TryToTakeTurn();
            AfterAction?.Invoke(Types.TakeAction);
        }

        public void BasicMove(Character character, List<HexCell> cellPath, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.BasicMove};{character.Name}:{string.Join(":", cellPath.Select(c => c.Coordinates.ToString()))}");
                return;
            }
            character.TryToTakeTurn();
            character.BasicMove(cellPath);
            AfterAction?.Invoke(Types.BasicMove);
        }

        public void BasicAttack(Character character, Character target)
        {
            character.TryToTakeTurn();
            character.BasicAttack(target);
            AfterAction?.Invoke(Types.BasicAttack);
        }

        public void ClickAbility(IClickable ability)
        {
            ability.Click();
            AfterAction?.Invoke(Types.ClickAbility);
        }

        public void UseAbility(IUseableCell ability, HexCell cell)
        {
            ability.Use(cell);
            AfterAction?.Invoke(Types.UseAbility);
        }
        public void UseAbility(IUseableCellList ability, IEnumerable<HexCell> cells)
        {
            ability.Use(cells.ToList());
            AfterAction?.Invoke(Types.UseAbility);
        }

        public void UseAbility(IUseableCharacter ability, Character character)
        {
            ability.Use(character);
            AfterAction?.Invoke(Types.UseAbility);
        }

        public void Cancel()
        {
            _game.Active.Cancel();
            AfterAction?.Invoke(Types.CancelAbility);
        }

        public void Select(Character character)
        {
            _game.Active.Select(character);
            AfterAction?.Invoke(Types.Select);
        }

        public void Deselect()
        {
            _game.Active.Deselect();
            AfterAction?.Invoke(Types.Deselect);
        }
        
    
        public static class Types
        {
            public const string PlaceCharacter = "PlaceCharacter";
            public const string FinishTurn = "FinishTurn";
            public const string TakeAction = "TakeAction";
            public const string BasicMove = "BasicMove";
            public const string BasicAttack = "BasicAttack";
            public const string ClickAbility = "ClickAbility";
            public const string UseAbility = "UseAbility";
            public const string CancelAbility = "CancelAbility";
            public const string Select = "Select";
            public const string Deselect = "Deselect";
            public const string TouchCell = "TouchCell";
        }
    }
}