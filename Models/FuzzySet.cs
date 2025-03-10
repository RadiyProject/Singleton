namespace Singleton.Models;

public class FuzzySet
{
    public FuzzyElement[] Set { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tuples" description="Tuple<Element, MembershipDegree>"></param>
    public FuzzySet(List<Tuple<float, float>> tuples)
    {
        FuzzyElement[] tempSet = new FuzzyElement[tuples.Count];
        int i = 0;
        foreach (Tuple<float, float> tuple in tuples)
        {
            tempSet[i] = new FuzzyElement(tuple.Item1, tuple.Item2);
            i++;
        }

        Set = tempSet;
    }

    public FuzzySet(FuzzySet setToDuplicate)
    {
        FuzzyElement[] tempSet = new FuzzyElement[setToDuplicate.Set.Length];
        for (int i = 0; i < setToDuplicate.Set.Length; i++)
            tempSet[i] = new FuzzyElement(setToDuplicate.Set[i].Element, setToDuplicate.Set[i].MembershipDegree);

        Set = tempSet;
    }

    public static FuzzySet operator * (FuzzySet set1, FuzzySet set2) 
    {
        if (set1.Set.Length != set2.Set.Length)
            throw new InvalidOperationException();

        FuzzySet result = new (set1);
        for (int i = 0; i < set1.Set.Length; i++)
        {
            if (result.Set[i].Element != set2.Set[i].Element)
                throw new InvalidOperationException();

            if (result.Set[i].MembershipDegree > set2.Set[i].MembershipDegree)
                result.Set[i].ChangeMembershipDegree(set2.Set[i].MembershipDegree);
        }

        return result;
    }

    public static FuzzySet operator + (FuzzySet set1, FuzzySet set2) 
    {
        if (set1.Set.Length != set2.Set.Length)
            throw new InvalidOperationException();

        FuzzySet result = new (set1);
        for (int i = 0; i < set1.Set.Length; i++)
        {
            if (result.Set[i].Element != set2.Set[i].Element)
                throw new InvalidOperationException();

            if (result.Set[i].MembershipDegree < set2.Set[i].MembershipDegree)
                result.Set[i].ChangeMembershipDegree(set2.Set[i].MembershipDegree);
        }

        return result;
    }
}
