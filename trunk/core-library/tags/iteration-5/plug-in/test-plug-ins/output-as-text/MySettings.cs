namespace Landis.Output.Test
{
	public class MySettings
		: Landis.Output.Settings
	{
		private int timestep;

		//---------------------------------------------------------------------

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

		public MySettings(int timestep)
			: base()
		{
			TimeStep = timestep;
		}
	}
}
