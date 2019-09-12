# Character creation

- [Prerequisites](#prerequisites)
- [Adding to database](#adding-to-database)
- [Ability creation](#ability-creation)
- [Adding character sprite](#adding-character-sprite)
- [Testing](#e-character-sprite)
  - [Init](#Init)
  - [Console](#console)

## Prerequisites

Before you start, it is good to have the character designed.\
You should prepare:
* Character name
* Base stats values (HP, speed, basic attack range, basic attack value, magic resist, armor).
* Character attack type (ranged | melee)
* Ability descriptions (generally every character has 3 abilities: passive, active and ultimate. It is possible to have more, but not recommended)
* Ability images
* Character image

## Adding to database

### Character

### Abilities

## Ability creation
## Adding images

All images should be in png format.

### Character

Character image should be cut to hexagon. You can do that for example by opening existing character image in gimp, select by color transparent edges, scale your image and put the face on the center of selected hexagon, and finally cut the selected edges to transparency and export new image. Or, alternatively, have someone to do it for you :).

Put it in `Assets/Resources/Sprites/CharacterHexagons` with name matching the character name.

### Abilities

Ability images should be squares. Something like 100x100 px would be good, but it is not mandatory (it can scale).

Put them in `Assets/Resources/Sprites/Abilities` with name matching the ability name.

### Effects

TBD

## Testing

### Init

To test a character, go to `Game` scene, select `GameStarter` in hierarchy and select `IsTesting` checkbox in the inspector. (Remember to uncheck it before commiting!). After you click `Start` in Unity, a testing map will appear with all characters specified in `testing_characters.txt` spawned. You can modify this file and put your character there with all other characters you wish to appear.

Example `testing_characters.txt`:
```
Kurogane Ikki
Bezimienni
Ononoki Yotsugi

Kirito

Hecate
Aqua
```
Will spawn three characters for player 1, `Kirito` for player 2 and two remaining characters to player 3. As you can see, an empty line marks the end of character definition for the player.


### Console

It could be tedious to test your character. For example, ultimates can be used from phase 4, so you would need to skip turn many times or if your character triggers something on death, you would need to reduce target HP.

With console it takes no time. You can show console by pressing `<F2>`, and then write some commands, for example:
* `set phase 4` - sets phase number to 4
* `set hp 0` - kills selected character

There are several abbreviations available, for example `s p 4` or `s h 0`. See CommandExecutor.cs for details.
