using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Scene
{
	/// <summary>
	/// This delegate is used when a movable is added to a scene.
	/// </summary>
	/// <param name="scene">The scene.</param>
	/// <param name="movable">The added movable.</param>
	public delegate void MovableAddedHandler(IScene scene, IMovable movable);
}
