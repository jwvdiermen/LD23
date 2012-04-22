/*
 *  IEntityWorld.cs
 *  TinySpace by Crealuz - TinySpace.Entity
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

using TinySpace.Scene;
using TinySpace.Core;

namespace TinySpace.Entity
{
	/// <summary>
	/// This interface represents a world in which entities (see <see cref="IEntity" />) are managed.
	/// To visualize the entities, the world also contains an <see cref="IScene" /> containing scene nodes
	/// which are coupled with entities.
	/// </summary>
	public interface IEntityWorld : IDisposableObject
	{
		#region Properties

		/// <summary>
		/// Gets the scene.
		/// </summary>
		IScene Scene
		{
			get;
		}

		/// <summary>
		/// Gets an enumerable containing all the entities in the world.
		/// </summary>
		IEnumerable<IEntity> Entities
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method adds the given entity to the world.
		/// </summary>
		/// <param name="entity">The entity to add.</param>
		void Add(IEntity entity);
		
		/// <summary>
		/// This method removes the given entity from the world.
		/// </summary>
		/// <param name="entity">The entity to remove.</param>
		void Remove(IEntity entity);

		/// <summary>
		/// This method updates the world.
		/// </summary>
		/// <param name="time">The game time.</param>
		/// <param name="camera">The camera.</param>
		void Update(GameTime time, ICamera camera);

		#endregion
	}
}
