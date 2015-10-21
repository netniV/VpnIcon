using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace VpnIcon.Handler
{
    /// <summary>
    /// A static helper class for runtime-based functions
    /// </summary>
    public static class RuntimeHelper
    {
        /// <summary>
        /// Specifies the type of assembly to find.
        /// </summary>
        public enum FindAssembly
        {
            /// <summary>
            /// The calling assembly (ie, the assembly that called the function)
            /// </summary>
            Calling,
            /// <summary>
            /// The entry assembly, (ie, the first assembly on the appdomain)
            /// </summary>
            Entry
        }

        /// <summary>
        /// Finds the type.
        /// </summary>
        /// <param name="TypeName">Name of the type.</param>
        /// <returns></returns>
        public static Type FindType(string TypeName)
        {
            Assembly ass = Assembly.GetCallingAssembly();
            return FindType(TypeName, ass);
        }

        /// <summary>
        /// Finds the type.
        /// </summary>
        /// <param name="TypeName">Name of the type.</param>
        /// <param name="findAssembly">The find assembly.</param>
        /// <returns></returns>
        public static Type FindType(string TypeName, FindAssembly findAssembly)
        {
            Assembly ass = (findAssembly == FindAssembly.Calling ? Assembly.GetCallingAssembly() : Assembly.GetEntryAssembly());
            return FindType(TypeName, ass);
        }

        /// <summary>
        /// Finds the type.
        /// </summary>
        /// <param name="TypeName">Name of the type.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static Type FindType(string TypeName, Assembly assembly)
        {
            Type findType = null;

            if (TypeName != null)
                if (TypeName.Contains("."))
                    findType = Type.GetType(TypeName);
                else
                    findType = (from lt in assembly.GetTypes()
                                where lt.Name == TypeName
                                select lt).FirstOrDefault();
            return findType;
        }

        /// <summary>
        /// Gets the version of the executing assembly
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        /// <summary>
        /// Gets the name of the entry assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public static string AssemblyName
        {
            get
            {
                return Path.GetFileName(AssemblyEntry.Location);
            }
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        /// <value>
        /// The assembly path.
        /// </value>
        public static string AssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(AssemblyEntry.Location);
            }
        }

        /// <summary>
        /// Gets the assembly location.
        /// </summary>
        /// <value>
        /// The assembly location.
        /// </value>
        public static string AssemblyLocation
        {
            get
            {
                return AssemblyEntry.Location;
            }
        }

        private static string mAssemblyTitle;
        private static object mAssemblyTitleLock = new object();
        /// <summary>
        /// Gets (and caches) the assembly emtry title.
        /// </summary>
        /// <value>
        /// The assembly title.
        /// </value>
        public static string AssemblyTitle
        {
            get
            {
                if (mAssemblyTitle == null)
                    lock (mAssemblyTitleLock)
                        if (mAssemblyTitle == null)
                        {
                            object[] attributes = AssemblyEntry.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                            mAssemblyTitle = System.IO.Path.GetFileNameWithoutExtension(AssemblyEntry.CodeBase);
                            if (attributes.Length > 0)
                            {
                                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                                if (!string.IsNullOrWhiteSpace(titleAttribute.Title))
                                    mAssemblyTitle = titleAttribute.Title;
                            }
                        }
                return mAssemblyTitle;
            }
        }

        private static string mAssemblyDescription;
        private static object mAssemblyDescriptionLock = new object();
        /// <summary>
        /// Gets (and caches) the assembly description.
        /// </summary>
        /// <value>
        /// The assembly description.
        /// </value>
        public static string AssemblyDescription
        {
            get
            {
                if (mAssemblyDescription == null)
                    lock (mAssemblyDescriptionLock)
                        if (mAssemblyDescription == null)
                        {
                            object[] attributes = AssemblyEntry.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                            mAssemblyDescription = (attributes.Length == 0) ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
                        }
                return mAssemblyDescription;
            }
        }

        private static string mAssemblyProduct;
        private static object mAssemblyProductLock = new object();
        /// <summary>
        /// Gets (and caches) the assembly product.
        /// </summary>
        /// <value>
        /// The assembly product.
        /// </value>
        public static string AssemblyProduct
        {
            get
            {
                if (mAssemblyProduct == null)
                    lock (mAssemblyProductLock)
                        if (mAssemblyProduct == null)
                        {
                            object[] attributes = AssemblyEntry.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                            mAssemblyProduct = (attributes.Length == 0) ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
                        }
                return mAssemblyProduct;
            }
        }

        /// <summary>
        /// Gets (and caches) the assembly copyright.
        /// </summary>
        /// <value>
        /// The assembly copyright.
        /// </value>
        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = AssemblyEntry.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private static string mAssemblyCompany;
        private static object mAssemblyCompanyLock = new object();
        /// <summary>
        /// Gets (and caches) the assembly company.
        /// </summary>
        /// <value>
        /// The assembly company.
        /// </value>
        public static string AssemblyCompany
        {
            get
            {
                if (mAssemblyCompany == null)
                    lock (mAssemblyCompanyLock)
                        if (mAssemblyCompany == null)
                        {
                            object[] attributes = AssemblyEntry.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                            mAssemblyCompany = (attributes.Length == 0) ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
                        }

                return mAssemblyCompany;
            }
        }

        private static string mAssemblyVersion;
        private static object mAssemblyVersionLock = new object();
        /// <summary>
        /// Gets (and caches) the assembly version.
        /// </summary>
        /// <value>
        /// The assembly version.
        /// </value>
        public static string AssemblyVersion
        {
            get
            {
                if (mAssemblyVersion == null)
                    lock (mAssemblyVersionLock)
                        if (mAssemblyVersion == null)
                            mAssemblyVersion = AssemblyEntry.GetName().Version.ToString();

                return mAssemblyVersion;
            }
        }

        private static Assembly mAssemblyEntry;
        private static object mAssemblyEntryLock = new object();

        /// <summary>
        /// Gets (and caches) the entry assembly.
        /// </summary>
        /// <value>
        /// The entry assembly.
        /// </value>
        public static Assembly AssemblyEntry
        {
            get
            {
                if (mAssemblyEntry == null)
                    lock (mAssemblyEntryLock)
                        if (mAssemblyEntry == null)
                            mAssemblyEntry = Assembly.GetEntryAssembly();


                return mAssemblyEntry;
            }
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">The object.</param>
        /// <returns></returns>
        public static string Serialize<T>(T @object)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            try
            {
                xs.Serialize(ms, @object);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    ms = null;
                    return sr.ReadToEnd();
                }

            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
            }
        }

        /// <summary>
        /// Deserializes the specified string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="throwErrors">if set to <c>true</c> [throw errors].</param>
        /// <param name="xmlDoc">The XML document.</param>
        /// <returns></returns>
        public static T Deserialize<T>(bool throwErrors, String xmlDoc)
        {
            return Deserialize<T>(throwErrors, xmlDoc, null, null, null, null);
        }

        /// <summary>
        /// Deserializes the specified string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="throwErrors">if set to <c>true</c> [throw errors].</param>
        /// <param name="xmlDoc">The XML document.</param>
        /// <param name="unknownAttribute">The unknown attribute.</param>
        /// <param name="unknownNode">The unknown node.</param>
        /// <param name="unknownElement">The unknown element.</param>
        /// <param name="unreferencedObject">The unreferenced object.</param>
        /// <returns></returns>
        public static T Deserialize<T>(bool throwErrors, String xmlDoc, XmlAttributeEventHandler unknownAttribute, XmlNodeEventHandler unknownNode, XmlElementEventHandler unknownElement, UnreferencedObjectEventHandler unreferencedObject)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            if (throwErrors)
            {
                if (unknownAttribute != null) xs.UnknownAttribute += new XmlAttributeEventHandler(unknownAttribute);
                if (unknownNode != null) xs.UnknownNode += new XmlNodeEventHandler(unknownNode);
                if (unknownElement != null) xs.UnknownElement += new XmlElementEventHandler(unknownElement);
                if (unreferencedObject != null) xs.UnreferencedObject += new UnreferencedObjectEventHandler(unreferencedObject);
            }

            try
            {
                using (StringReader sr = new StringReader(xmlDoc))
                    return (T)xs.Deserialize(sr);
            }
            catch (Exception ex)
            {
                File.AppendAllText(string.Format("Message {0:yyyyMMdd-HHmmss}.Log", DateTime.Now), string.Format("Failed to convert {0}:\r\nData:\r\n{1}\r\n\r\nException: {2}", typeof(T).Name, xmlDoc, RuntimeHelper.ConvertExceptionToString(ex)));
                if (throwErrors)
                    throw;

                return default(T);
            }
        }


        /// <summary>
        /// Converts the bytes to string.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string ConvertBytesToString(byte[] message)
        {
            Exception ex = null;
            return ConvertBytesToString(message, out ex);
        }

        /// <summary>
        /// Converts the bytes to string.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static string ConvertBytesToString(byte[] message, out Exception exception)
        {
            try
            {
                exception = null;
                UTF8Encoding encoder = new UTF8Encoding();
                return encoder.GetString(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static SynchronizationContext mContext = SynchronizationContext.Current;
        private static object mContextLock = new object();

        internal static void SetSynchronizationContext(SynchronizationContext context)
        {
            lock (mContextLock)
            {
                mContext = context;
                SynchronizationContext.SetSynchronizationContext(context);
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public static SynchronizationContext Context
        {
            get
            {
                if (mContext == null)
                    lock (mContextLock)
                        if (mContext == null)
                            mContext = SynchronizationContext.Current;
                return mContext;
            }
        }
        private static bool? debuggerInstalled;
        /// <summary>
        /// Gets a value indicating whether a debugger installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this a debugger is installed; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDebuggerInstalled
        {
            get
            {
                if (!debuggerInstalled.HasValue)
                {
                    debuggerInstalled = false;
                    RegistryKey regKey = null;
                    try
                    {
                        regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AeDebug");
                        if (regKey != null)
                            debuggerInstalled = regKey.GetValue("Debugger") != null;
                    }
                    catch
                    {

                    }
                    finally
                    {
                        try
                        {
                            if (regKey != null)
                                regKey.Close();
                        }
                        catch
                        {

                        }
                    }
                }

                return debuggerInstalled.Value;
            }
        }

        /// <summary>
        /// Provides a console-based timeout counter when debugging is requested
        /// </summary>
        /// <param name="timeOut">The time out.</param>
        /// <param name="forceWait">if set to <c>true</c> [force wait].</param>
        public static void DebugCheck(int timeOut, bool forceWait = false)
        {
            if (!System.Diagnostics.Debugger.IsAttached && (IsDebuggerInstalled || forceWait))
            {
                ConsoleKeyInfo cki = new ConsoleKeyInfo();
                Console.WriteLine();
                DateTime endTime = DateTime.Now.AddSeconds(timeOut);

                while (Console.KeyAvailable == false && endTime > DateTime.Now)
                {
                    Console.CursorLeft = 0;
                    Console.Write("Press D for debug mode, or any other key in {0:##0} seconds to begin processing... ", (endTime - DateTime.Now).TotalSeconds);

                    Thread.Sleep(250); // Loop until input is entered.
                }

                if (Console.KeyAvailable)
                {
                    cki = Console.ReadKey(true);
                    if (cki.KeyChar == 'D' || cki.KeyChar == 'd')
                        System.Diagnostics.Debugger.Launch();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether in visual studio.
        /// </summary>
        /// <value>
        /// <c>true</c> if in visual studio; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInVisualStudio
        {
            get
            {
                bool inIDE = false;
                string[] args = System.Environment.GetCommandLineArgs();
                if (args != null && args.Length > 0)
                {
                    string prgName = args[0].ToUpper();
                    inIDE = prgName.EndsWith("VSHOST.EXE");
                }
                return inIDE;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is debugging.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debugging; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDebugging
        {
            get
            {
                return IsInVisualStudio || System.Diagnostics.Debugger.IsAttached;
            }
        }

        /// <summary>
        /// Converts the exception to string.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static String ConvertExceptionToString(Exception exception)
        {
            return ConvertExceptionToString(exception, null, null);
        }

        /// <summary>
        /// Converts the exception to string prefixed with <see cref="string.Format(string, object[])" /> of <paramref name="format" /> and <paramref name="args" />
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static String ConvertExceptionToString(Exception exception, string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            if (format != null)
            {
                if (args != null)
                    sb.AppendFormat(format, args);
                else
                    sb.Append(format);
                sb.AppendLine();
                sb.AppendLine();
            }

            int errorCount = 0;
            while (exception != null)
            {
                errorCount++;
                if (sb.Length > 0)
                    sb.AppendLine(Environment.NewLine);

                COMException comEx = exception as COMException;
                if (comEx != null)
                    sb.AppendFormat("#{3} - {0}: Error {4} (0x{4:X8}) - {1}\r\nStack: {2}", exception.GetType().FullName, exception.Message, exception.StackTrace, errorCount, comEx.ErrorCode);
                else
                    sb.AppendFormat("#{3} - {0}: {1}\r\nStack: {2}", exception.GetType().FullName, exception.Message, exception.StackTrace, errorCount);

                exception = exception.InnerException;
            }
            return sb.ToString();
        }


        static DateTime start = GetDateAndWatch();
        static Stopwatch watch;

        private static DateTime GetDateAndWatch()
        {
            watch = Stopwatch.StartNew();
            return DateTime.Now;
        }

        /// <summary>
        /// Writes the log with information.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        [ConditionalAttribute("DEBUG")]
        public static void WriteLogWithInfo(string fileName, string format, params object[] args)
        {
            string info = null;
            if (start != null && watch != null)
            {
                TimeSpan elapsed = watch.Elapsed;
                DateTime accurate = start.Add(elapsed);
                Thread current = Thread.CurrentThread;
                info = string.Format("[{0:yyyy-MM-dd HH:mm:ss} - {1:0000.0000}] [Thread {2,3} - {3}] ", accurate, elapsed.TotalSeconds, current.ManagedThreadId, current.Name);
            }

            WriteLog(fileName, info + format, args);
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        [ConditionalAttribute("DEBUG")]
        public static void WriteLog(string fileName, string format, params object[] args)
        {
            if (args != null)
                format = string.Format(format, args);


            while (true)
            {
                try
                {
                    System.IO.File.AppendAllText(fileName, format);
                    break;
                }
                catch (IOException ex)
                {
                    if (ex == null)
                        throw;
                }
            }
        }

    }
}
