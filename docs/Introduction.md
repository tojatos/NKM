# Introduction

NKM is a turn-based multiplayer game on a hexagonal board.

- [Project overview](#project-overview)
- [Hooks](#hooks)
- [Server](#server)
  - [Access control](#access-control)
  - [Randomness](#randomness)
- [Core details](#core-details)

## Project overview

NKM has fully decoupled frontend and backend.\
Frontend is created in Unity, but it is theoretically possible to make other frontend (for example in console).\
Backend is located at Assets/Scripts/NKMCore and is a git submodule.\
They are separated not only to make testing easier, but it had to be done in order to make online mode work.\
At beginning, only local multiplayer mode was implemented, but I wanted to make it possible to play online. I realized that if I want to write one code that would work in both local and online mode, I need to extract the core of my game and have it mirrored on the server. Bye, Unity code.

But how did I get rid of Unity code without breaking the game?

## Hooks

C# has a powerful keyword - `event`.\
Using events has its ups and downs. It may be a bit harder to track what is using event and handle exceptions, but it is a great tool for decoupling code.

Example of using events:
```csharp
game.Active.Phase.PhaseChanged += UpdateActivePhaseText;
game.Active.Turn.TurnStarted += UpdateActivePlayerUI;
```
where UpdateActivePhaseText and UpdateActivePlayerUI are functions that update UI.

## Server

To make the same code work on local and online modes, I came up with and idea to run every game as a deterministic simulation with the same inputs. 


### Access control

I needed to somehow prohibit actions of any player outside theirs turn.\
Here is the idea:

1. Player X makes an action Y
2. Server receives that player X tried to make action Y
3. Server validates if player X can make that action. If player cannot, exit at that point.
4. Server makes an action Y on his simulation
5. Server sends to each player that action Y is made
6. Action Y is made on every player's simulation

Here are some of the possible actions (located at Actions.cs):
```csharp
PlaceCharacter
FinishTurn
TakeAction
BasicMove
BasicAttack
ClickAbility
UseAbility
Cancel
Select
Deselect
OpenSelectable
CloseSelectable
ExecuteCommand (should be restricted for server only)
```

### Randomness
To sync randomness on every client, after a random event occurs on the server it sends ID of that event with result to feed the NKMRandom systems on every client.

Why didn't I just seed the random generators with the same unique ID? Well, I was a little bit worried that it would give the players an option to cheat, since they could predict the result of any random action. And remember, even without source code, C# is fairly easy to reverse engineer.

## Core details

This part of documentation is under work.

Existing pages:
* [Game dependencies](https://github.com/tojatos/NKM/blob/rewrite/docs/GameDependencies.md)
