namespace Application.Models.Navigation
{
	public class NavigationNode(INavigationNode parent) : INavigationNode
	{
		#region Properties

		public virtual bool Active { get; set; }
		IEnumerable<INavigationNode> INavigationNode.Children => this.Children;
		public virtual IList<NavigationNode> Children { get; } = new List<NavigationNode>();
		public virtual INavigationNode Parent { get; } = parent;
		public virtual string Text { get; set; }
		public virtual string Tooltip { get; set; }
		public virtual Uri Url { get; set; }

		#endregion
	}
}