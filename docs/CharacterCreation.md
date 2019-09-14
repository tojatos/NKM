# Character creation

- [Prerequisites](#prerequisites)
- [Adding to database](#adding-to-database)
  - [Character](#character)
  - [Abilities](#abilities)
- [Ability creation](#ability-creation)
- [Adding images](#adding-images)
  - [Character](#character-1)
  - [Abilities](#abilities-1)
  - [Effects](#effects)
- [Testing](#testing)
  - [Init](#Init)
  - [Console](#console)

## Prerequisites

Before you start, it is good to have the character designed.\
You should prepare:
* Character name
* Base stats values (HP, speed, basic attack range, basic attack value, magic resist, armor).
* Character attack type (ranged | melee)
* Ability descriptions (generally every character has 3 abilities: passive, active and ultimate. It is possible to have more, but not recommended)
* Images:
  * Character
  * Abilities
  * Effects (Optional)

If you want to contribute your character to this repository, you should present your designed character with the NKM team. As there is no flow created for this yet, simply mail me or raise an issue.
After your design is officially accepted, you are ready to go!

## Adding to database

### Character

TODO

### Abilities

TODO

## Ability creation

Look at existing abilities at `Assets/Scripts/NKMCore/Abilities`, read their descriptions and analyze the code.\
Play the game and see how they actually work. (Or you can [test](#testing) it.)

There is no better way of learning this right now. Trust me.

Create a folder inside `Abilities` folder with your character name, and create `<ability_name>.cs` for every ability. Remember to be accurate with the naming, otherwise you will be running into issues.
You shouldn't care about how to modify other code than your ability files, just use the existing system. If it is not possible to make your ability using current system, raise an issue.

## Adding images

All images should be in png format.

### Character

Character image should be cut to hexagon. You can do that for example by opening existing character image in gimp, select by color transparent edges, scale your image and put the face on the center of selected hexagon, and finally cut the selected edges to transparency and export new image. Or, alternatively, have someone to do it for you :).

Put it in `Assets/Resources/Sprites/CharacterHexagons` with name matching the character name.

### Abilities

Ability images should be squares. Something like 100x100 px would be good, but it is not mandatory (it can scale).

Put them in `Assets/Resources/Sprites/Abilities` with name matching the ability name.

### Effects

Effect images should be diamonds. You should use existing effect image as a template, just like in character case.

Put them in `Assets/Resources/Sprites/Effects` with name matching the ability name.

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
