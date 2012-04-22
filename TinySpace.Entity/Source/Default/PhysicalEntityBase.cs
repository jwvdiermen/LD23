/*
 *  PhysicalEntityBase.cs
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using FarseerPhysics.Dynamics;

using TinySpace.Scene;
using TinySpace.Core;

namespace TinySpace.Entity
{
	/// <summary>
	/// This abstract class provides a base implementation of the <see cref="IPhysicalEntity" /> interface.
	/// </summary>
	public abstract class PhysicalEntityBase : EntityBase, IPhysicalEntity
	{
		#region Constructors

		/// <summary>
		/// This constructor creates a new instance of a physical entity.
		/// </summary>
		/// <param name="name">The optional name.</param>
		protected PhysicalEntityBase(string name = null)
			: base(name)
		{
		}

		#endregion

		#region Fields

		private Body m_body;

		private Vector2? m_position;
		private float? m_rotation;

		#endregion

		#region Properties

		public override Vector2 Position
		{
			get { return m_body != null ? m_body.Position : m_position.GetValueOrDefault(Vector2.Zero); }
			set
			{
				if (m_body != null)
				{
					m_body.Position = value;
				}
				else
				{
					m_position = value;
				}
			}
		}

		public override float Rotation
		{
			get { return m_body != null ? MathHelper.ToDegrees(m_body.Rotation) : m_rotation.GetValueOrDefault(0.0f); }
			set 
			{
				if (m_body != null)
				{
					m_body.Rotation = MathHelper.ToRadians(value);
				}
				else
				{
					m_rotation = value;
				}
			}
		}

		public override bool IsActive
		{
			get { return m_body.Awake; }
			set
			{
				if (m_body != null)
				{
					m_body.Awake = value;
				}
			}
		}

		#endregion

		#region Methods

		public override void Update(GameTime gameTime, ICamera camera)
		{
			base.Update(gameTime, camera);

			this.SceneNode.Position = this.Body.Position;
			this.SceneNode.Rotation = MathHelper.ToDegrees(this.Body.Rotation);
		}

		#endregion

		#region IPhysicalEntity Members

		public Body Body
		{
			get { return m_body; }
			protected set
			{
				m_body = value;
				if (m_position != null)
				{
					m_body.Position = (Vector2)m_position;
					m_position = null;
				}
				if (m_rotation != null)
				{
					m_body.Rotation = MathHelper.ToRadians((float)m_rotation);
					m_rotation = null;
				}
			}
		}

		public virtual void UpdatePhysics(GameTime gameTime, ICamera camera)
		{
		}

		#endregion
	}
}
