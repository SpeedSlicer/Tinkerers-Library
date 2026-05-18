using System;
using Unity.Collections;
using Unity.Netcode;

public struct NetworkInventoryItem : INetworkSerializable, IEquatable<NetworkInventoryItem>
{
    public int ItemId;
    public int Quantity;
    public FixedString32Bytes ItemName; 

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ItemId);
        serializer.SerializeValue(ref Quantity);
        serializer.SerializeValue(ref ItemName);
    }

    public bool Equals(NetworkInventoryItem other)
    {
        return ItemId == other.ItemId && 
               Quantity == other.Quantity && 
               ItemName == other.ItemName;
    }

}
