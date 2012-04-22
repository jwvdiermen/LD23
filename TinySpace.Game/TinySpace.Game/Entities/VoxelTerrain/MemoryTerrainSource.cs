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
	public class MemoryTerrainSource : ITerrainSource
	{
		#region Constructors

		/// <summary>
		/// Creates a terrain source
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="regionWidth">The size of a region.</param>
		public MemoryTerrainSource(sbyte[,] data, int regionSize)
		{
			int width = data.GetUpperBound(0);
			int height = data.GetUpperBound(1);

			if (width % regionSize != 0)
			{
				throw new Exception("The width of the terrain should be divisable by the size of a region.");
			}
			if (height % regionSize != 0)
			{
				throw new Exception("The height of the terrain should be divisable by the size of a region.");
			}

			// Create the regions.
			this.RegionCountX = width / regionSize;
			this.RegionCountY = height / regionSize;
			m_regions = new MemoryTerrainRegionSource[this.RegionCountX, this.RegionCountY];

			for (int x = 0; x < this.RegionCountX; ++x)
			{
				for (int y = 0; y < this.RegionCountY; ++y)
				{
					m_regions[x, y] = new MemoryTerrainRegionSource(this, regionSize, data, regionSize * x, regionSize * y);
				}
			}
		}

		#endregion

		#region Fields

		private MemoryTerrainRegionSource[,] m_regions;
		private List<MemoryTerrainRegionSource> m_dirtyRegionList = new List<MemoryTerrainRegionSource>();

		#endregion

		#region Methods

		private void NotifyRegionChanged(MemoryTerrainRegionSource region)
		{
			// Track modified regions.
			lock (m_dirtyRegionList)
			{
				m_dirtyRegionList.Add(region);
			}
		}

		#endregion

		#region ITerrainSource Members

		public int RegionCountX
		{
			get;
			private set;
		}

		public int RegionCountY
		{
			get;
			private set;
		}

		public bool HasChanged
		{
			get { return m_dirtyRegionList.Count > 0; }
		}

		public ITerrainRegionSource GetRegionSource(int x, int y)
		{
			if (x < 0 || x >= this.RegionCountX)
			{
				throw new ArgumentOutOfRangeException("x", "Should be larger then zero and smaller then the number of regions on the X axis.");
			}
			if (x < 0 || x >= this.RegionCountY)
			{
				throw new ArgumentOutOfRangeException("y", "Should be larger then zero and smaller then the number of regions on the Y axis.");
			}

			return m_regions[x, y];
		}

		#endregion
	}
}
