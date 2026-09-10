using Godot;
using MICE.scripts.lib;
using MICE.scripts.lib.itemlogic;
using System;

public interface IActor
{
	Vector2I Cell { get; set; } // refactor to use WorldCoords?
    Sprite2D Sprite { get; set; }

	// later, we will add a requirement for a body. everybody needs a body. things like equipSlots and species is derived from body
	Signature signature { get; set; }
	Inventory inventory { get; set; }
	CoreSkills coreSkills { get; set; }
	Experience experience { get; set; }

	Health health { get; set; }
	Psyche psyche { get; set; }

}
