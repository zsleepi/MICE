using Godot;
using MICE.scripts.lib;
using MICE.scripts.lib.itemlogic;
using System;

public interface IActor
{
	Vector2I Cell { get; set; } // refactor to use WorldCoords?
	Node2D Node { get; } // might move away from using a node-based system at some point

    Sprite2D Sprite { get; set; }
    abstract void HandleBump(bool didBump); // will remove this at some point. this is not a good way to have actors react to things.

	// later, we will add a requirement for a body. everybody needs a body. things like equipSlots and species is derived from body
	Signature signature { get; set; }
	Inventory inventory { get; set; }
	CoreSkills coreSkills { get; set; }
	Experience experience { get; set; }

	Health health { get; set; }
	Psyche psyche { get; set; }

}
