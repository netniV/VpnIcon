using System;
using System.Collections.Generic;
using System.Text;

namespace VpnIcon
{
    /// <summary>
    /// Program version history class
    /// </summary>
    public class ProgramVersion
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public Version Version { get; set; }
        /// <summary>
        /// Gets or sets the updates.
        /// </summary>
        /// <value>
        /// The updates.
        /// </value>
        public List<string> Updates { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramVersion"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="updates">The updates.</param>
        public ProgramVersion(string version, params string[] updates)
        {
            Version = new Version(version);
            Updates = new List<string>(updates);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Version " + Version.ToString());
            foreach (string update in Updates)
                sb.AppendFormat("{0}  = {1}", Environment.NewLine, update);
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    /// <summary>
    /// A collection of <see cref="ProgramVersion"/>
    /// </summary>
    public class ProgramVersions : List<ProgramVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramVersions"/> class.
        /// </summary>
        public ProgramVersions()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramVersions"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ProgramVersions(int capacity)
            : base(capacity)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramVersions"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ProgramVersions(IEnumerable<ProgramVersion> collection)
            : base(collection)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramVersions"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ProgramVersions(params ProgramVersion[] args)
            : base()
        {
            this.AddRange(args);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string newLine = null;
            foreach (var ver in this)
            {
                sb.Append($"{newLine}{ver}");
                newLine = Environment.NewLine;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
