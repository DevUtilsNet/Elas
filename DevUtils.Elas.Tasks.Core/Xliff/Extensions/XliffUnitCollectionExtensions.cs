using System.Collections.Generic;

namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	/// <summary> An xliff unit collection extensions. </summary>
	static public class XliffUnitCollectionExtensions
	{
		/// <summary> Gets all units in this collection. </summary>
		///
		/// <param name="units"> The units to act on. </param>
		///
		/// <returns> An enumerator that allows foreach to be used to process all units in this collection. </returns>
		public static IEnumerable<XliffUnit> GetAllUnits(this IEnumerable<XliffUnit> units)
		{
			foreach (var item in units)
			{
				yield return item;
				var group = item as XliffGroup;
				if (group != null)
				{
					foreach (var item2 in group.Units.GetAllUnits())
					{
						yield return item2;
					}
				}
			}
		}

		private static bool IsDelimeter(char ch)
		{
			var ret = ch == '.' || ch == '_' || ch == ' ';
			return ret;
		}

		/// <summary> An XliffUnitCollection extension method that ensures that unit collection by
		/// composite identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		///
		/// <returns> An XliffUnitCollection. </returns>
		public static XliffUnitCollection GetOrAddUnitCollectionByCompositeId(this XliffUnitCollection unitCollection, string id)
		{
			var restKey = id;
			while (true)
			{
				var index = 0;
				for (; index < restKey.Length && !IsDelimeter(restKey[index]); ++index)
				{
				}

				if (index < restKey.Length)
				{
					var subKey = restKey.Substring(0, index);
					restKey = restKey.Substring(index + 1);
					unitCollection = unitCollection.GetOrAddGroup(subKey).Units;
					continue;
				}
				break;
			}

			return unitCollection;
		}

		/// <summary> An XliffUnitCollection extension method that gets or add group by composite
		/// identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		///
		/// <returns> The or add group by composite identifier. </returns>
		public static XliffGroup GetOrAddGroupByCompositeId(this XliffUnitCollection unitCollection, string id)
		{
			unitCollection = unitCollection.GetOrAddUnitCollectionByCompositeId(id);

			var ret = unitCollection.GetOrAddGroup(id);
			return ret;
		}

		/// <summary> An XliffUnitCollection extension method that gets or add transaction unit by
		/// composite identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		///
		/// <returns> The or add transaction unit by composite identifier. </returns>
		public static XliffTransUnit GetOrAddTransUnitByCompositeId(this XliffUnitCollection unitCollection, string id)
		{
			unitCollection = unitCollection.GetOrAddUnitCollectionByCompositeId(id);

			var ret = unitCollection.GetOrAddTransUnit(id);
			return ret;
		}

		/// <summary> An XliffUnitCollection extension method that updates the or create transaction unit
		/// by identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		/// <param name="source">				  Source for the. </param>
		///
		/// <returns> An XliffTransUnit. </returns>
		public static XliffTransUnit UpdateOrCreateTransUnitById(this XliffUnitCollection unitCollection, string id, string source)
		{
			bool updateOrCreate;
			var ret = unitCollection.UpdateOrCreateTransUnitById(id, source, out updateOrCreate);
			return ret;
		}

		/// <summary> An XliffUnitCollection extension method that updates the or create transaction unit
		/// by identifier. </summary>
		///
		/// <param name="unitCollection">	  The unitCollection to act on. </param>
		/// <param name="id">							  The identifier. </param>
		/// <param name="source">					  Source for the. </param>
		/// <param name="updatedOrCreated"> [out] The updated or created. </param>
		///
		/// <returns> An XliffTransUnit. </returns>
		public static XliffTransUnit UpdateOrCreateTransUnitById(this XliffUnitCollection unitCollection, string id, string source, out bool updatedOrCreated)
		{
			var unit = unitCollection.GetOrAddTransUnit(id);
			if (unit.Source != null)
			{
				if (unit.Source.Content == source && unit.Target != null)
				{
					unit.Used();
					updatedOrCreated = false;
					return unit;
				}
			}
			else
			{
				unit.Source = unit.Document.CreateSource();
			}

			unit.Used();
			updatedOrCreated = true;

			if (unit.Target == null)
			{
				unit.Target = unit.Document.CreateTarget();
				unit.Target.State = XliffTargetState.New;
			}
			else
			{
				unit.Target.State = XliffTargetState.NeedsTranslation;
			}

			unit.Source.Content = source;

			return unit;
		}

		/// <summary> An XliffUnitCollection extension method that updates the or create transaction unit
		/// by composite identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		/// <param name="source">				  Source for the. </param>
		///
		/// <returns> An XliffTransUnit. </returns>
		public static XliffTransUnit UpdateOrCreateTransUnitByCompositeId(this XliffUnitCollection unitCollection, string id, string source)
		{
			bool updatedOrCreated;
			var ret = unitCollection.UpdateOrCreateTransUnitByCompositeId(id, source, out updatedOrCreated);
			return ret;
		}

		/// <summary> An XliffUnitCollection extension method that updates the or create transaction unit
		/// by composite identifier. </summary>
		///
		/// <param name="unitCollection">	  The unitCollection to act on. </param>
		/// <param name="id">							  The identifier. </param>
		/// <param name="source">					  Source for the. </param>
		/// <param name="updatedOrCreated"> [out] The updated or created. </param>
		///
		/// <returns> An XliffTransUnit. </returns>
		public static XliffTransUnit UpdateOrCreateTransUnitByCompositeId(this XliffUnitCollection unitCollection, string id, string source, out bool updatedOrCreated)
		{
			var ret = unitCollection.GetOrAddUnitCollectionByCompositeId(id).UpdateOrCreateTransUnitById(id, source, out updatedOrCreated);
			return ret;
		}

		/// <summary> An XliffUnitCollection extension method that gets unit collection by composite
		/// identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		///
		/// <returns> The unit collection by composite identifier. </returns>
		public static XliffUnitCollection GetUnitCollectionByCompositeId(this XliffUnitCollection unitCollection, string id)
		{
			var restKey = id;
			while (true)
			{
				var index = 0;
				for (; index < restKey.Length && restKey[index] != '.' && restKey[index] != '_'; ++index)
				{
				}

				if (index < restKey.Length)
				{
					var subKey = restKey.Substring(0, index);
					restKey = restKey.Substring(index + 1);
					var group = unitCollection.GetGroup(subKey);
					if (group != null)
					{
						unitCollection = group.Units;
						continue;
					}
					return null;
				}

				return unitCollection;
			}
		}

		/// <summary> An XliffUnitCollection extension method that gets group by composite identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		///
		/// <returns> The group by composite identifier. </returns>
		public static XliffGroup GetGroupByCompositeId(this XliffUnitCollection unitCollection, string id)
		{
			unitCollection = unitCollection.GetUnitCollectionByCompositeId(id);
			if (unitCollection == null)
			{
				return null;
			}
			var ret = unitCollection.GetGroup(id);
			return ret;
		}

		/// <summary> Gets transaction unit by composite identifier. </summary>
		///
		/// <param name="unitCollection"> The unitCollection to act on. </param>
		/// <param name="id">						  The identifier. </param>
		///
		/// <returns> The transaction unit by composite identifier. </returns>
		public static XliffTransUnit GetTransUnitByCompositeId(this XliffUnitCollection unitCollection, string id)
		{
			unitCollection = unitCollection.GetUnitCollectionByCompositeId(id);
			if (unitCollection == null)
			{
				return null;
			}
			var ret = unitCollection.GetTransUnit(id);
			return ret;
		}
	}
}