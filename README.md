# PCG Universe 2.0
A 3D Procedurally Generated Universe, Built in Unity 2019

This project was created as part of my degree in Game Programming from Academy of Ary University. This time in 3D, using Unity. It generates a galaxy, stars, solar systems, and planets down to the surface details. There's a little gameplay inserted to guide you along the path and see some interesting procedurally generated content. 

Here are some of the techniques employed:
- Simplex Noise
- Space Partitioning
- L-Systems

This project is still under construction as of 12/7/2020, so check back soon for a more complete product!

## Runnning the Project

Open /Assets/Scenes/scene_Startup/scene_Startup.unity. This scene contains some overall gameplay tunables and some persistent objects necessary for the game to work. 

## Scenes

There are three main scenes in the game:

- Galaxy
- Solar System
- Planet

You'll find code folders for each of these under the /Assets/Scripts/ folder. Each scene has a code file with a matching name that governs the mechanics in that scene. 

There is a persistent scene in scene_Startup that has a single script, GameManager.cs. That manages the actual galaxy data, because that needs to be present in all the scenes to manage game flow. 
