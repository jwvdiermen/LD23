/*
 *  ISceneNode.cs
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
	/// <summary>
	/// This interface represents a node inside an <see cref="IScene" />. It contains an optional name and has a parent node for recursive purposes.
	/// It also has transformation properties and the ability to attach movable objects using the <see cref="IMovable" /> interface.
	/// </summary>
	public interface ISceneNode : IDisposableObject
	{
		#region Properties

		/// <summary>
		/// Gets the optional name of the scene node.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets the scene in which the scene node is placed.
		/// </summary>
		IScene Scene
		{
			get;
		}

		/// <summary>
		/// Gets or sets the parent of the scene node.
		/// </summary>
		ISceneNode Parent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the global position of the scene node.
		/// </summary>
		Vector2 Position
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the global depth of the scene node.
		/// </summary>
		float Depth
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the global rotation of the scene node.
		/// </summary>
		float Rotation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the global scale of the scene node.
		/// </summary>
		Vector2 Scale
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the local position of the scene node relative from the parent.
		/// </summary>
		Vector2 LocalPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the local depth of the scene node.
		/// </summary>
		float LocalDepth
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the local rotation of the scene node relative from the parent.
		/// </summary>
		float LocalRotation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the local scale of the scene node.
		/// </summary>
		Vector2 LocalScale
		{
			get;
			set;
		}

		/// <summary>
		/// Gets an enumerable containing the childs of the scene node.
		/// </summary>
		IEnumerable<ISceneNode> Childs
		{
			get;
		}
		
		/// <summary>
		/// Gets an enumeration containing the movables attached to the scene node.
		/// </summary>
		IEnumerable<IMovable> Movables
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method creates a new child.
		/// </summary>
		/// <param name="name">The optional name of the child.</param>
		/// <returns>The created child.</returns>
		ISceneNode CreateChild(string name = null);

		/// <summary>
		/// This method adds the given scene node as a child.
		/// </summary>
		/// <param name="child">The child to add.</param>
		/// <remarks>If the scene node is already a child of another scene node, it is first removed.</remarks>
		void AddChild(ISceneNode child);

		/// <summary>
		/// This method removes the given scene node as a child.
		/// </summary>
		/// <param name="child">The child to remove.</param>
		void RemoveChild(ISceneNode child);

		/// <summary>
		/// This method removes the node from its parent.
		/// </summary>
		void Remove();		

		/// <summary>
		/// This method attaches the given movable to the scene node.
		/// </summary>
		/// <param name="movable">The movable to attach.</param>
		/// <remarks>If the given movable is already attached to another scene node, it is first detached.</remarks>
		void Attach(IMovable movable);

		/// <summary>
		/// This method detaches the given movable from the scene node.
		/// </summary>
		/// <param name="movable">The movable to detach.</param>
		void Detach(IMovable movable);

		/// <summary>
		/// This method finds the first movable with the given type.
		/// </summary>
		/// <typeparam name="T">The type of the movable to find.</typeparam>
		/// <returns>The movable if found.</returns>
		T FindMovable<T>()
			where T : IMovable;

		/// <summary>
		/// This method should be called if the parent has changed its transformation, so that this scene node can
		/// mirror that transformation using its local transformation.
		/// </summary>
		void UpdateTransformation();

		#endregion
	}
}
