# NKM

A multiplayer turn-based strategy game on a hex board.

Made with C# (Unity as frontend)

![Picture of the game](https://raw.githubusercontent.com/tojatos/NKM/rewrite/docs/images/nkm.png)

Currently the only supported language in game is Polish.

## About

This is my first bigger project. I learned a lot here.
This game was rewritten many times, as you may see in commit history.
Every time I wanted to add a new feature, a new problem would arise.
Hopefully I'm good enough now to stop rewriting the game and start developing!

## How the game works

You can read about the rules in the [Game Rules](https://github.com/tojatos/NKM/blob/rewrite/docs/GameRules.md) section.\
If you are a developer, you may be interested in [Introduction](https://github.com/tojatos/NKM/blob/rewrite/docs/Introduction.md).

## Where is the code?

`Assets/Scripts/` - all code

`Assets/Scripts/Unity` - frontend code

`Assets/Scripts/NKMCore` - backend code

`Assets/Scripts/NKMCore/Templates/` - some important files, such as Ability.cs and Character.cs

`Assets/Scripts/NKMCore/Abilities/` - fun stuff - character abilities

## Running / building the game

To run the game clone the project (there are submodules, run `git submodule sync --recursive && git submodule update --init --recursive` after cloning) and open it in Unity (I am working on newest versions, so be careful: it may or may not work in other versions).
You can build using Unity's usual way (ctrl+B), or using `Developement` on the toolbar in Unity (recommended).

Make sure that in the Main Game scene IsTesting is not selected in Game Starter object (it automatically loads characters from Assets/testing_characters.txt).

Warning: git-lfs must be installed to pull everything correctly!

## Contributing

Pull requests are welcome, but please raise an issue before making any changes to let me know that you are working on something.
