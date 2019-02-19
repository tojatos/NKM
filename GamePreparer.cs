using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public class GamePreparer
    {
        private readonly GamePreparerOptions _preparerOptions;
        public readonly GameOptions GameOptions;
        
        private readonly GameOptionsValidator _gameOptionsValidator;

        public GamePreparer(GamePreparerOptions options)
        {
            _preparerOptions = options;
            GameOptions = new GameOptions
            {
                Players = GetPlayers(),
                HexMap = options.HexMap,
                Type = options.GameType,
                Selectable = options.Selectable,
                Connection = options.Connection,
                LogFilePath = options.LogFilePath,
                PlaceAllCharactersRandomlyAtStart = options.PickType == PickType.AllRandom,
            };
            _gameOptionsValidator = new GameOptionsValidator(
                GameOptions,
                options.NumberOfPlayers,
                options.NumberOfCharactersPerPlayer
            );
        }

        public void AddPlayer(GamePlayer player) => GameOptions.Players.Add(player);
        public void RemovePlayer(GamePlayer player) => GameOptions.Players.Remove(player);
        public void AddPlayers(IEnumerable<GamePlayer> players) => GameOptions.Players.AddRange(players);
        
		public async Task BindCharactersToPlayers(Game game)
		{
            switch (_preparerOptions.PickType)
            {
                case PickType.Blind:
                    await BlindPick(game);
                    break;
                case PickType.Draft:
                    List<Character> charactersToPick = Game.GetMockCharacters();
                    if(_preparerOptions.BansEnabled) await Bans(game, charactersToPick);
                    await DraftPick(game, charactersToPick);
                    break;
                case PickType.AllRandom:
                    AllRandom(game);
                    break;
            }
		}
        
		private async Task BlindPick(Game game)
		{
			foreach (GamePlayer p in game.Players)
			{
				List<Character> allCharacters = Game.GetMockCharacters();
				await PickCharacters(allCharacters, _preparerOptions.NumberOfCharactersPerPlayer, p, game);
			}
		}
		private async Task PickCharacters(List<Character> charactersToPick, int numberOfCharactersToPick, GamePlayer player, Game game)
		{
			await SelectAndWait(new SelectableProperties<Character>
			{
				ToSelect = charactersToPick,
				ConstraintOfSelection = list => list.Count == numberOfCharactersToPick,
				OnSelectFinish = list => player.Characters.AddRange(list.Select(c => CharacterFactory.Create(game, c.Name))),
				SelectionTitle = $"Wybór postaci - {player.Name}",
			});
		}
		private async Task DraftPick(Game game, List<Character> charactersToPick)
		{
			Func<bool> allCharactersPicked = () => game.Players.All(p => p.Characters.Count == _preparerOptions.NumberOfCharactersPerPlayer);
			while (!allCharactersPicked())
			{
				foreach (GamePlayer player in game.Players) 
					await DraftPickOneCharacter(charactersToPick, player, game);
				if(allCharactersPicked()) break;
				foreach (GamePlayer player in game.Players.AsEnumerable().Reverse()) 
					await DraftPickOneCharacter(charactersToPick, player, game);
			}
		}
		private void AllRandom(Game game)
		{
			List<string> allCharacterNames = _preparerOptions.Connection.GetCharacterNames();
			game.Players.ForEach(p=>
			{
				while (p.Characters.Count != _preparerOptions.NumberOfCharactersPerPlayer)
				{
                    string randomCharacterName = allCharacterNames.GetRandom();
                    allCharacterNames.Remove(randomCharacterName);
                    p.Characters.Add(CharacterFactory.Create(game, randomCharacterName));
				}
			});
		}
		private async Task Bans(Game game, List<Character> charactersToPick)
		{
			int bansLeft = _preparerOptions.NumberOfBans;
			while(bansLeft != 0)
			{
				foreach (GamePlayer player in game.Players) 
					await BanOneCharacter(charactersToPick, player);
				bansLeft--;
				if(bansLeft==0) break;
				foreach (GamePlayer player in game.Players.AsEnumerable().Reverse()) 
					await BanOneCharacter(charactersToPick, player);
				bansLeft--;
			}
		}
		private async Task DraftPickOneCharacter(List<Character> charactersToPick, GamePlayer player, Game game)
		{
			await SelectAndWait(new SelectableProperties<Character>
			{
				ToSelect = charactersToPick,
				ConstraintOfSelection = list => list.Count == 1,
				OnSelectFinish = list =>
				{
					player.Characters.Add(CharacterFactory.Create(game, list[0].Name));
					charactersToPick.Remove(list[0]);
				},
				SelectionTitle = $"Wybór postaci - {player.Name}",
			});
		}
		private async Task BanOneCharacter(List<Character> charactersToPick, GamePlayer player)
		{
			await SelectAndWait(new SelectableProperties<Character>
			{
				ToSelect = charactersToPick,
				ConstraintOfSelection = list => list.Count == 1,
				OnSelectFinish = list => charactersToPick.Remove(list[0]),
				SelectionTitle = $"Banowanie postaci - {player.Name}",
			});
		}
		private async Task SelectAndWait(SelectableProperties<Character> props)
		{
			bool isSelected= false;
			props.OnSelectFinish += list => isSelected = true;
			_preparerOptions.Selectable.Select(props);
			Func<bool> picked = () => isSelected;
			await picked.WaitToBeTrue();
		}
        
		private List<GamePlayer> GetPlayers()
        {
            if (_preparerOptions.GameType == GameType.Local)
            {
                return Enumerable.Range(0, _preparerOptions.NumberOfPlayers)
                    .Select(n => new GamePlayer {Name = $"Player {n}"}).ToList();
            }
            return new List<GamePlayer>();
        }

        public bool AreOptionsValid => _gameOptionsValidator.AreOptionsValid;
    }
}