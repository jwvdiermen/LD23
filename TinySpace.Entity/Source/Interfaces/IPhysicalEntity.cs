/*
 *  IEntity.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TinySpace.Scene;
using FarseerPhysics.Dynamics;

namespace TinySpace.Entity
{
	/// <summary>
	/// This interface represents an entity with physical properties.
	/// </summary>
	public interface IPhysicalEntity : IEntity
	{
		#region Properties

		/// <summary>
		/// Gets the physical body.
		/// </summary>
		Body Body
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method updates the entity's physics.
		/// </summary>
		/// <param name="gameTime">The game time.</param>
		/// <param name="camera">The camera.</param>
		void UpdatePhysics(GameTime gameTime, ICamera camera);

		#endregion
	}
}
