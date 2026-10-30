2D Pixel Terrain
Free edition, version 1.0
by Mythryl Studio

2D isometric terrain tileset: grass, cliffs, water, trees, plants and rocks in 4 seasons. 32px and 64px.

DOCUMENTATION

This file is the pack's documentation: its sizes, how it works and how it's set up in Unity.

SIZES

32px/ and 64px/: the same pack at each size, each with its own example scene.

HOW THIS PACK WORKS

Isometric terrain at 32px: 32 x 16 tiles. tiles.json has the cell size and the numbering of every set; nature/nature.json has every piece of nature.

UNITY

Every tile is a sprite, one unit to a tile's width, with a Tile asset and Tile Palettes for an isometric Tilemap (cell size 1 x 0.5). Water tiles are Animated Tiles; each coast land has a Rule Tile that lays its shores beside any cell painted with something else (not beside empty cells, so the map's edge stays land). Each corner set has a Rule Tile that lays its path: paint the path on a Tilemap of its own over the ground, two cells wide or more, and the path's edge runs inside the cells you paint. Rule Tiles need the 2D Tilemap Extras package, and the Autotiles palette has them all. Put each height of cliffs on a Tilemap of its own, raised by the block's step, sorted below anything standing on the map (the example uses -10 for the ground and -9 up for the cliffs). Each piece of nature has a prefab with a Shadow and a Body, its sheet sliced as piece_look_turn. The example scene lays the valley.

All images are PNG with transparency. Scale by whole numbers
(2x, 3x, 4x) with nearest-neighbour filtering to keep pixels crisp.

This is the free edition. The full pack is available at
https://mythrylstudio.itch.io/pixel-terrain

SUPPORT

hello@mythryl.com
https://mythryl.com

LICENCE

Unity Asset Store EULA.
