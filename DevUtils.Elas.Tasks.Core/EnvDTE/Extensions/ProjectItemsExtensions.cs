using System.Collections.Generic;
using EnvDTE;

namespace DevUtils.Elas.Tasks.Core.EnvDTE.Extensions
{
	static class ProjectItemsExtensions
	{
		public static IEnumerable<ProjectItem> GetAllItems(this ProjectItems project)
		{
			foreach (ProjectItem item in project)
			{
				yield return item;
				foreach (var item2 in item.ProjectItems.GetAllItems())
				{
					yield return item2;
				}
			}
		}
	}
}