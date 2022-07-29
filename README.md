# Subnautica Cinematic Mod
A Subnautica mod made to create cool looking camera shots

## Requirements
- [SMLHelper](https://www.nexusmods.com/subnautica/mods/113)
- [QModsManager](https://www.nexusmods.com/subnautica/mods/201)

## How to use
*Currently, everything is done with commands, so remember to enable console*

### Create path
First thing you have to do is to setup a path for the camera to follow,
path works with points that will be interpolated to form a path.

To add a point, move where you want to add your point (easier with free cam),
adjust roll and FOV, then execute `addpoint` command.

Once you have setup at least 2 points, you will be able to execute command : `runpath <time>`,
with parameter time being the duration of the path in seconds. Example: `runpath 15` will make a 15 seconds long path.

## Commands
`clearpoints` : Delete every previously set points.

`stoppath` : Stop a path while it's rendering (similar to Escape).

`togglepathpreview` : Toggle the path preview lines (red is linear, yellow is the true interpolated path).

`addpoint` : Add a point at position, rotation and FOV of the camera.

`runpath <time>` : Run the path for *\<time>* seconds 

## Key Binds
- `RightArrow` : roll right
- `LeftArrow` : roll left
- `DownArrow` : reset roll
- `NumpadPlus` : increase FOV (dezoom)
- `NumpadMinus` : decrease FOV (zoom)
- `NumpadEnter` : Reset FOV to 70
- `Escape` : Stop path while it's rendering (like the `stoppath` command)

## Features planned
[ ] GUI Edition of path

## Other
License : [![License](https://img.shields.io/badge/license-CC--BY--NC-orange?style=flat-square&logo=creative-commons)](https://github.com/MoreOwO/Subnautica-Cinematic-Mod/blob/main/LICENSE.md)