# ActionEffectRange

A FFXIV Dalamud plugin that provides a visual cue on the effect range of the AoE action the player has just used.

May be used as a supplement/replacement to the actions' VFXs in showing effect range related information, 
such as where has the action landed and how large an area it covered.


## How to Install

[XIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) is required to install and run the plugin.

Add my [Dalamud plugin repo](https://github.com/yomishino/MyDalamudPlugins) to Dalamud's Custom Plugin Repositories.

Once added, look for the plugin "ActionEffectRange" in Plugin Installer's available plugins.


## Disclaimer

1. Because the visual cues are drawn on an overlay without any current context/knowledge about the in-game geographical features etc.,
   it can sometimes look distorted or "hovered in the air" depending on the terrain and/or camera angle.

2. Please expect small errors in calculation. Apart from that, there may also be mistakes. 


## Known Issues

- Not showing effect range for several AoE skills that are automatically triggered after set time

- Dancer's "Curing Waltz": Not showing effect range for additional effect (AoE heal around partner)

- Dark Knight's "Salt and Darkness": Not showing effect range when used
