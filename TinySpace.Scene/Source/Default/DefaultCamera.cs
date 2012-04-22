using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using TinySpace.Core;

namespace TinySpace.Scene
{
	/// <summary>
	/// This class implements the <see cref="ICamera" /> interface.
	/// </summary>
	public class DefaultCamera : Movable, ICamera
	{
		#region Constructors

		public DefaultCamera(RenderTarget2D renderTarget, Viewport viewport)
		{
			this.RenderTarget = renderTarget;
			this.Viewport = viewport;

			//Matrix.CreateOrthographicOffCenter(0.0f, 100.0f, 100.0f, 0.0f, 0.0f, 1.0f, out m_projectMatrix);
			var ratio = (float)viewport.Width / (float)viewport.Height;
			this.ScreenSize = new Vector2(ratio * 100.0f, 100.0f);

			var halfWidth = this.ScreenSize.X / 2.0f;
			var halfHeight = this.ScreenSize.Y / 2.0f;

			Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, halfHeight, -halfHeight, -1.0f, 1.0f, out m_projectMatrix);			
		}

		#endregion

		#region Fields

		private Matrix m_projectMatrix;

		#endregion

		#region ICamera Members

		public RenderTarget2D RenderTarget
		{
			get;
			private set;
		}

		public Viewport Viewport
		{
			get;
			private set;
		}

		public Vector2 ScreenSize
		{
			get;
			private set;
		}

		public Matrix Transformation
		{
			get 
			{
				Matrix projectionMatrix, viewMatrix;
				GetMatrices(out projectionMatrix, out viewMatrix);

				return projectionMatrix * viewMatrix;
			}
		}

		public void GetMatrices(out Matrix projectionMatrix, out Matrix viewMatrix)
		{
			var nodeScale = this.SceneNode.Scale;

			projectionMatrix = m_projectMatrix;

			if (this.SceneNode != null)
			{
				viewMatrix =
					Matrix.CreateTranslation(new Vector3(-this.SceneNode.Position, 0.0f)) *
					Matrix.CreateRotationZ(MathHelper.ToRadians(this.SceneNode.Rotation)) *
					Matrix.CreateScale(new Vector3(1.0f / nodeScale.X, 1.0f / nodeScale.Y, 1.0f));
			}
			else
			{
				viewMatrix = Matrix.Identity;
			}
		}

		public void Apply(GraphicsDevice graphics)
		{
			graphics.SetRenderTarget(this.RenderTarget);
			graphics.Viewport = this.Viewport;
		}

		#endregion
	}
}
