using Unity.Netcode;
using Unity.Collections;


public struct NetworkString : INetworkSerializable
{

    // Non-static attributes.
    private FixedString32Bytes data;


    // Non-static methods.
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {

        serializer.SerializeValue(ref data);

    }

    public override string ToString()
    {

        return data.ToString();

    }

    // Dos cosas necesarias según la documentación de Unity para poder crear este tipo bien.
    public static implicit operator string(NetworkString s) => s.ToString();

    public static implicit operator NetworkString(string s) => new NetworkString()
    {

        data = new FixedString32Bytes(s)

    };

}