using System;
using UnityEngine;

namespace Client.Libs.UI.Types
{
    /// <summary>
    /// Tag for locking screens
    /// </summary>
    [Serializable]
    public struct ScreenLockTag : IComparable<ScreenLockTag>, IEquatable<ScreenLockTag>
    {
        [SerializeField] private string _id;

        public ScreenLockTag(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(id));

            _id = id;
        }

        public int CompareTo(ScreenLockTag other) => string.Compare(_id, other._id, StringComparison.Ordinal);

        public bool Equals(string id) => _id == id;

        public bool Equals(ScreenLockTag other) => _id == other._id;

        public override bool Equals(object obj) => obj is ScreenLockTag other && Equals(other);

        public override int GetHashCode() => _id != null ? _id.GetHashCode() : 0;

        public static bool operator ==(ScreenLockTag a, ScreenLockTag b) => a._id == b._id;

        public static bool operator !=(ScreenLockTag a, ScreenLockTag b) => !(a == b);

        public override string ToString() => _id;
    }
}