namespace Landis.Succession
{
	/// <summary>
	/// Settings for a succession component.
	/// </summary>
	public class Settings
	{
		private int timestep;

		//---------------------------------------------------------------------

		/// <summary>
		/// The component's timestep.
		/// </summary>
		public int TimeStep
		{
			get {
				return timestep;
			}
			set {
				if (value <= 0)
					throw new System.ArgumentException("timestep must be > 0");
				timestep = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Create a set of settings.
		/// </summary>
		/// <param name="timestep">The component's timestep</param>
		public Settings(int timestep)
		{
			TimeStep = timestep;
		}
	}
}
