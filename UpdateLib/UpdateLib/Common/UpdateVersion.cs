/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Common
{
    /// <summary>
    /// Versioning class with small extensions over the original <see cref="System.Version"/>.
    /// Support for version label's and serializable.
    /// Partially based on Semantic Versioning <http://semver.org/>
    /// </summary>
    [Serializable]
    public class UpdateVersion : IComparable, IComparable<UpdateVersion>, IEquatable<UpdateVersion>
    {
        private int m_major, m_minor, m_patch;
        private VersionLabel m_label;

        #region Constants

        private const string ALPHA_STRING = "-alpha";
        private const string BETA_STRING = "-beta";
        private const string RC_STRING = "-rc";
        private static readonly char[] CharSplitDot = new char[] { '.' };
        private static readonly char[] CharSplitDash = new char[] { '-' };

        #endregion

        #region Properties

        [XmlIgnore]
        public int Major
        {
            get { return m_major; }
        }

        [XmlIgnore]
        public int Minor
        {
            get { return m_minor; }
        }

        [XmlIgnore]
        public int Patch
        {
            get { return m_patch; }
        }

        [XmlIgnore]
        public VersionLabel Label
        {
            get { return m_label; }
        }

        [XmlText]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string Value
        {
            get { return ToString(); }
            set
            {
                UpdateVersion version;

                if (!TryParse(value, out version))
                    throw new ArgumentException(nameof(value), "Unable to parse input string");

                m_major = version.m_major;
                m_minor = version.m_minor;
                m_patch = version.m_patch;
                m_label = version.m_label;
            }
        }

        #endregion

        #region Constructor

        public UpdateVersion()
            : this(0, 0, 0, VersionLabel.None)
        { }

        public UpdateVersion(int major)
            : this(major, 0, 0, VersionLabel.None)
        { }

        public UpdateVersion(int major, int minor)
            : this(major, minor, 0, VersionLabel.None)
        { }

        public UpdateVersion(int major, int minor, int patch)
            : this(major, minor, patch, VersionLabel.None)
        { }

        public UpdateVersion(int major, int minor, int patch, VersionLabel label)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), "Version cannot be negative");
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), "Version cannot be negative");
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), "Version cannot be negative");

            m_major = major;
            m_minor = minor;
            m_patch = patch;
            m_label = label;
        }

        public UpdateVersion(string input)
        {
            UpdateVersion version;

            if (!TryParse(input, out version))
                throw new ArgumentException(nameof(input), "Unable to parse input string");

            m_major = version.m_major;
            m_minor = version.m_minor;
            m_patch = version.m_patch;
            m_label = version.m_label;
        }

        #endregion

        #region Interface Impl.

        public int CompareTo(UpdateVersion other)
        {
            if (other == null)
                return 1;

            if (m_major != other.m_major)
                return m_major > other.m_major ? 1 : -1;

            if (m_minor != other.m_minor)
                return m_minor > other.m_minor ? 1 : -1;

            if (m_patch != other.m_patch)
                return m_patch > other.m_patch ? 1 : -1;

            if (m_label != other.m_label)
                return m_label > other.m_label ? 1 : -1;

            return 0;
        }

        public int CompareTo(object obj)
        {
            UpdateVersion other = obj as UpdateVersion;

            if (other == null)
                return 1;

            return CompareTo(other);
        }

        public bool Equals(UpdateVersion other)
        {
            if (other == null)
                return false;

            return m_major == other.m_major
                && m_minor == other.m_minor
                && m_patch == other.m_patch
                && m_label == other.m_label;
        }

        #endregion

        public override string ToString()
        {
            return $"{m_major}.{m_minor}.{m_patch}{LabelToString()}";
        }

        private string LabelToString()
        {
            switch (m_label)
            {
                case VersionLabel.Alpha:
                    return ALPHA_STRING;
                case VersionLabel.Beta:
                    return BETA_STRING;
                case VersionLabel.RC:
                    return RC_STRING;
                case VersionLabel.None:
                default:
                    return string.Empty;
            }
        }

        private static bool TryParseVersionLabelString(string input, out VersionLabel label)
        {
            input = $"-{input}";

            if (input == ALPHA_STRING)
            {
                label = VersionLabel.Alpha;
                return true;
            }
            else if (input == BETA_STRING)
            {
                label = VersionLabel.Beta;
                return true;
            }
            else if (input == RC_STRING)
            {
                label = VersionLabel.RC;
                return true;
            }
            else
            {
                label = VersionLabel.None;
                return false;
            }
        }

        public static bool TryParse(string input, out UpdateVersion version)
        {
            var tokens = input.Split(CharSplitDot);
            version = new UpdateVersion();

            if (tokens.Length != 3)
                return false;

            if (tokens.Length > 2)
            {
                var extraTokens = tokens[2].Split(CharSplitDash);

                if (!int.TryParse(extraTokens[0], out version.m_patch))
                    return false;

                if (extraTokens.Length > 1)
                    if (!TryParseVersionLabelString(extraTokens[1], out version.m_label))
                        return false;
            }

            if (tokens.Length > 1)
                if (!int.TryParse(tokens[1], out version.m_minor))
                    return false;

            if (tokens.Length > 0)
                if (!int.TryParse(tokens[0], out version.m_major))
                    return false;

            return true;
        }

        public static bool operator ==(UpdateVersion v1, UpdateVersion v2)
            => ReferenceEquals(v1, null) ? ReferenceEquals(v2, null) : v1.Equals(v2);

        public static bool operator !=(UpdateVersion v1, UpdateVersion v2)
            => !(v1 == v2);

        public static bool operator >(UpdateVersion v1, UpdateVersion v2)
            => v2 < v1;

        public static bool operator >=(UpdateVersion v1, UpdateVersion v2)
            => v2 <= v1;

        public static bool operator <(UpdateVersion v1, UpdateVersion v2)
            => !ReferenceEquals(v1, null) && v1.CompareTo(v2) < 0;

        public static bool operator <=(UpdateVersion v1, UpdateVersion v2)
            => !ReferenceEquals(v1, null) && v1.CompareTo(v2) <= 0;
    }
}
