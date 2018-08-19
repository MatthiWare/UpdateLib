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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Common
{
    /// <summary>
    /// Versioning class with small extensions over the original <see cref="System.Version"/> as the original is sealed.
    /// Support for version label's and serializable.
    /// Partially based on Semantic Versioning <http://semver.org/>
    /// </summary>
    [DebuggerDisplay("{Value}")]
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
        private static readonly Regex m_regex = new Regex(@"([v]?[0-9]+){1}(\.[0-9]+){0,2}([-](alpha|beta|rc))?");

        #endregion

        #region Properties

        public int Major => m_major;

        public int Minor => m_minor;

        public int Patch => m_patch;

        public VersionLabel Label => m_label;

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

        public override bool Equals(object obj)
            => Equals(obj as UpdateVersion);

        public override int GetHashCode()
        {
            int hash = 269;

            hash = (hash * 47) + Major.GetHashCode();
            hash = (hash * 47) + Minor.GetHashCode();
            hash = (hash * 47) + Patch.GetHashCode();
            hash = (hash * 47) + Label.GetHashCode();

            return hash;
        }

        #endregion

        public override string ToString() => $"{m_major}.{m_minor}.{m_patch}{LabelToString()}";

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

        public static bool CanParse(string input)
            => m_regex.IsMatch(input);

        /// <summary>
        /// Tries to parse the <paramref name="input"/> to a <see cref="UpdateVersion"/>
        /// </summary>
        /// <param name="input">Input string should be of format "(v)major.minor.patch(-label)". The (v) and (-label) are optional</param>
        /// <param name="version">The output parameter</param>
        /// <returns>True if succesfully parsed, false if failed</returns>
        public static bool TryParse(string input, out UpdateVersion version)
        {
            if (input.StartsWith("v"))
                input = input.Substring(1, input.Length - 2);

            var tokens = input.Split(CharSplitDot);
            version = new UpdateVersion();

            if (tokens.Length > 3) // invalid version format, needs to be the following major.minor.patch(-label)
                return false;

            if (tokens.Length > 2)
            {
                var extraTokens = tokens[2].Split(CharSplitDash);

                if (!int.TryParse(extraTokens[0], out version.m_patch))
                    return false;

                if (extraTokens.Length > 1 && !TryParseVersionLabelString(extraTokens[1], out version.m_label)) // unable to parse the version label
                    return false;

                if (extraTokens.Length > 2) return false; // invalid, can only contain 1 version label string
            }

            if (tokens.Length > 1 && !int.TryParse(tokens[1], out version.m_minor))
                return false;

            if (tokens.Length > 0 && !int.TryParse(tokens[0], out version.m_major))
                return false;

            return true; // everything parsed succesfully
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

        public static implicit operator UpdateVersion(string value)
            => new UpdateVersion(value);

        public static implicit operator string(UpdateVersion version)
            => version.Value;
    }
}
