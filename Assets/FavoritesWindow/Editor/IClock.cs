namespace Favorites
{
	using UnityEditor;

	public interface IClock
	{
		double GetNowInSeconds();
	}

	public class EditorClock : IClock
	{
		public double GetNowInSeconds()
		{
			return EditorApplication.timeSinceStartup;
		}
	}
}