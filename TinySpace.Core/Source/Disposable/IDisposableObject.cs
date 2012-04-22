/*
 *  TinySpace by Crealuz - TinySpace.Core
 * 
 *  @original authors: jwvdiermen
 */

using System;

namespace TinySpace.Core
{
    /// <summary>
    /// An object that can report whether or not it is disposed.
    /// </summary>
    public interface IDisposableObject : IDisposable
	{
		#region Events

		/// <summary>
		/// This event occurs when the object is disposed.
		/// </summary>
		event EventHandler Disposed;

		#endregion

		#region Properties

		/// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        bool IsDisposed 
		{ 
			get;
		}

		#endregion
	}
}