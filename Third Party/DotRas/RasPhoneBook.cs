//--------------------------------------------------------------------------
// <copyright file="RasPhoneBook.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      GNU Library General Public License (LGPL) v2.1 which can be found
//      in the License.rtf at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace DotRas
{
    using DotRas.Design;
    using DotRas.Internal;
    using DotRas.Properties;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// Represents a remote access service (RAS) phone book. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When using <b>RasPhoneBook</b> to monitor a phone book for external changes, ensure the SynchronizingObject property is set if thread synchronization is If this is not done, you may get cross-thread exceptions thrown from the component. This is typically needed with applications that have an interface; for example, Windows Forms or Windows Presentation Foundation (WPF).
    /// </para>
    /// <para>
    /// There are multiple phone books in use by Windows at any given point in time and this class can only manage one phone book per instance. If you add an entry to the all user's profile phone book, attempting to manipulate it with the current user's profile phone book opened will result in failure. Entries will not be located, and changes made to the phone book will not be recognized by the instance.
    /// </para>
    /// <para><b>Known Limitations</b>
    /// <list type="bullet">
    /// <item>For phone books which are not located in the all users profile directory (including those in custom locations) any stored credentials for the entries must be stored per user.</item>
    /// <item>The <b>RasPhoneBook</b> component may not be able to modify entries in the All User's profile without elevated application privileges. If your application cannot request elevated privileges you can either use the current user profile phone book, or use a custom phone book in a path that will not require elevated permissions.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to open a phone book in a custom location using a <b>RasPhoneBook</b> class.
    /// <code lang="C#">
    /// <![CDATA[
    /// RasPhoneBook pbk = RasPhoneBook.Open(@"C:\Test.pbk");
    /// ]]>
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim pbk As RasPhoneBook = RasPhoneBook.Open("C:\Test.pbk")
    /// ]]>
    /// </code>
    /// </example>
    public sealed class RasPhoneBook
    {
        #region Fields

        /// <summary>
        /// Defines the partial path (including filename) for a default phonebook file.
        /// </summary>
        private const string PhoneBookFilePath = @"Microsoft\Network\Connections\Pbk\rasphone.pbk";

        /// <summary>
        /// Contains the collection of entries in the phone book.
        /// </summary>
        private ObservableCollection<RasEntry> entries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasPhoneBook"/> class.
        /// </summary>
        public RasPhoneBook()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the full path (including filename) of the phone book.
        /// </summary>
        [Browsable(false)]
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of entries within the phone book.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<RasEntry> Entries
        {
            get
            {
                if (this.entries == null)
                {
                    this.entries = new ObservableCollection<RasEntry>();
                }

                return this.entries;
            }
        }

        #endregion

        /// <summary>
        /// Determines the full path (including filename) of the phone book.
        /// </summary>
        /// <param name="phoneBookType">The type of phone book to locate.</param>
        /// <returns>The full path (including filename) of the phone book.</returns>
        public static string GetPhoneBookPath(RasPhoneBookType phoneBookType)
        {
            Environment.SpecialFolder folder;
            switch (phoneBookType)
            {
                case RasPhoneBookType.User:
                    folder = Environment.SpecialFolder.ApplicationData;
                    break;

                case RasPhoneBookType.AllUsers:
                    folder = Environment.SpecialFolder.CommonApplicationData;
                    break;

                default:
                    throw new NotSupportedException();
            }

            string path = Environment.GetFolderPath(folder);
            if (string.IsNullOrWhiteSpace(path))
            {
                ThrowHelper.ThrowInvalidOperationException(Resources.Exception_PathWasNotReturned);
            }

            return Path.Combine(path, PhoneBookFilePath);
        }

        /// <summary>
        /// Opens the phone book.
        /// </summary>
        /// <param name="fileName">The path (including filename) of a phone book.</param>
        /// <remarks>This method opens an existing phone book or creates a new phone book if the file does not already exist.</remarks>
        /// <exception cref="System.ArgumentException"><paramref name="fileName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public static RasPhoneBook Open(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                ThrowHelper.ThrowArgumentException("fileName", Resources.Argument_StringCannotBeNullOrEmpty);
            }
            else if (string.IsNullOrWhiteSpace(Path.GetFileName(fileName)))
            {
                ThrowHelper.ThrowArgumentException("fileName", Resources.Argument_InvalidFileName);
            }

            RasPhoneBook result = new RasPhoneBook();
            result.Load(fileName);

            return result;
        }

        private void Load(string fileName)
        {
            var entryNames = RasHelper.Default.GetEntryNames(fileName);
            if (entryNames != null)
            {
                foreach (var entryName in entryNames)
                {
                    var entry = RasHelper.Default.GetEntryProperties(entryName.phoneBookPath, entryName.name);
                    if (entry == null)
                    {
                        throw new InvalidOperationException();
                    }

                    this.Entries.Add(entry);
                }
            }

            this.FileName = fileName;
        }
    }
}