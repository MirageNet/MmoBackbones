namespace LUD.DataStructures
{
    public struct ServerDataInfo
    {
        public string ServerName;
        public byte ServerId;
    }

    public class ServerDataInfoComparer : IEqualityComparer<ServerDataInfo>
    {
        #region Implementation of IEqualityComparer<in ServerDataInfo>

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public bool Equals(ServerDataInfo x, ServerDataInfo y)
        {
            return x.ServerId == y.ServerId;
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(ServerDataInfo obj)
        {
            return obj.ServerId;
        }

        #endregion
    }
}
