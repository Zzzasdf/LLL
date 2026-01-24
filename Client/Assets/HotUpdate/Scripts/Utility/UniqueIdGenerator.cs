using System.Collections.Generic;

public class UniqueIdGenerator
{
    private HashSet<int> uniqueIds;
    private int incrementId;

    public UniqueIdGenerator()
    {
        uniqueIds = new HashSet<int>();
    }
    
    public int CreateUniqueId()
    {
        do
        {
            incrementId++;
        } while (uniqueIds.Contains(incrementId));
        uniqueIds.Add(incrementId);
        return incrementId;
    }

    public bool DeleteUniqueId(int uniqueId)
    {
        if (!uniqueIds.Remove(uniqueId))
        {
            LLogger.FrameError($"删除无效的 uniqueId: {uniqueIds}");
            return false;
        }
        return true;
    }
}
