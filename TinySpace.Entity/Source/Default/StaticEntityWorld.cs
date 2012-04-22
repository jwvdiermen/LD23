/*
 *  TinySpace by Crealuz - TinySpace.Entity
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

using TinySpace.Scene;
using TinySpace.Core;

namespace TinySpace.Entity
{
	/// <summary>
	/// This class implements the <see cref="IEntityWorld" /> interface and provides a static entity world
	/// without physics.
	/// </summary>
	public class StaticEntityWorld : DisposableObject, IEntityWorld
	{
		#region Constructors

		/// <summary>
		/// This constructor creates a new static entity world using a default scene (<see cref="DefaultScene" />).
		/// </summary>
		/// <param name="services">The service provider.</param>
		public StaticEntityWorld(IServiceProvider services)
			: this(services, new DefaultScene())
		{
		}

		/// <summary>
		/// This constructor creates a new static entity world using the given scene.
		/// </summary>
		/// <param name="services">The service provider.</param>
		/// <param name="scene">The scene to use.</param>
		public StaticEntityWorld(IServiceProvider services, IScene scene)
		{
			m_services = services;

			m_scene = scene;
			m_scene.MovableAdded += Scene_MovableAdded;
			m_scene.MovableRemoved += Scene_MovableRemoved;

			foreach (IRenderable renderable in m_scene.Collect(typeof(IRenderable)))
			{
				renderable.LoadContent(m_services);
			}
		}

		#endregion

		#region Fields

		private IServiceProvider m_services;

		private IScene m_scene = null;
		private List<IEntity> m_entityList = new List<IEntity>();

		#endregion

		#region Methods

		private void Scene_MovableAdded(IScene scene, IMovable movable)
		{
			var renderable = movable as IRenderable;
			if (renderable != null)
			{
				renderable.LoadContent(m_services);
			}
		}

		private void Scene_MovableRemoved(IScene scene, IMovable movable)
		{
			var renderable = movable as IRenderable;
			if (renderable != null)
			{
				renderable.UnloadContent();
			}
		}

		protected override void DisposeManaged()
		{
			if (m_scene != null)
			{
				m_scene.Dispose();
				m_scene = null;
			}

			if (m_entityList.Count > 0)
			{
				lock (m_entityList)
				{
					if (m_entityList.Count > 0)
					{
						for (int i = m_entityList.Count - 1; i >= 0; --i)
						{
							var entity = m_entityList[i];

							var disposable = entity as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
						m_entityList.Clear();
					}
				}
			}
		}

		#endregion

		#region IEntityWorld Members

		#region Properties

		public IScene Scene
		{
			get { return m_scene; }
		}

		public IEnumerable<IEntity> Entities
		{
			get { return m_entityList; }
		}

		#endregion

		#region Methods

		public void Add(IEntity entity)
		{
			lock (m_entityList)
			{
				if (m_entityList.Contains(entity) == false)
				{
					m_entityList.Add(entity);

					entity.World = this;
					entity.LoadContent(m_services);
				}
			}
		}

		public void Remove(IEntity entity)
		{
			lock (m_entityList)
			{
				if (m_entityList.Contains(entity) == true)
				{
					m_entityList.Remove(entity);

					entity.UnloadContent();
					entity.World = null;
				}
			}
		}

		public void Update(GameTime time, ICamera camera)
		{
			// Update the entities.
			foreach (var entity in m_entityList)
			{
				if (entity.IsActive == true)
				{
					entity.Update(time, camera);
				}
			}

			// Update the scene.
			m_scene.Update(time, camera);
		}

		#endregion

		#endregion
	}
}
