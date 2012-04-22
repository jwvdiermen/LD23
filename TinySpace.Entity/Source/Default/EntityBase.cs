/*
 *  EntityBase.cs
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TinySpace.Entity
{
	/// <summary>
	/// This abstract class provides a base implementation of the <see cref="IEntity" /> interface.
	/// </summary>
	public abstract class EntityBase : DisposableObject, IEntity
	{
		#region Constructors

		/// <summary>
		/// This constructor creates a new instance of an entity.
		/// </summary>
		/// <param name="name">The optional name.</param>
		protected EntityBase(string name = null)
		{
			this.Name = name;
			this.IsActive = true;
		}

		#endregion

		#region Fields

		private IEntityWorld m_entityWorld = null;
		private ISceneNode m_sceneNode = null;
		private List<IEntityController> m_controllerList = null;

		private Vector2? m_position;
		private float? m_rotation;

		#endregion

		#region Methods

		/// <summary>
		/// This method is called when a new world has been set for the entity.
		/// </summary>
		/// <param name="world">The new world. Can be null.</param>
		protected virtual void OnWorldChanged(IEntityWorld world)
		{
		}

		#endregion

		#region IEntity Members

		#region Properties

		public string Name
		{
			get;
			private set;
		}

		public IEntityWorld World
		{
			get { return m_entityWorld; }
			set
			{
				if (m_entityWorld == null && value != null)
				{
					m_entityWorld = value;
					m_entityWorld.Add(this);

					OnWorldChanged(m_entityWorld);
				}
				else if (m_entityWorld != value && value != null)
				{
					var previousWorld = m_entityWorld;
					m_entityWorld = null;

					previousWorld.Remove(this);

					m_entityWorld = value;
					m_entityWorld.Add(this);

					OnWorldChanged(m_entityWorld);
				}
				else if (m_entityWorld != null && value == null)
				{
					var previousWorld = m_entityWorld;
					m_entityWorld = null;

					previousWorld.Remove(this);

					OnWorldChanged(null);
				}
			}
		}

		public ISceneNode SceneNode
		{
			get { return m_sceneNode; }
			protected set 
			{ 
				m_sceneNode = value;
				if (m_position != null)
				{
					m_sceneNode.Position = (Vector2)m_position;
					m_position = null;
				}
				if (m_rotation != null)
				{
					m_sceneNode.Rotation = (float)m_rotation;
					m_rotation = null;
				}
			}
		}

		public virtual Vector2 Position
		{
			get { return m_sceneNode != null ? m_sceneNode.Position : m_position.GetValueOrDefault(Vector2.Zero); }
			set
			{
				if (m_sceneNode != null)
				{
					m_sceneNode.Position = value;
				}
				else
				{
					m_position = value;
				}
			}
		}

		public virtual float Rotation
		{
			get { return m_sceneNode != null ? m_sceneNode.Rotation : m_rotation.GetValueOrDefault(0.0f); }
			set
			{
				if (m_sceneNode != null)
				{
					m_sceneNode.Rotation = value;
				}
				else
				{
					m_rotation = value;
				}
			}
		}

		public virtual bool IsActive
		{
			get;
			set;
		}

		#endregion

		#region Methods

		public virtual void LoadContent(IServiceProvider services)
		{
		}

		public virtual void UnloadContent()
		{
		}

		public void Attach(IEntityController controller)
		{
			if (m_controllerList == null || (m_controllerList != null && m_controllerList.Contains(controller) == false))
			{
				m_controllerList = m_controllerList ?? new List<IEntityController>();
				m_controllerList.Add(controller);

				if (controller.Entity != this)
				{
					controller.AttachTo(this);
				}
			}
		}

		public void Detach(IEntityController controller)
		{
			if (m_controllerList != null && m_controllerList.Contains(controller) == true)
			{
				m_controllerList.Remove(controller);

				if (m_controllerList.Count == 0)
				{
					m_controllerList = null;
				}

				if (controller.Entity == this)
				{
					controller.Detach();
				}
			}
		}

		public virtual void Update(GameTime gameTime, ICamera camera)
		{
			if (m_controllerList != null)
			{
				foreach (var controller in m_controllerList)
				{
					controller.Update(gameTime, camera);
				}
			}
		}

		public virtual void Destroy()
		{
			this.World = null;
			Dispose();
		}

		#endregion

		#endregion
	}
}
