/*
 *  TinySpace by Crealuz - TinySpace.Scene
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
	public sealed class SceneNode : DisposableObject, ISceneNode
	{
		#region Constructors

		public SceneNode(IScene scene, string name = null)
		{
			this.Name = name;
			this.Scene = scene;
		}

		#endregion

		#region Fields

		private ISceneNode m_parent;

		private volatile bool m_isUpdating = false;

		private List<ISceneNode> m_childAddQueue = new List<ISceneNode>();
		private List<ISceneNode> m_childRemoveQueue = new List<ISceneNode>();
		private List<ISceneNode> m_childList = new List<ISceneNode>();

		private List<IMovable> m_movableAddQueue = new List<IMovable>();
		private List<IMovable> m_movableRemoveQueue = new List<IMovable>();
		private List<IMovable> m_movableList = new List<IMovable>();

		private Vector2? m_globalPosition = null;
		private float? m_globalDepth = null;
		private float? m_globalRotation = null;
		private Vector2? m_globalScale = null;

		private Vector2? m_localPosition = Vector2.Zero;
		private float? m_localDepth = 0.0f;
		private float? m_localRotation = 0.0f;
		private Vector2? m_localScale = Vector2.One;

		#endregion

		#region Methods

		internal void Update(GameTime gameTime, ICamera camera)
		{
			m_isUpdating = true;

			// Update the child list.
			lock (m_childList)
			{	
				if (m_childList.Count > 0)
				{
					foreach (SceneNode child in m_childList)
					{
						child.Update(gameTime, camera);
					}
				}

				// If any childs where added or removed during the update, apply those changes now.
				if (m_childRemoveQueue.Count > 0)
				{
					lock (m_childRemoveQueue)
					{
						foreach (var child in m_childRemoveQueue)
						{
							m_childList.Remove(child);
						}
						m_childRemoveQueue.Clear();
					}
				}

				if (m_childAddQueue.Count > 0)
				{
					lock (m_childAddQueue)
					{
						foreach (var child in m_childAddQueue)
						{
							m_childList.Add(child);
						}
						m_childAddQueue.Clear();
					}
				}
			}

			// Update the movable list.
			lock (m_movableList)
			{
				var scene = this.Scene;

				if (m_movableList.Count > 0)
				{
					foreach (var movable in m_movableList)
					{
						if (movable is IUpdatable)
						{
							((IUpdatable)movable).Update(gameTime, camera);
						}
					}
				}

				// If any movables where added or removed during the update, apply those changes now.
				if (m_movableRemoveQueue.Count > 0)
				{
					lock (m_movableRemoveQueue)
					{
						foreach (var movable in m_movableRemoveQueue)
						{
							m_movableList.Remove(movable);
							scene.RemoveMovable(movable);
						}
						m_movableRemoveQueue.Clear();
					}
				}

				if (m_movableAddQueue.Count > 0)
				{
					lock (m_movableAddQueue)
					{
						foreach (var movable in m_movableAddQueue)
						{
							m_movableList.Add(movable);
							scene.AddMovable(movable);
						}
						m_movableAddQueue.Clear();
					}
				}
			}

			m_isUpdating = false;
		}

		private void NotifyChildsOfTransform()
		{
			lock (m_childList)
			{
				foreach (var child in m_childList)
				{
					child.UpdateTransformation();
				}
			}
		}

		protected override void DisposeUnmanaged()
		{
			Remove();

			lock (m_movableList)
			{
				var scene = this.Scene;

				foreach (var movable in m_movableList)
				{
					if (movable is IDisposable)
					{
						((IDisposable)movable).Dispose();
					}

					scene.RemoveMovable(movable);
				}

				m_movableList.Clear();
			}

			lock (m_movableAddQueue)
			{
				m_movableAddQueue.Clear();
			}
			lock (m_movableRemoveQueue)
			{
				m_movableRemoveQueue.Clear();
			}

			lock (m_childList)
			{
				for (int i = m_childList.Count - 1; i >= 0; --i)
				{
					m_childList[i].Remove();
				}

				m_childList.Clear();
			}

			lock (m_childAddQueue)
			{
				m_childAddQueue.Clear();
			}
			lock (m_childRemoveQueue)
			{
				m_childRemoveQueue.Clear();
			}
		}

		#endregion

		#region ISceneNode Members

		#region Properties

		public string Name
		{
			get;
			private set;
		}

		public IScene Scene
		{
			get;
			private set;
		}

		public ISceneNode Parent
		{
			get { return m_parent; }
			set
			{
				if (m_parent == null && value != null)
				{
					m_parent = value;
					m_parent.AddChild(this);
				}
				else if (m_parent != value && value != null)
				{
					var previousParent = m_parent;
					m_parent = null;

					previousParent.RemoveChild(this);

					m_parent = value;
					m_parent.AddChild(this);
				}
				else if (m_parent != null && value == null)
				{
					var previousParent = m_parent;
					m_parent = null;

					previousParent.RemoveChild(this);
				}
			}
		}

		public Vector2 Position
		{
			get
			{
				EnsureGlobalTransformation();
				return (Vector2)m_globalPosition;
			}
			set
			{
				m_globalPosition = value;

				EnsureGlobalTransformation();

				m_localPosition = null; // Reset local position.
				m_localDepth = null; // Reset local depth;
				m_localRotation = null; // Reset local rotation.
				m_localScale = null; // Reset local scale.

				NotifyChildsOfTransform();
			}
		}

		public float Depth
		{
			get
			{
				EnsureGlobalTransformation();
				return (float)m_globalDepth;
			}
			set
			{
				m_globalDepth = value;

				EnsureGlobalTransformation();

				m_localPosition = null; // Reset local position.
				m_localDepth = null; // Reset local depth;
				m_localRotation = null; // Reset local rotation.
				m_localScale = null; // Reset local scale.

				NotifyChildsOfTransform();
			}
		}

		public float Rotation
		{
			get
			{
				EnsureGlobalTransformation();
				return (float)m_globalRotation;
			}
			set
			{
				m_globalRotation = MathCore.WrapAngle(value);
				
				EnsureGlobalTransformation();

				m_localPosition = null; // Reset local position.
				m_localDepth = null; // Reset local depth;
				m_localRotation = null; // Reset local rotation.
				m_localScale = null; // Reset local scale.

				NotifyChildsOfTransform();
			}
		}

		public Vector2 Scale
		{
			get
			{
				EnsureGlobalTransformation();
				return (Vector2)m_globalScale;
			}
			set
			{
				m_globalScale = value;

				EnsureGlobalTransformation();

				m_localPosition = null; // Reset local position.
				m_localDepth = null; // Reset local depth;
				m_localRotation = null; // Reset local rotation.
				m_localScale = null; // Reset local scale.

				NotifyChildsOfTransform();
			}
		}

		public Vector2 LocalPosition
		{
			get
			{
				EnsureLocalTransformation();
				return (Vector2)m_localPosition;
			}
			set
			{
				m_localPosition = value;

				EnsureLocalTransformation();

				m_globalPosition = null; // Reset global position.
				m_globalDepth = null; // Reset global depth.
				m_globalRotation = null; // Reset global rotation.
				m_globalScale = null; // Reset global scale.

				NotifyChildsOfTransform();
			}
		}

		public float LocalDepth
		{
			get
			{
				EnsureLocalTransformation();
				return (float)m_localDepth;
			}
			set
			{
				m_localRotation = value;

				EnsureLocalTransformation();

				m_globalPosition = null; // Reset global position.
				m_globalDepth = null; // Reset global depth.
				m_globalRotation = null; // Reset global rotation.
				m_globalScale = null; // Reset global scale.

				NotifyChildsOfTransform();
			}
		}

		public float LocalRotation
		{
			get
			{
				EnsureLocalTransformation();
				return (float)m_localRotation;
			}
			set
			{
				m_localRotation = MathCore.WrapAngle(value);

				EnsureLocalTransformation();

				m_globalPosition = null; // Reset global position.
				m_globalDepth = null; // Reset global depth.
				m_globalRotation = null; // Reset global rotation.
				m_globalScale = null; // Reset global scale.

				NotifyChildsOfTransform();
			}
		}

		public Vector2 LocalScale
		{
			get
			{
				EnsureLocalTransformation();
				return (Vector2)m_localScale;
			}
			set
			{
				m_localScale = value;

				EnsureLocalTransformation();

				m_globalPosition = null; // Reset global position.
				m_globalDepth = null; // Reset global depth.
				m_globalRotation = null; // Reset global rotation.
				m_globalScale = null; // Reset global scale.

				NotifyChildsOfTransform();
			}
		}

		public IEnumerable<ISceneNode> Childs
		{
			get
			{
				IEnumerable<ISceneNode> result;
				lock (m_childList)
				{
					result = m_childList.ToArray();
				}

				return result;
			}
		}

		public IEnumerable<IMovable> Movables
		{
			get
			{
				IEnumerable<IMovable> result;
				lock (m_movableList)
				{
					result = m_movableList.ToArray();
				}

				return result;
			}
		}

		#endregion

		#region Methods

		public ISceneNode CreateChild(string name = null)
		{
			var child = new SceneNode(this.Scene, name);
			AddChild(child);

			return child;
		}

		public void AddChild(ISceneNode child)
		{
			if (m_childAddQueue.Contains(child) == false && m_childList.Contains(child) == false)
			{
				if (m_isUpdating == true)
				{
					lock (m_childAddQueue)
					{
						if (m_childAddQueue.Contains(child) == false && m_childList.Contains(child) == false)
						{
							m_childAddQueue.Add(child);
							if (child.Parent != this)
							{
								child.Parent = this;
							}
						}
					}
				}
				else
				{
					lock (m_childList)
					{
						if (m_childAddQueue.Contains(child) == false && m_childList.Contains(child) == false)
						{
							m_childList.Add(child);
							if (child.Parent != this)
							{
								child.Parent = this;
							}
						}
					}
				}
			}
		}

		public void RemoveChild(ISceneNode child)
		{
			if (m_childAddQueue.Contains(child) == true)
			{
				lock (m_childAddQueue)
				{
					if (m_childAddQueue.Contains(child) == true)
					{
						m_childAddQueue.Remove(child);
						if (child.Parent == this)
						{
							child.Parent = null;
						}
					}
				}
			}
			else if (m_childRemoveQueue.Contains(child) == true)
			{
				// Do nothing.
			}
			else if (m_childList.Contains(child) == true)
			{
				if (m_isUpdating == true)
				{
					lock (m_childRemoveQueue)
					{
						if (m_childRemoveQueue.Contains(child) == false)
						{
							m_childRemoveQueue.Add(child);
							if (child.Parent == this)
							{
								child.Parent = null;
							}
						}
					}
				}
				else
				{
					lock (m_childList)
					{
						m_childList.Remove(child);
						if (child.Parent == this)
						{
							child.Parent = null;
						}
					}
				}
			}
		}

		public void Remove()
		{
			this.Parent = null;
		}

		public void Attach(IMovable movable)
		{
			if (m_movableAddQueue.Contains(movable) == false && m_movableList.Contains(movable) == false)
			{
				if (m_isUpdating == true)
				{
					lock (m_movableAddQueue)
					{
						if (m_movableAddQueue.Contains(movable) == false && m_movableList.Contains(movable) == false)
						{
							m_movableAddQueue.Add(movable);
							if (movable.SceneNode != this)
							{
								movable.AttachTo(this);
							}
						}
					}
				}
				else
				{
					lock (m_movableList)
					{
						if (m_movableAddQueue.Contains(movable) == false && m_movableList.Contains(movable) == false)
						{
							m_movableList.Add(movable);
							if (movable.SceneNode != this)
							{
								movable.AttachTo(this);
							}
							this.Scene.AddMovable(movable);
						}
					}
				}
			}
		}

		public void Detach(IMovable movable)
		{
			if (m_movableAddQueue.Contains(movable) == true)
			{
				lock (m_movableAddQueue)
				{
					if (m_movableAddQueue.Contains(movable) == true)
					{
						m_movableAddQueue.Remove(movable);
						if (movable.SceneNode == this)
						{
							movable.Detach();
						}
					}
				}
			}
			else if (m_movableRemoveQueue.Contains(movable) == true)
			{
				// Do nothing.
			}
			else if (m_movableList.Contains(movable) == true)
			{
				if (m_isUpdating == true)
				{
					lock (m_movableRemoveQueue)
					{
						if (m_movableRemoveQueue.Contains(movable) == false)
						{
							m_movableRemoveQueue.Add(movable);
							if (movable.SceneNode == this)
							{
								movable.Detach();
							}
						}
					}
				}
				else
				{
					lock (m_movableList)
					{
						m_movableList.Remove(movable);
						if (movable.SceneNode == this)
						{
							movable.Detach();
						}
						this.Scene.RemoveMovable(movable);
					}
				}
			}
		}

		public T FindMovable<T>()
			where T : IMovable
		{
			IMovable result = null;

			lock (m_movableList)
			{
				foreach (var movable in m_movableList)
				{
					if (movable is T)
					{
						result = movable;
						break;
					}
				}
			}

			if (result == null)
			{
				lock (m_movableAddQueue)
				{
					foreach (var movable in m_movableAddQueue)
					{
						if (movable is T)
						{
							result = movable;
							break;
						}
					}
				}
			}

			return (T)result;
		}

		public void UpdateTransformation()
		{
			// When our parent moves, make sure we only know our local transformation, so we can
			// calculate our global transformation again when it's needed.

			// Ensure the local variables.
			EnsureLocalTransformation();

			// Clear the globals.
			m_globalPosition = null;
			m_globalDepth = null;
			m_globalRotation = null;

			// Also notify our childs.
			NotifyChildsOfTransform();
		}

		public void Move(Vector2 distance)
		{
			if (m_globalPosition != null)
			{
				var position = m_globalPosition.Value;
				position += distance;
				m_globalPosition = position;
			}
			else if (m_localPosition != null)
			{
				var position = m_localPosition.Value;
				position += distance;
				m_localPosition = position;
			}

			NotifyChildsOfTransform();
		}

		private void EnsureGlobalTransformation()
		{
			if (this.Parent != null)
			{
				if (m_globalPosition == null)
				{
					var rotationRadians = MathHelper.ToRadians(this.Parent.Rotation);
					var sin = (float)Math.Sin(rotationRadians);
					var cos = (float)Math.Cos(rotationRadians);

					var localPosition = m_localPosition.GetValueOrDefault(Vector2.Zero);

					m_globalPosition = this.Parent.Position + new Vector2(localPosition.X * cos + localPosition.Y * sin, localPosition.X * sin + localPosition.Y * cos);
				}

				if (m_globalDepth == null)
				{
					m_globalDepth = this.Parent.Depth + m_localDepth.GetValueOrDefault(0.0f);
				}

				if (m_globalRotation == null)
				{
					m_globalRotation = MathCore.WrapAngle(this.Parent.Rotation + m_localRotation.GetValueOrDefault(0.0f));
				}

				if (m_globalScale == null)
				{
					m_globalScale = this.Parent.Scale * m_localScale.GetValueOrDefault(Vector2.One);
				}
			}
			else
			{
				if (m_globalPosition == null)
				{
					m_globalPosition = m_localPosition.GetValueOrDefault(Vector2.Zero);
				}

				if (m_globalDepth == null)
				{
					m_globalDepth = m_localDepth.GetValueOrDefault(0.0f);
				}

				if (m_globalRotation == null)
				{
					m_globalRotation = MathCore.WrapAngle(m_localRotation.GetValueOrDefault(0.0f));
				}

				if (m_globalScale == null)
				{
					m_globalScale = m_localScale.GetValueOrDefault(Vector2.One);
				}
			}
		}

		private void EnsureLocalTransformation()
		{
			if (this.Parent != null)
			{
				if (m_localPosition == null)
				{
					var rotationRadians = MathHelper.ToRadians(-this.Parent.Rotation);
					var sin = (float)Math.Sin(rotationRadians);
					var cos = (float)Math.Cos(rotationRadians);

					var globalPosition = m_localPosition.GetValueOrDefault(Vector2.Zero);

					m_localPosition = new Vector2(globalPosition.X * cos + globalPosition.Y * sin, globalPosition.X * sin + globalPosition.Y * cos) - this.Parent.Position;
				}

				if (m_localDepth == null)
				{
					m_localDepth = m_globalDepth.GetValueOrDefault(0.0f) - this.Parent.Depth;
				}

				if (m_localRotation == null)
				{
					m_localRotation = MathCore.WrapAngle(m_globalRotation.GetValueOrDefault(0.0f) - this.Parent.Rotation);
				}

				if (m_localScale == null)
				{
					m_localScale = m_globalScale.GetValueOrDefault(Vector2.One) / this.Parent.Scale;
				}
			}
			else
			{
				if (m_localPosition == null)
				{
					m_localPosition = m_globalPosition.GetValueOrDefault(Vector2.Zero);
				}

				if (m_localDepth == null)
				{
					m_localDepth = m_globalDepth.GetValueOrDefault(0.0f);
				}

				if (m_localRotation == null)
				{
					m_localRotation = MathCore.WrapAngle(m_globalRotation.GetValueOrDefault(0.0f));
				}

				if (m_localScale == null)
				{
					m_localScale = m_globalScale.GetValueOrDefault(Vector2.One);
				}
			}
		}

		#endregion

		#endregion
	}
}
