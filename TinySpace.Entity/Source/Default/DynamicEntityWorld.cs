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
using FarseerPhysics.Dynamics;

namespace TinySpace.Entity
{
	/// <summary>
	/// This class implements the <see cref="IEntityWorld" /> interface and provides a dynamic entity world
	/// with physics using FarseerPhysics.
	/// </summary>
	public class DynamicEntityWorld : DisposableObject, IEntityWorld
	{
		#region Constructors

		/// <summary>
		/// This constructor creates a new static entity world using a default scene (<see cref="DefaultScene" />).
		/// </summary>
		/// <param name="services">The service provider.</param>
		public DynamicEntityWorld(IServiceProvider services)
			: this(services, new DefaultScene())
		{
		}

		/// <summary>
		/// This constructor creates a new static entity world using the given scene.
		/// </summary>
		/// <param name="services">The service provider.</param>
		/// <param name="scene">The scene to use.</param>
		public DynamicEntityWorld(IServiceProvider services, IScene scene)
		{
			m_services = services;

			m_scene = scene;
			m_scene.MovableAdded += Scene_MovableAdded;
			m_scene.MovableRemoved += Scene_MovableRemoved;

			foreach (IRenderable renderable in m_scene.Collect(typeof(IRenderable)))
			{
				renderable.LoadContent(m_services);
			}

			// Create the physics world.
			m_physicsWorld = new World(Vector2.Zero);
		}

		#endregion

		#region Fields

		private IServiceProvider m_services;

		private IScene m_scene = null;

		private volatile bool m_isUpdating = false;

		private List<IEntity> m_entityAddQueue = new List<IEntity>();
		private List<IEntity> m_entityRemoveQueue = new List<IEntity>();
		private List<IEntity> m_entityList = new List<IEntity>();

		private List<IPhysicalEntity> m_physicalEntityList = new List<IPhysicalEntity>();

		private World m_physicsWorld;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the physics world.
		/// </summary>
		public World PhysicsWorld
		{
			get { return m_physicsWorld; }
		}

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
			if (m_entityAddQueue.Contains(entity) == false && m_entityList.Contains(entity) == false)
			{
				if (m_isUpdating == true)
				{
					lock (m_entityAddQueue)
					{
						if (m_entityAddQueue.Contains(entity) == false && m_entityList.Contains(entity) == false)
						{
							m_entityAddQueue.Add(entity);
							if (entity.World != this)
							{
								entity.World = this;
							}
						}
					}
				}
				else
				{
					lock (m_entityList)
					{
						if (m_entityAddQueue.Contains(entity) == false && m_entityList.Contains(entity) == false)
						{
							m_entityList.Add(entity);
							if (entity.World != this)
							{
								entity.World = this;
							}
							entity.LoadContent(m_services);
						}
					}
				}
			}

			lock (m_physicalEntityList)
			{
				var physicalEntity = entity as IPhysicalEntity;
				if (physicalEntity != null && m_physicalEntityList.Contains(physicalEntity) == false)
				{
					m_physicalEntityList.Add(physicalEntity);
				}
			}
		}

		public void Remove(IEntity entity)
		{
			if (m_entityAddQueue.Contains(entity) == true)
			{
				lock (m_entityAddQueue)
				{
					if (m_entityAddQueue.Contains(entity) == true)
					{
						m_entityAddQueue.Remove(entity);
						if (entity.World == this)
						{
							entity.World = null;
						}
					}
				}
			}
			else if (m_entityRemoveQueue.Contains(entity) == true)
			{
				// Do nothing.
			}
			else if (m_entityList.Contains(entity) == true)
			{
				if (m_isUpdating == true)
				{
					lock (m_entityRemoveQueue)
					{
						if (m_entityRemoveQueue.Contains(entity) == false)
						{
							m_entityRemoveQueue.Add(entity);
							if (entity.World == this)
							{
								entity.World = null;
							}
						}
					}
				}
				else
				{
					lock (m_entityList)
					{
						m_entityList.Remove(entity);
						entity.UnloadContent();
						if (entity.World == this)
						{
							entity.World = null;
						}
					}
				}
			}

			lock (m_physicalEntityList)
			{
				var physicalEntity = entity as IPhysicalEntity;
				if (physicalEntity != null && m_physicalEntityList.Contains(physicalEntity))
				{
					m_physicalEntityList.Remove(physicalEntity);
				}
			}
		}

		//public void Add(IEntity entity)
		//{
		//    lock (m_entityList)
		//    {
		//        if (m_entityList.Contains(entity) == false)
		//        {
		//            m_entityList.Add(entity);

		//            entity.World = this;
		//            entity.LoadContent(m_services);
		//        }

		//        var physicalEntity = entity as IPhysicalEntity;
		//        if (physicalEntity != null && m_physicalEntityList.Contains(physicalEntity) == false)
		//        {
		//            m_physicalEntityList.Add(physicalEntity);
		//        }
		//    }
		//}

		//public void Remove(IEntity entity)
		//{
		//    lock (m_entityList)
		//    {
		//        if (m_entityList.Contains(entity) == true)
		//        {
		//            m_entityList.Remove(entity);

		//            entity.UnloadContent();
		//            entity.World = null;
		//        }

		//        var physicalEntity = entity as IPhysicalEntity;
		//        if (physicalEntity != null && m_physicalEntityList.Contains(physicalEntity))
		//        {
		//            m_physicalEntityList.Add(physicalEntity);
		//        }
		//    }
		//}

		public void Update(GameTime time, ICamera camera)
		{
			m_isUpdating = true;

			// Update the physical properties of the entities.
			lock (m_physicalEntityList)
			{
				foreach (var entity in m_physicalEntityList)
				{
					entity.UpdatePhysics(time, camera);
				}
			}

			// Update the physics.
			m_physicsWorld.Step((float)time.ElapsedGameTime.TotalSeconds);

			// Update the entities.
			lock (m_entityList)
			{
				ApplyEntityQueues();

				if (m_entityList.Count > 0)
				{
					foreach (var entity in m_entityList)
					{
						entity.Update(time, camera);
					}
				}

				ApplyEntityQueues();
			}

			// Update the scene.
			m_scene.Update(time, camera);

			m_isUpdating = false;
		}

		private void ApplyEntityQueues()
		{
			// If any entities where added or removed during the update, apply those changes now.
			if (m_entityRemoveQueue.Count > 0)
			{
				lock (m_entityRemoveQueue)
				{
					foreach (var entity in m_entityRemoveQueue)
					{
						m_entityList.Remove(entity);
						entity.UnloadContent();
					}
					m_entityRemoveQueue.Clear();
				}
			}

			if (m_entityAddQueue.Count > 0)
			{
				lock (m_entityAddQueue)
				{
					foreach (var entity in m_entityAddQueue)
					{
						m_entityList.Add(entity);
						entity.LoadContent(m_services);
					}
					m_entityAddQueue.Clear();
				}
			}
		}

		#endregion

		#endregion
	}
}
