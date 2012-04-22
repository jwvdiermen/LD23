using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// Implements the <see cref="ITerrainSource" /> interface and provides the source
	/// directly from memory.
	/// </summary>
	public class MemoryTerrainRegionSource : ITerrainRegionSource
	{
		#region Constructors

		internal MemoryTerrainRegionSource(MemoryTerrainSource terrainSource, int size, sbyte[,] data, int offsetX, int offsetY)
		{
			m_terrainSource = terrainSource;

			this.Size = size;

			m_offsetX = offsetX;
			m_offsetY = offsetY;
			m_data = data;

			this.HasChanged = false;
		}

		#endregion

		#region Fields

		MemoryTerrainSource m_terrainSource;

		private sbyte[,] m_data;
		private int m_offsetX;
		private int m_offsetY;

		#endregion

		#region ITerrainRegionSource Members

		public int Size
		{
			get;
			private set;
		}

		public bool HasChanged
		{
			get;
			private set;
		}

		public int OffsetX 
		{
			get { return m_offsetX; }
		}

		public int OffsetY 
		{
			get { return m_offsetY; } 
		}

		public sbyte[,] Data 
		{
			get { return m_data; }
		}

		//public sbyte GetPoint(int x, int y)
		//{
		//    if (x < 0 || x >= this.Size)
		//    {
		//        throw new ArgumentOutOfRangeException("x", "Should be larger then zero and smaller then the size of the region.");
		//    }
		//    if (y < 0 || y >= this.Size)
		//    {
		//        throw new ArgumentOutOfRangeException("y", "Should be larger then zero and smaller then the size of the region.");
		//    }

		//    return m_data[m_offsetX + x, m_offsetY + y];
		//}

		public void SetPoint(int x, int y, sbyte value)
		{
			if (x < 0 || x >= this.Size)
			{
				throw new ArgumentOutOfRangeException("x", "Should be larger then zero and smaller then the size of the region.");
			}
			if (y < 0 || y >= this.Size)
			{
				throw new ArgumentOutOfRangeException("y", "Should be larger then zero and smaller then the size of the region.");
			}

			m_data[m_offsetX + x, m_offsetY + y] = value;
			this.HasChanged = true;
		}

		#endregion
	}
}
