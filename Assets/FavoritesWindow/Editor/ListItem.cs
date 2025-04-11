namespace Favorites
{
	public class ListItem<T>
	{
		public T Value;
		public bool IsSelected;

		public override string ToString()
		{
			return string.Format( "{0}:{1}", IsSelected ? 's' : 'u', Value.ToString() );
		}
	} 
}