using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinySpace.Core;

namespace TinySpace.Scene
{
	/// <summary>
	/// This layer automatically collects any renderable from the scene that implements the given type.
	/// </summary>
	public class DefaultLayer : DisposableObject, ILayer
	{
		#region Constructors

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="depth">The depth.</param>
		public DefaultLayer(float depth)
		{
			this.RenderableTypes = new Type[0];
			this.StartsWithName = null;
			this.Depth = depth;

			m_readOnlyRenderableList = new ReadOnlyList<IRenderable>(m_renderableList);
		}

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="renderableType">The renderable type.</param>
		/// <param name="startsWithName">If not null, only collect renderables with a name that start with the given value.</param>
		/// <param name="depth">The depth.</param>
		public DefaultLayer(Type renderableType, string startsWithName, float depth)
		{
			this.RenderableTypes = new Type[] { renderableType };
			this.StartsWithName = startsWithName;
			this.Depth = depth;

			m_readOnlyRenderableList = new ReadOnlyList<IRenderable>(m_renderableList);
		}

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="renderableTypes">The renderable types.</param>
		/// <param name="startsWithName">If not null, only collect renderables with a name that start with the given value.</param>
		/// <param name="depth">The depth.</param>
		public DefaultLayer(Type[] renderableTypes, string startsWithName, float depth)
		{
			this.RenderableTypes = renderableTypes;
			this.StartsWithName = startsWithName;
			this.Depth = depth;

			m_readOnlyRenderableList = new ReadOnlyList<IRenderable>(m_renderableList);
		}

		#endregion

		#region Fields

		private IScene m_scene = null;

		private List<IRenderable> m_renderableList = new List<IRenderable>();
		private ReadOnlyList<IRenderable> m_readOnlyRenderableList = null;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the types that renderables must implement atleast one of to be added to the scene.
		/// </summary>
		public IEnumerable<Type> RenderableTypes
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the value that names of renderables should start with.
		/// </summary>
		public string StartsWithName
		{
			get;
			private set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method is called when a new scene has been set for the layer.
		/// </summary>
		/// <param name="world">The new scene. Can be null.</param>
		protected virtual void OnSceneChanged(IScene scene)
		{
			m_renderableList.Clear();

			if (scene != null)
			{
				foreach (IRenderable renderable in scene.Collect(typeof(IRenderable)))
				{
					if (MatchRenderable(renderable) == true)
					{
						m_renderableList.Add(renderable);
					}
				}
			}
		}

		/// <summary>
		/// This method is called when a movable is added to the scene.
		/// </summary>
		/// <param name="scene">The scene</param>
		/// <param name="movable">The movable.</param>
		protected virtual void OnMovableAdded(IScene scene, IMovable movable)
		{
			var renderable = movable as IRenderable;
			if (renderable != null && MatchRenderable(renderable) == true)
			{
				m_renderableList.Add((IRenderable)movable);
			}
		}

		/// <summary>
		/// This method checks if the given renderable should be handled by the layer.
		/// </summary>
		/// <param name="renderable">The renderable.</param>
		/// <returns>True if the renderable should be handled by the layer.</returns>
		protected virtual bool MatchRenderable(IRenderable renderable)
		{
			var result = this.RenderableTypes.FirstOrDefault(e => e.IsAssignableFrom(renderable.GetType())) != null;

			var namable = renderable as INamable;
			if (result == true && String.IsNullOrEmpty(this.StartsWithName) == false)
			{
				result = namable != null && String.IsNullOrEmpty(namable.Name) == false && namable.Name.StartsWith(this.StartsWithName);
			}

			return result;
		}

		/// <summary>
		/// Manuall adds a renderable to the layer, even if it doesn't match.
		/// </summary>
		/// <param name="renderable">The renderable.</param>
		public void AddRenderable(IRenderable renderable)
		{
			if (m_renderableList.Contains(renderable) == false)
			{
				m_renderableList.Add(renderable);
			}
		}

		/// <summary>
		/// Removes a renderable from the layer.
		/// </summary>
		/// <param name="renderable">The renderable.</param>
		public void RemoveRenderable(IRenderable renderable)
		{
			m_renderableList.Remove(renderable);
		}

		/// <summary>
		/// This methos is called when a movable is removed from the scene.
		/// </summary>
		/// <param name="scene">The scene.</param>
		/// <param name="movable">The movable.</param>
		protected virtual void OnMovableRemoved(IScene scene, IMovable movable)
		{
			if (movable is IRenderable)
			{
				m_renderableList.Remove((IRenderable)movable);
			}
		}

		protected override void DisposeUnmanaged()
		{
			this.Scene = null;
		}

		#endregion

		#region ILayer Members

		public IScene Scene
		{
			get { return m_scene; }
			set
			{
				if (m_scene == null && value != null)
				{
					m_scene = value;
					m_scene.AddLayer(this);

					OnSceneChanged(m_scene);

					m_scene.MovableAdded += OnMovableAdded;
					m_scene.MovableRemoved += OnMovableRemoved;
				}
				else if (m_scene != value && value != null)
				{
					var previousScene = m_scene;
					m_scene = null;

					previousScene.RemoveLayer(this);

					previousScene.MovableAdded -= OnMovableAdded;
					previousScene.MovableRemoved -= OnMovableRemoved;

					m_scene = value;
					m_scene.AddLayer(this);

					m_scene.MovableAdded += OnMovableAdded;
					m_scene.MovableRemoved += OnMovableRemoved;

					OnSceneChanged(m_scene);
				}
				else if (m_scene != null && value == null)
				{
					var previousScene = m_scene;
					m_scene = null;

					previousScene.RemoveLayer(this);

					previousScene.MovableAdded -= OnMovableAdded;
					previousScene.MovableRemoved -= OnMovableRemoved;

					OnSceneChanged(m_scene);
				}
			}
		}

		public float Depth
		{
			get;
			private set;
		}

		public ReadOnlyList<IRenderable> Renderables
		{
			get { return m_readOnlyRenderableList; }
		}

		#endregion
	}
}
