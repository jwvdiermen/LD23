/*
 *  IMovable.cs
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
	/// This interface represents a movable object which can be attached to an <see cref="ISceneNode" />.
	/// Optionally, the movable can be assigned a name.
	/// </summary>
	public interface IMovable : INamable
	{
		#region Properties

		/// <summary>
		/// Gets the scene node to which the movable is attached.
		/// </summary>
		ISceneNode SceneNode
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method attaches the movable to the given scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node to attach the movable to.</param>
		/// <remarks>If the movable is already attached to another scene node, it is first detached.</remarks>
		void AttachTo(ISceneNode sceneNode);

		/// <summary>
		/// This method detaches the movable from the current scene node.
		/// </summary>
		void Detach();

		#endregion
	}
}
