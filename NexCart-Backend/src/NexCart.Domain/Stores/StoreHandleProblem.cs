namespace NexCart.Domain.Stores;

/// <summary>Why a string cannot be used as a Store Handle, without looking at other Stores.</summary>
public enum StoreHandleProblem
{
    Invalid,
    Reserved
}
