using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Scene
{
	/// <summary>
	/// This delegate is used when a movable is removed from a scene.
	/// </summary>
	/// <param name="scene">The scene.</param>
	/// <param name="movable">The removed movable.</param>
	public delegate void MovableRemovedHandler(IScene scene, IMovable movable);
}
