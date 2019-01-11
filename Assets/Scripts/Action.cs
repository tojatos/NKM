using System;
using System.Collections.Generic;
using Hex;
using NKMObjects.Templates;

public class Action
{
    private readonly Game _game;
    public Action(Game game)
    {
        _game = game;
    }

    public void Make(string actionType, string[] args)
    {
        switch (actionType)
        {
            case Types.PlaceCharacter:
                break;
            case Types.FinishTurn:
                break;
            case Types.TakeAction:
                break;
            case Types.Move:
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

    public void PlaceCharacter(Character character, HexCell targetCell) => _game.HexMap.Place(character, targetCell);
    private void FinishTurn() => _game.Active.Turn.Finish();
    private void TakeAction(Character character) => character.TryToInvokeJustBeforeFirstAction();
    private void Move(Character character, HexCell targetCell) => _game.HexMap.Move(character, targetCell);
    private void BasicAttack(Character character, Character target) => character.MakeActionBasicAttack(target);
    private void ClickAbility(IClickable ability) => ability.Click();
    private void UseAbility(IUseable ability, List<HexCell> cells) => ability.Use(cells);
    private void CancelAbility(Ability ability) => ability.Cancel();

    public static class Types
    {
        public const string PlaceCharacter = "PlaceCharacter";
        public const string FinishTurn = "FinishTurn";
        public const string TakeAction = "TakeAction";
        public const string Move = "Move";
        public const string BasicAttack = "BasicAttack";
        public const string ClickAbility = "ClickAbility";
        public const string UseAbility = "UseAbility";
        public const string CancelAbility = "CancelAbility";
    }
}