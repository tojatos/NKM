# Game Rules

## Phases

Every phase has a number.

When you start the game, you enter the **phase zero** - this is the initial phase, where every player has to place every character on his spawn points. Additionally, in that phase every turn is finished just after placing one character on a spawn point. This phase is finished immediately when every champion is placed.

After finishing **phase zero**, you enter the next phase. In that and every further phase, Players can take turns.
Those phases are finished when no character on map can take action.

## Turn

When phase starts, the first player can take a turn. Taking a turn means making actions with selected character. In a turn only one character can make actions. After a turn is finished, that character cannot make any more actions in this phase, and the next player can take his turn.

### Passing a turn
If a character cannot make any action, or if you just want to, you can pass a turn using this character. This means that the character cannot take any more turns in the phase, and a turn is immediately finished.

### Finishing a turn

To click the "Finish turn" button a Player has to make an action with a character. The only exception of that rule is when every alive character has already taken their turn.

## Characters

Every character has:

* Stats
	* Attack Points
	* Health Points
	* Basic Attack Range
	* Speed
	* Physical Defense
	* Magical Defense
* Attack Type
* Abilities
* Effects (none at start by default)

### Death
When a character has no Health Points left, they are removed from the map.


## Actions

There are four possible actions:
* Basic move
* Basic attack
* Use normal ability
* Use ultimatum ability

### Constraints
A character can make as many actions as he wants in his turn, with following constraints (that refer to this character in the current phase):
* Basic move:
	* A basic move has not been used
	* An ultimatum ability has not been used
* Basic attack and use normal ability:
	* A basic attack has not been used
	* A normal ability has not been used
	* An ultimatum ability has not been used
* Use ultimatum ability:
	* An ultimatum ability has not been used
	* No action had been made by this character in this turn

## Abilities
Every ability has a cooldown. If that cooldown is bigger than zero, the character that owns the ability cannot use it. The cooldown is decreased each time a phase is finished.

There are three ability types:
* Passive
* Normal
* Ultimatum

Usually, a character has three abilities, each with different type.

## Effects
Every effect has a cooldown. When that cooldown reaches 0, the effect is removed from the character. The cooldown is decreased by one each time an effect is applied.

An effect applies just before the first action, that character takes.
If that effect kills the character, it cancels the action that would be done.
