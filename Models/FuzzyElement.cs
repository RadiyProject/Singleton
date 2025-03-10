namespace Singleton.Models;

public class FuzzyElement(float element, float membershipDegree)
{
    public float Element { get; private set; } = element;
    public float MembershipDegree { get; private set; } = membershipDegree;

    public void ChangeMembershipDegree(float newDegree)
    {
        MembershipDegree = newDegree;
    }
}
