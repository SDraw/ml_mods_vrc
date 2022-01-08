# Vertex Animation Remover
Universal MelonLoader mod for disabling blendshapes on old hardware, designed for Unity 2018.

# Why?
In Unity 2018 way of handling blendshapes has been changed. This change leads to distorted vertex geometry if any blendshape is applied to mesh on old hardware (for example, Intel HD Graphics 3000).  
This mod simply removes all blendshapes upon mesh baking.

# Notes
* It's possible that your game can have ties between in-game mechanics and blendshapes. In such case nothing can be done.
* Game can look slightly different in comparison with non-moded game.
