/*
 *  TinySpace by Crealuz - TinySpace.Core
 * 
 *  @original authors: jwvdiermen
 */

using System;

namespace TinySpace.Core
{
    /// <summary>
    /// An object that notifies when it is disposed.
    /// </summary>
    public abstract class DisposableObject : IDisposableObject
	{
		#region Events

		/// <summary>
		/// This event occurs when the object is disposed.
		/// </summary>
		public event EventHandler Disposed;

		private void OnDisposed()
		{
			if (this.Disposed != null)
			{
				this.Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Properties

		/// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed 
		{ 
			get; 
			private set; 
		}

		#endregion

		#region Methods

		/// <summary>
        /// This method performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
			Dispose(true);
			GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
				if (this.IsDisposed == false)
				{
					this.IsDisposed = true;

					if (disposing == true)
					{
						try
						{
							DisposeManaged();
						}
						catch (Exception ex)
						{
							//Log.Exception(ex);
						}
					}

					try
					{
						DisposeUnmanaged();
					}
					catch (Exception ex)
					{
						//Log.Exception(ex);
					}

					try
					{
						OnDisposed();
					}
					catch (Exception ex)
					{
						//Log.Exception(ex);
					}						
				}
            }
        }

		/// <summary>
		/// This method should released managed objects.
		/// </summary>
		protected virtual void DisposeManaged()
		{
		}

		/// <summary>
		/// This method should released unmanaged objects.
		/// </summary>
		protected virtual void DisposeUnmanaged()
		{
		}

		/// <summary>
		/// This method checks if the current object is disposed and throws an exception if it is.
		/// </summary>
		/// <exception cref="ObjectDisposedException" />
		protected void CheckDisposed()
		{
			if (this.IsDisposed == true)
			{
				throw new ObjectDisposedException(this.GetType().Name);
			}
		}

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
		}

		#endregion
	}
}