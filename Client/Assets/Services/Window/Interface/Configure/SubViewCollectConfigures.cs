using System.Collections;
using System.Collections.Generic;

public class SubViewCollectConfigures: ISubViewCollectConfigures
{
    private Dictionary<SubViewCollect, ISubViewDisplayConfigure> subViewDisplayConfigures;
    
    public SubViewCollectConfigures(Dictionary<SubViewCollect, ISubViewDisplayConfigure> subViewDisplayConfigures)
    {
        this.subViewDisplayConfigures = subViewDisplayConfigures;
    }

    IEnumerator<KeyValuePair<SubViewCollect, ISubViewDisplayConfigure>> IEnumerable<KeyValuePair<SubViewCollect, ISubViewDisplayConfigure>>.GetEnumerator()
    {
        return subViewDisplayConfigures.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return subViewDisplayConfigures.GetEnumerator();
    }
}
