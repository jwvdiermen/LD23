/*
 *  DefaultScene.cs
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
	/// This class implements the <see cref="IScene" /> interface and provides a default implementation.
	/// </summary>
	public sealed class DefaultScene : DisposableObject, IScene
	{
		#region Constructors

		public DefaultScene()
		{
			m_rootNode = new SceneNode(this, "Root");

			// Monitor defaults.
			Monitor(typeof(IRenderable));
			Monitor(typeof(IUpdatable));
		}

		#endregion

		#region Fields

		private List<ILayer> m_layerList = new List<ILayer>();

		private SceneNode m_rootNode;

		private List<IMovable> m_movableList = new List<IMovable>();
		private List<Type> m_monitorList = new List<Type>();

		private Dictionary<Type, List<object>> m_collectedMovableList = new Dictionary<Type, List<object>>();
		private Dictionary<Type, ReadOnlyList<object>> m_readOnlyCollectedMovableList = new Dictionary<Type, ReadOnlyList<object>>();

		#endregion

		#region Methods

		private void Collect(Type type, SceneNode sceneNode, List<object> movables)
		{
			lock (m_movableList)
			{
				foreach (var movable in m_movableList)
				{
					if (type.IsAssignableFrom(movable.GetType()) == true)
					{
						movables.Add(movable);
					}
				}
			}
		}

		protected override void DisposeUnmanaged()
		{
			m_rootNode.Dispose();
			m_rootNode = null;
		}

		#endregion

		#region IScene Members

		#region Events

		public event MovableAddedHandler MovableAdded;

		private void OnMovableAdded(IMovable movable)
		{
			if (this.MovableAdded != null)
			{
				this.MovableAdded(this, movable);
			}
		}

		public event MovableRemovedHandler MovableRemoved;

		private void OnMovableRemoved(IMovable movable)
		{
			if (this.MovableRemoved != null)
			{
				this.MovableRemoved(this, movable);
			}
		}

		#endregion

		#region Properties

		public IEnumerable<ILayer> Layers
		{
			get { return m_layerList; }
		}

		public ISceneNode Root
		{
			get { return m_rootNode; }
		}

		#endregion

		#region Methods

		public void Update(GameTime time, ICamera camera)
		{
			m_rootNode.Update(time, camera);
		}

		public void AddLayer(ILayer layer)
		{
			if (layer.Scene != this)
			{
				m_layerList.Add(layer);
				layer.Scene = this;
			}
		}

		public void RemoveLayer(ILayer layer)
		{
			if (layer.Scene == this)
			{
				m_layerList.Remove(layer);
				layer.Scene = null;
			}
		}

		public ISceneNode CreateSceneNode(string name = null)
		{
			return new SceneNode(this, name);
		}

		public void Monitor(Type type)
		{
			if (m_monitorList.Contains(type) == false)
			{
				lock (m_monitorList)
				{
					if (m_monitorList.Contains(type) == false)
					{
						m_monitorList.Add(type);

						var movables = new List<object>();

						m_collectedMovableList.Add(type, movables);
						m_readOnlyCollectedMovableList.Add(type, new ReadOnlyList<object>(movables));

						// Collect existing movables.
						Collect(type, m_rootNode, movables);
					}
				}
			}
		}

		public void AddMovable(IMovable movable)
		{
			lock (m_movableList)
			{
				m_movableList.Add(movable);

				lock (m_monitorList)
				{
					var movableType = movable.GetType();
					foreach (var type in m_monitorList)
					{
						if (type.IsAssignableFrom(movableType) == true)
						{
							var collectedMovableList = m_collectedMovableList[type];
							collectedMovableList.Add(movable);
						}
					}
				}
			}

			OnMovableAdded(movable);
		}

		public void RemoveMovable(IMovable movable)
		{
			lock (m_movableList)
			{
				if (m_movableList.Remove(movable) == true)
				{
					lock (m_monitorList)
					{
						var movableType = movable.GetType();
						foreach (var type in m_monitorList)
						{
							if (type.IsAssignableFrom(movableType) == true)
							{
								var collectedMovableList = m_collectedMovableList[type];
								collectedMovableList.Remove(movable);
							}
						}
					}
				}
			}

			OnMovableRemoved(movable);
		}

		public ReadOnlyList<object> Collect(Type type)
		{
			ReadOnlyList<object> result;
			m_readOnlyCollectedMovableList.TryGetValue(type, out result);

			return result;
		}

		#endregion

		#endregion
	}
}
