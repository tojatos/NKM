# NKM

A local multiplayer turn-based strategy game on a hex board.

Made with C# (Unity as frontend)

## About

This is my first bigger project. I learned a lot here.
This game was rewritten many times, as you may see in commit history.
Every time I wanted to add a new feature, a new problem would arise.
Hopefully I'm good enough now to stop rewriting the game and start developing!

## How the game works

You can read about it in the [Game Rules](https://github.com/tojatos/NKM/blob/master/docs/GameRules.md) section.
It will be improved later.

## Where is the code?!

`Assets/Scripts/` - all code

`Assets/Scripts/Unity` - frontend code

`Assets/Scripts/NKMCore` - backend code

`Assets/Scripts/NKMCore/Templates/` - some important files, such as Ability.cs and Character.cs

`Assets/Scripts/NKMCore/Abilities/` - fun stuff - character abilities

## Running / building the game

To run the game simply clone the project and open it in Unity (should work in 2018.1+).
You can build using Unity's usual way (ctrl+B), using `Developement` on the toolbar, or using script in root folder (BuildLinux.sh or BuildWindows.cmd).

Make sure that in the Main Game scene IsTesting is not selected in Game Starter object.

Warning: the game may not work properly on Windows due to a bug with directory names. This will be fixed after decoupling (Soon&trade;)

## Future

Right now I am focused on decoupling and writing tests.
In the future I'd like to add multiplayer mode, support for mobile devices, better documentation for contributing and a website.
