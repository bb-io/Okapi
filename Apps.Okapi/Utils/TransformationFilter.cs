using Blackbird.Filters.Enums;
using Blackbird.Filters.Transformations;
using System.Linq;

namespace Apps.Okapi.Utils;

public class TransformationFilter
{
    public static void FilterContent(Transformation initialTransformation, bool excludeEmpty, IEnumerable<SegmentState> statesToKeep)
    {
        initialTransformation.Children.RemoveAll(u =>
        {
            if (u is Unit unit)
            {
                FilterSegments(unit, excludeEmpty, statesToKeep);
                return !unit.Segments.Any();
            }
            else if (u is Group group)
            {
                FilterGroupChildren(group, excludeEmpty, statesToKeep);
                return !group.Children.Any();
            }
            else if (u is Transformation transformation)
            {
                FilterContent(transformation, excludeEmpty, statesToKeep);
                return !transformation.Children.Any();
            }
            return true;
        });
    }

    private static void FilterGroupChildren(Group group, bool excludeEmpty, IEnumerable<SegmentState> statesToKeep)
    {
        group.Children.RemoveAll(c =>
        {
            if (c is Unit unit)
            {
                FilterSegments(unit, excludeEmpty, statesToKeep);
                return !unit.Segments.Any();
            }
            else if (c is Group subGroup)
            {
                FilterGroupChildren(subGroup, excludeEmpty, statesToKeep);
                return !subGroup.Children.Any();
            }
            return true;
        });
    }

    private static void FilterSegments(Unit unit, bool excludeEmpty, IEnumerable<SegmentState> statesToKeep)
    {
        if (statesToKeep.Any())
            unit.Segments.RemoveAll(s => !statesToKeep.Contains(s.State ?? SegmentState.Initial));

        if (excludeEmpty)
            unit.Segments.RemoveAll(s => string.IsNullOrEmpty(s.GetSource()) || string.IsNullOrEmpty(s.GetTarget()));
    }
}
