# rhythm-game

- This is a prototype of the game Magic Tiles 3
- The project was made using Unity 2021.3.45f1
- Project demo video [Here](https://drive.google.com/file/d/1QlUouMexU27PL0BkPgiPhUtNB8yWiXo5/view?usp=sharing)

## How to run:
- Open the project with Unity Editor
- Go the Scenes folder and run the MapSelect scene
- In the MapSelect scene, you can choose from 3 maps
    - DJ Genericname - Dear you
    - Sporty-O - Let me hit it
    - SynthWulf - Passacaglia
- After hitting the **Play** button on one of the map, the game will transition to the Gameplay scene
- The notes will fall downwards, on PC use the keyboard keys **DFJK** to press the notes, holding notes require **holding** down the key. Touch input for mobile is also supported
- Missing one note will result in Game Over, hitting all the notes will result in a victory when the song ends

## Beatmap design:
Each map in this game consist of 2 main component
- An audio file
- A midi file (.mid)

The midi file is used as a template to spawn notes rhythmically. I used FL Studio along with its Piano roll tool to place notes for each song and then export the pattern as a midi file to the Unity project. The midi file contains time stamp for each note, it also contains the length of each note (long notes will be spawned as holding notes)

- Each map in the game is defined as a ScriptableObject, it will hold the audio file and midi file path along with some other basic data, a MapCatalog class will load the midi file from StreamingAssets along with all the maps
- This makes it much easier to add new maps, all we need is an audio file and a midi file

## Tap detection:
When the user inputs a tap or hold, the game will take the time at which the note is pressed and determine if it is a hit or not

```csharp
if (inputDown)
{
    double timingDifference = Math.Abs(audioTime - timeStamp);
    if (timingDifference < marginOfError)
    {
        HitType hitType = timingDifference < marginOfError / 2f ? HitType.Perfect : HitType.Good;
        currentTile.DestroyTile(hitType);
        lane.AdvanceInputIndex();
        return;
    }
}
```


## Note spawning:
- The game contains 4 Lanes, a Lane is reponsible for spawning and keeping track of each note, a LaneInputHanlder is attached to each lane to detect input for that lane
- Each note spawned has a Tile class attached, the class manages its own movement, tap/hold logic. Each note also has a TileVisual class to handle feedback VFX

## Design pattern
The project mainly utilizes the Singleton and Observer pattern, this allows for decoupling and remove dependencies between classes. One clear benefit is that all the UI and background VFX logic does not affect the core gameplay logic, they listen to events fired during the game and update accordingly.

## External libraries used in the project:
- [LeanPool](https://assetstore.unity.com/packages/tools/utilities/lean-pool-35666?srsltid=AfmBOornci8W9T76f5De7f59USnpcFLBvNClUPp1WZlutIhlxy_B1eUd) for quick object pooling implementation
- [Dotween](https://assetstore.unity.com/packages/tools/utilities/lean-pool-35666?srsltid=AfmBOornci8W9T76f5De7f59USnpcFLBvNClUPp1WZlutIhlxy_B1eUd) for tweening, animating game objects
- [DryWetMIDI](https://assetstore.unity.com/packages/tools/audio/drywetmidi-222171?srsltid=AfmBOooppkbds4H1AtaIar4xY8gFMU7WHoGao2LLru-i7cmMndUjdQi8) for reading and handling midi files
    - DryWetMIDI Github repo: https://github.com/melanchall/drywetmidi
- [FL Studio](https://www.image-line.com) for beatmap creation

## Game screen shots
![level_select](https://github.com/user-attachments/assets/0299bf8d-1d8b-4240-90ff-c241a3ef7587)
![gameplay](https://github.com/user-attachments/assets/bf67876e-0c22-4ff8-a710-7b16da240096)

