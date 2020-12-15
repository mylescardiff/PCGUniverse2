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

## Procedural Objects

The primary procedurally generated item is the planet. There are two types of planets in the universe:

- Rocky
- Gas Giants

Both types of planets have colored biomes, and each biome color has within it a hight map. This basically gets ignored
on Gas giants because they have no surface detail, only a smooth surface. 

### Rocky Planets

Rocky planets are made by creating an icosphere; A six sided cube that puffed out by normalizing the 
points, creating a perfect sphere. An algorithm then goes over each vertex and applies simplex noise 
to them with pre-determined paramters to create terrain.

There is a minimum floor level imposed, so that planets take a generally sphereical shape and don't 
have pits that reach down too far to be realistic. In the case where that planet has a liquid ocean,
the floor is the oceans surface, and the shader color is altered to have the very lowest color element
of the gradient be the ocean color, and that layer has a shine applied to it via ShaderGraph. 

I would like to ackknowlege Sebastian Lague who has an excellent video series on this creating planets in this fasion: 
https://www.patreon.com/SebastianLague/posts

### Gas Giants

Gas giants have no surface detail, but the generator will assign them many more "biomes" than the rocky planets
to similate the stripey look we see on planets like jupiter. Then noise and blending are applied to smooth the
transition, and make the stripes look more naturally skewed.
