/*
 *  Movable.cs
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
	/// This class provides an abstract implementation of the <see cref="IMovable" /> interface.
	/// </summary>
	public abstract class Movable : DisposableObject, IMovable
	{
		#region Constructors

		/// <summary>
		/// This constructor creates a new movable with the given name.
		/// </summary>
		/// <param name="name">The optional name of the movable.</param>
		protected Movable(string name = null)
		{
			this.Name = name;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method is called when the movable is attached to the given scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node to which the movable was attached.</param>
		protected virtual void OnAttached(ISceneNode sceneNode)
		{
		}

		/// <summary>
		/// This method is called when the movable has been detached from the current scene node.
		/// </summary>
		protected virtual void OnDetached()
		{
		}

		protected override void DisposeUnmanaged()
		{
			Detach();
		}

		#endregion

		#region IMovable Members

		#region Properties

		public string Name
		{
			get;
			private set;
		}

		public ISceneNode SceneNode
		{
			get;
			private set;
		}

		#endregion

		#region Methods

		public void AttachTo(ISceneNode sceneNode)
		{
			if (this.SceneNode != sceneNode)
			{
				Detach();

				// Set the scene node first, because calling Attach on the scene node will also call
				// the AttachTo method on the movable. Setting this property first will prevent a
				// circular method call thanks to the previous if statement.
				this.SceneNode = sceneNode;

				this.SceneNode.Attach(this);
				OnAttached(sceneNode);
			}
		}

		public void Detach()
		{
			if (this.SceneNode != null)
			{
				// Reset the scene node first, because calling Detach on the scene node will also call
				// the Detach method on the movable. Setting this property to null first will prevent a
				// circular method call thanks to the previous if statement.
				var previous = this.SceneNode;
				this.SceneNode = null;

				previous.Detach(this);
				OnDetached();
			}
		}

		#endregion
	
		#endregion
	}
}
