/*
 *  IScene.cs
 *  TinySpace by Crealuz - TinySpace.Scene
 *
 *  © copyright 2010 Crealuz All rights reserved.
 *  The computer program(s) is the proprietary information of the original authors. 
 *  and provided under the relevant License Agreement containing restrictions 
 *  on use and disclosure. Use is subject to the License Agreement.
 *
 *  No part of this package may be reproduced and/or published by print, 
 *  photoprint, microfilm, audiotape, electronically, mechanically or any other means, 
 *  or stored in an information retrieval system, without prior permission from Crealuz.
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TinySpace.Core;

namespace TinySpace.Scene
{
	/// <summary>
	/// This interface represents a scene containing a single root <see cref="ISceneNode" />.
	/// </summary>
	public interface IScene : IDisposableObject
	{
		#region Events

		/// <summary>
		/// This event is fired when a movable is added to the scene.
		/// </summary>
		event MovableAddedHandler MovableAdded;

		/// <summary>
		/// This event is fired when a movable is removed from the scene.
		/// </summary>
		event MovableRemovedHandler MovableRemoved;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the root scene node.
		/// </summary>
		ISceneNode Root
		{
			get;
		}

		/// <summary>
		/// Gets an enumerable containing the layers.
		/// </summary>
		IEnumerable<ILayer> Layers
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method update the scene.
		/// </summary>
		/// <param name="time">The game time.</param>
		/// <param name="camera">The camera.</param>
		void Update(GameTime time, ICamera camera);
		
		/// <summary>
		/// This method adds the given layer.
		/// </summary>
		/// <param name="layer">The layer to add.</param>
		void AddLayer(ILayer layer);

		/// <summary>
		/// This method removes the given layer.
		/// </summary>
		/// <param name="layer">The layer.</param>
		void RemoveLayer(ILayer layer);

		/// <summary>
		/// This method creates a new scene node with the given optional name.
		/// </summary>
		/// <param name="name">The optional name of the scene node.</param>
		/// <returns>The created scene node.</returns>
		ISceneNode CreateSceneNode(string name = null);

		/// <summary>
		/// This method tells the scene to monitor for the given type so it can be collected.
		/// </summary>
		/// <param name="type">The type to monitor for.</param>
		void Monitor(Type type);

		/// <summary>
		/// This method adds a movable to the scene.
		/// </summary>
		/// <param name="movable">The movable to add.</param>
		/// <remarks>This method is mostly called from scene nodes when a movable is attached.</remarks>
		void AddMovable(IMovable movable);

		/// <summary>
		/// This method removes a movable from the scene.
		/// </summary>
		/// <param name="movable">The movable to remove.</param>
		/// <remarks>This method is mostly called from scene nodes when a movable is attached.</remarks>
		void RemoveMovable(IMovable movable);

		/// <summary>
		/// This method collects objects of the given type. These objects are represented by
		/// movables which can be casted to the given type.
		/// </summary>
		/// <param name="type">The type of the objects to collect.</param>
		/// <returns>A read-only list containing the collected objects.</returns>
		ReadOnlyList<object> Collect(Type type);

		#endregion
	}
}
