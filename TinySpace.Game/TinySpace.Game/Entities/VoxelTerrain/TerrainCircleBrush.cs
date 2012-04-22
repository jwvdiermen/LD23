using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// A circular terrain brush.
	/// </summary>
	public class TerrainCircleBrush : ITerrainBrush
	{
		#region Constructors

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="radius">The circel radius.</param>
		public TerrainCircleBrush(float radius)
		{
			m_boundingSphere = new BoundingSphere(Vector3.Zero, radius);
		}

		#endregion

		#region Fields

		private BoundingSphere m_boundingSphere;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the circle radius.
		/// </summary>
		public float Radius
		{
			get { return m_boundingSphere.Radius; }
		}

		#endregion

		#region ITerrainBrush Members

		public Vector2 Position
		{
			get { return new Vector2(m_boundingSphere.Center.X, m_boundingSphere.Center.Y); }
			set { m_boundingSphere.Center.X = value.X; m_boundingSphere.Center.Y = value.Y; }
		}

		public ContainmentType Contains(ref BoundingBox boundingBox)
		{
			ContainmentType result;
			m_boundingSphere.Contains(ref boundingBox, out result);

			return result;
		}

		#endregion
	}
}
