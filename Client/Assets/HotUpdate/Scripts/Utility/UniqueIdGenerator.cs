using System.Collections.Generic;

public class UniqueIdGenerator
{
    private static UniqueIdGenerator _default = new UniqueIdGenerator();
    public static UniqueIdGenerator Default => _default;
    
    private HashSet<int> uniqueIds;
    private int incrementId;
    private UniqueIdGenerator()
    {
        uniqueIds = new HashSet<int>();
    }
    public int Create()
    {
        do
        {
            incrementId++;
        } while (uniqueIds.Contains(incrementId));
        uniqueIds.Add(incrementId);
        return incrementId;
    }
    public bool Delete(int uniqueId)
    {
        if (!uniqueIds.Remove(uniqueId))
        {
            LLogger.FrameError($"删除无效的 uniqueId: {uniqueIds}");
            return false;
        }
        return true;
    }
}
