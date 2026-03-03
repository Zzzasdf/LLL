using System.Collections.Generic;

public class ViewUniqueIdGenerator
{
    private static ViewUniqueIdGenerator _default = new ViewUniqueIdGenerator();
    public static ViewUniqueIdGenerator Default => _default;
    
    private Dictionary<int, ViewType> uniqueId2ViewTypes;
    private int incrementId;
    private ViewUniqueIdGenerator()
    {
        uniqueId2ViewTypes = new Dictionary<int, ViewType>();
    }
    public int Create(ViewType viewType)
    {
        do
        {
            incrementId++;
        } while (uniqueId2ViewTypes.ContainsKey(incrementId));
        uniqueId2ViewTypes.Add(incrementId, viewType);
        return incrementId;
    }
    public bool Delete(int uniqueId)
    {
        if (!uniqueId2ViewTypes.Remove(uniqueId))
        {
            LLogger.FrameError($"删除无效的 uniqueId: {uniqueId2ViewTypes}");
            return false;
        }
        return true;
    }

    public ViewType GetViewType(int uniqueId) => uniqueId2ViewTypes[uniqueId];
}
