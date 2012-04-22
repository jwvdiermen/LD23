/*
 *  IEntityController.cs
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

namespace TinySpace.Entity
{
	/// <summary>
	/// This interface represents a controller which can be attached to an <see cref="IEntity" /> which
	/// is then automatically updated by the entity. A controller is used for controlling the behaviour
	/// of an entity, like handling player input.
	/// </summary>
	public interface IEntityController
	{
		#region Properties

		/// <summary>
		/// Gets the entity to which the controller is attached.
		/// </summary>
		IEntity Entity
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method attaches the controller to the given entity.
		/// </summary>
		/// <param name="entity">The entity to attach the controller to.</param>
		/// <remarks>If the controller is already attached to another entity, it is first detached.</remarks>
		void AttachTo(IEntity entity);

		/// <summary>
		/// This method detaches the controller from the current entity.
		/// </summary>
		void Detach();

		/// <summary>
		/// This method updates the entity controller.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		/// <param name="camera">The camera.</param>
		void Update(GameTime gameTime, ICamera camera);

		#endregion
	}
}
