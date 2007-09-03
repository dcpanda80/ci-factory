// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
//
// John R. Hicks (angryjohn69@nc.rr.com)
// Gerry Shaw (gerry_shaw@yahoo.com)
// William E. Caputo (wecaputo@thoughtworks.com | logosity@yahoo.com)
// Gert Driesen (gert.driesen@ardatis.com)
//
// Some of this class was based on code from the Mono class library.
// Copyright (C) 2002 John R. Hicks <angryjohn69@nc.rr.com>
//
// The events described in this file are based on the comments and
// structure of Ant.
// Copyright (C) Copyright (c) 2000,2002 The Apache Software Foundation.
// All rights reserved.

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Text;

using CIScript.Core.Util;

namespace CIScript.Core {
    /// <summary>
    /// Defines the set of levels recognised by the CIScript logging system.
    /// </summary>
    public enum Level : int {
        /// <summary>
        /// Designates fine-grained informational events that are most useful 
        /// to debug a build process.
        /// </summary>
        Debug = 1000,

        /// <summary>
        /// Designates events that offer a more detailed view of the build 
        /// process.
        /// </summary>
        Verbose = 2000,

        /// <summary>
        /// Designates informational events that are useful for getting a 
        /// high-level view of the build process.
        /// </summary>
        Info = 3000,

        /// <summary>
        /// Designates potentionally harmful events.
        /// </summary>
        Warning = 4000,

        /// <summary>
        /// Designates error events.
        /// </summary>
        Error = 5000,

        /// <summary>
        /// Can be used to suppress all messages.
        /// </summary>
        /// <remarks>
        /// No events should be logged with this <see cref="Level" />.
        /// </remarks>
        None = 9999
    }

    /// <summary>
    /// Class representing an event occurring during a build.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An event is built by specifying either a project, a task or a target.
    /// </para>
    /// <para>
    /// A <see cref="Project" /> level event will only have a <see cref="Project" /> 
    /// reference.
    /// </para>
    /// <para>
    /// A <see cref="Target" /> level event will have <see cref="Project" /> and 
    /// <see cref="Target" /> references.
    /// </para>
    /// <para>
    /// A <see cref="Task" /> level event will have <see cref="Project" />, 
    /// <see cref="Target" /> and <see cref="Task" /> references.
    /// </para>
    /// </remarks>
    public class BuildEventArgs : EventArgs {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventArgs" /> 
        /// class.
        /// </summary>
        public BuildEventArgs() {
            _project = null;
            _target = null;
            _task = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventArgs" />
        /// class for a <see cref="Project" /> level event.
        /// </summary>
        /// <param name="project">The <see cref="Project" /> that emitted the event.</param>
        public BuildEventArgs(Project project) {
            _project = project;
            _target = null;
            _task = null;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventArgs" />
        /// class for a <see cref="Target" /> level event.
        /// </summary>
        /// <param name="target">The <see cref="Target" /> that emitted the event.</param>
        public BuildEventArgs(Target target) {
            _project = target.Project;
            _target = target;
            _task = null;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventArgs" />
        /// class for a <see cref="Task" /> level event.
        /// </summary>
        /// <param name="task">The <see cref="Task" /> that emitted the event.</param>
        public BuildEventArgs(Task task) {
            _project = task.Project;
            _target = task.Parent as Target;
            _task = task;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the message associated with this event.
        /// </summary>
        /// <value>
        /// The message associated with this event.
        /// </value>
        public string Message {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Gets or sets the priority level associated with this event.
        /// </summary>
        /// <value>
        /// The priority level associated with this event.
        /// </value>
        public Level MessageLevel {
            get { return _messageLevel; }
            set { _messageLevel = value; }
        }
    
        /// <summary>
        /// Gets or sets the <see cref="Exception" /> associated with this event.
        /// </summary>
        /// <value>
        /// The <see cref="Exception" /> associated with this event.
        /// </value>
        public Exception Exception {
            get { return _exception; }
            set { _exception = value; }
        }

        /// <summary>
        /// Gets the <see cref="Project" /> that fired this event.
        /// </summary>
        /// <value>
        /// The <see cref="Project" /> that fired this event.
        /// </value>
        public Project Project {
            get { return _project; }
        }

        /// <summary>
        /// Gets the <see cref="Target" /> that fired this event.
        /// </summary>
        /// <value>
        /// The <see cref="Target" /> that fired this event, or a null reference 
        /// if this is a <see cref="Project" /> level event.
        /// </value>
        public Target Target {
            get { return _target; }
        }

        /// <summary>
        /// Gets the <see cref="Task" /> that fired this event.
        /// </summary>
        /// <value>
        /// The <see cref="Task" /> that fired this event, or <see langword="null" />
        /// if this is a <see cref="Project" /> or <see cref="Target" /> level 
        /// event.
        /// </value>
        public Task Task {
            get { return _task; }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private readonly Project _project;
        private readonly Target _target;
        private readonly Task _task;
        private string _message;
        private Level _messageLevel = Level.Verbose;
        private Exception _exception;

        #endregion Private Instance Fields
    }

    /// <summary>
    /// Represents the method that handles the build events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="BuildEventArgs" /> that contains the event data.</param>
    public delegate void BuildEventHandler(object sender, BuildEventArgs e);

    /// <summary>
    /// Instances of classes that implement this interface can register to be 
    /// notified when things happen during a build.
    /// </summary>
    public interface IBuildListener {
        /// <summary>
        /// Signals that a build has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event is fired before any targets have started.
        /// </remarks>
        void BuildStarted(object sender, BuildEventArgs e);

        /// <summary>
        /// Signals that the last target has finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event will still be fired if an error occurred during the build.
        /// </remarks>
        void BuildFinished(object sender, BuildEventArgs e);

        /// <summary>
        /// Signals that a target has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        void TargetStarted(object sender, BuildEventArgs e);

        /// <summary>
        /// Signals that a target has finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event will still be fired if an error occurred during the build.
        /// </remarks>
        void TargetFinished(object sender, BuildEventArgs e);

        /// <summary>
        /// Signals that a task has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        void TaskStarted(object sender, BuildEventArgs e);

        /// <summary>
        /// Signals that a task has finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event will still be fired if an error occurred during the build.
        /// </remarks>
        void TaskFinished(object sender, BuildEventArgs e);

        /// <summary>
        /// Signals that a message has been logged.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        void MessageLogged(object sender, BuildEventArgs e);
    }

    /// <summary>
    /// Interface used by CIScript to log the build output. 
    /// </summary>
    /// <remarks>
    /// Depending on the supplied command-line arguments, CIScript will set the
    /// <see cref="OutputWriter" /> to <see cref="Console.Out" /> or a
    /// <see cref="StreamWriter" />  with a file as backend store.
    /// </remarks>
    public interface IBuildLogger : IBuildListener {
        /// <summary>
        /// Gets or sets the highest level of message this logger should respond 
        /// to.
        /// </summary>
        /// <value>The highest level of message this logger should respond to.</value>
        /// <remarks>
        /// Only messages with a message level higher than or equal to the given 
        /// level should actually be written to the log.
        /// </remarks>
        Level Threshold {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to produce emacs (and other
        /// editor) friendly output.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if output is to be unadorned so that emacs 
        /// and other editors can parse files names, etc.
        /// </value>
        bool EmacsMode {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="TextWriter" /> to which the logger is 
        /// to send its output.
        /// </summary>
        TextWriter OutputWriter {
            get;
            set;
        }

        /// <summary>
        /// Flushes buffered build events or messages to the underlying storage.
        /// </summary>
        void Flush();
    }

    [Serializable()]
    public class DefaultLogger : IBuildLogger {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLogger" /> 
        /// class.
        /// </summary>
        public DefaultLogger() {
        }

        #endregion Public Instance Constructors

        #region Implementation of IBuildLogger

        /// <summary>
        /// Gets or sets the highest level of message this logger should respond 
        /// to.
        /// </summary>
        /// <value>
        /// The highest level of message this logger should respond to.
        /// </value>
        /// <remarks>
        /// Only messages with a message level higher than or equal to the given 
        /// level should be written to the log.
        /// </remarks>
        public virtual Level Threshold {
            get { return _threshold; }
            set { _threshold = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to produce emacs (and other
        /// editor) friendly output.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if output is to be unadorned so that emacs 
        /// and other editors can parse files names, etc. The default is
        /// <see langword="false" />.
        /// </value>
        public virtual bool EmacsMode {
            get { return _emacsMode; }
            set { _emacsMode = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TextWriter" /> to which the logger is 
        /// to send its output.
        /// </summary>
        /// <value>
        /// The <see cref="TextWriter" /> to which the logger sends its output.
        /// </value>
        public virtual TextWriter OutputWriter {
            get { return _outputWriter; }
            set { _outputWriter = value; }
        }

        /// <summary>
        /// Flushes buffered build events or messages to the underlying storage.
        /// </summary>
        public virtual void Flush() {
            if (OutputWriter != null) {
                OutputWriter.Flush();
            }
        }

        #endregion Implementation of IBuildLogger

        #region Implementation of IBuildListener

        /// <summary>
        /// Signals that a build has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event is fired before any targets have started.
        /// </remarks>
        public virtual void BuildStarted(object sender, BuildEventArgs e) {
            _buildReports.Push(new BuildReport(DateTime.Now));
        }

        /// <summary>
        /// Signals that the last target has finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event will still be fired if an error occurred during the build.
        /// </remarks>
        public virtual void BuildFinished(object sender, BuildEventArgs e) {
            Exception error = e.Exception;
            int indentationLevel = 0;

            if (e.Project != null) {
                indentationLevel = e.Project.IndentationLevel * e.Project.IndentationSize;
            }

            BuildReport report = (BuildReport) _buildReports.Pop();

            if (error == null) {
                OutputMessage(Level.Info, string.Empty, indentationLevel);
                if (report.Errors == 0 && report.Warnings == 0) {
                    OutputMessage(Level.Info, "BUILD SUCCEEDED", indentationLevel);
                } else {
                    OutputMessage(Level.Info, string.Format(CultureInfo.InvariantCulture,
                        ResourceUtils.GetString("String_BuildSucceeded"), 
                        report.Errors, report.Warnings), indentationLevel);
                }
                OutputMessage(Level.Info, string.Empty, indentationLevel);
            } else {
                OutputMessage(Level.Error, string.Empty, indentationLevel);
                if (report.Errors == 0 && report.Warnings == 0) {
                    OutputMessage(Level.Error, "BUILD FAILED", indentationLevel);
                } else {
                    OutputMessage(Level.Info, string.Format(CultureInfo.InvariantCulture,
                        ResourceUtils.GetString("String_BuildFailed"), 
                        report.Errors, report.Warnings), indentationLevel);
                }
                OutputMessage(Level.Error, string.Empty, indentationLevel);

                if (error is BuildException) {
                    if (Threshold <= Level.Verbose) {
                        OutputMessage(Level.Error, error.ToString(), indentationLevel);
                    } else {
                        if (error.Message != null) {
                            OutputMessage(Level.Error, error.Message, indentationLevel);
                        }

                        // output nested exceptions
                        Exception nestedException = error.InnerException;
                        int exceptionIndentationLevel = indentationLevel;
                        int indentShift = 4; //e.Project.IndentationSize;
                        while (nestedException != null && !StringUtils.IsNullOrEmpty(nestedException.Message)) {
                            exceptionIndentationLevel += indentShift;
                            OutputMessage(Level.Error, nestedException.Message, exceptionIndentationLevel);
                            nestedException = nestedException.InnerException;
                        }
                    }
                } else {
                    OutputMessage(Level.Error, "INTERNAL ERROR", indentationLevel);
                    OutputMessage(Level.Error, string.Empty, indentationLevel);
                    OutputMessage(Level.Error, error.ToString(), indentationLevel);
                    OutputMessage(Level.Error, string.Empty, indentationLevel);
                    OutputMessage(Level.Error, "Please send bug report to nant-developers@lists.sourceforge.net.", indentationLevel);
                }

                OutputMessage(Level.Error, string.Empty, indentationLevel);
            }

            // output total build time
            TimeSpan buildTime = DateTime.Now - report.StartTime;
            OutputMessage(Level.Info, string.Format(CultureInfo.InvariantCulture, 
                ResourceUtils.GetString("String_TotalTime") + Environment.NewLine, 
                Math.Round(buildTime.TotalSeconds, 1)), indentationLevel);

            // make sure all messages are written to the underlying storage
            Flush();
        }

        /// <summary>
        /// Signals that a target has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        public virtual void TargetStarted(object sender, BuildEventArgs e) {
            int indentationLevel = 0;

            if (e.Project != null) {
                indentationLevel = e.Project.IndentationLevel * e.Project.IndentationSize;
            }

            if (e.Target != null) {
                OutputMessage(Level.Info, string.Empty, indentationLevel);
                OutputMessage(
                    Level.Info, 
                    string.Format(CultureInfo.InvariantCulture, "{0}:", e.Target.Name), 
                    indentationLevel);
                OutputMessage(Level.Info, string.Empty, indentationLevel);
            }
        }

        /// <summary>
        /// Signals that a task has finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event will still be fired if an error occurred during the build.
        /// </remarks>
        public virtual void TargetFinished(object sender, BuildEventArgs e) {
        }

        /// <summary>
        /// Signals that a task has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        public virtual void TaskStarted(object sender, BuildEventArgs e) {
        }

        /// <summary>
        /// Signals that a task has finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// This event will still be fired if an error occurred during the build.
        /// </remarks>
        public virtual void TaskFinished(object sender, BuildEventArgs e) {
        }

        /// <summary>
        /// Signals that a message has been logged.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="BuildEventArgs" /> object that contains the event data.</param>
        /// <remarks>
        /// Only messages with a priority higher or equal to the threshold of 
        /// the logger will actually be output in the build log.
        /// </remarks>
        public virtual void MessageLogged(object sender, BuildEventArgs e) {
            if (_buildReports.Count > 0) {
                if (e.MessageLevel == Level.Error) {
                    BuildReport report = (BuildReport) _buildReports.Peek();
                    report.Errors++;
                } else if (e.MessageLevel == Level.Warning) {
                    BuildReport report = (BuildReport) _buildReports.Peek();
                    report.Warnings++;
                }
            }

            // output the message
            OutputMessage(e);
        }

        #endregion Implementation of IBuildListener

        #region Protected Instance Methods

        /// <summary>
        /// Empty implementation which allows derived classes to receive the
        /// output that is generated in this logger.
        /// </summary>
        /// <param name="message">The message being logged.</param>
        protected virtual void Log(string message) {
        }

        #endregion Protected Instance Methods

        #region Private Instance Methods

        /// <summary>
        /// Outputs an indented message to the build log if its priority is 
        /// greather than or equal to the <see cref="Threshold" /> of the 
        /// logger.
        /// </summary>
        /// <param name="messageLevel">The priority of the message to output.</param>
        /// <param name="message">The message to output.</param>
        /// <param name="indentationLength">The number of characters that the message should be indented.</param>
        private void OutputMessage(Level messageLevel, string message, int indentationLength) {
            OutputMessage(CreateBuildEvent(messageLevel, message), indentationLength);
        }

        /// <summary>
        /// Outputs an indented message to the build log if its priority is 
        /// greather than or equal to the <see cref="Threshold" /> of the 
        /// logger.
        /// </summary>
        /// <param name="e">The event to output.</param>
        private void OutputMessage(BuildEventArgs e) {
            int indentationLength = 0;
            
            if (e.Project != null) {
                indentationLength = e.Project.IndentationLevel * e.Project.IndentationSize;
            }

            OutputMessage(e, indentationLength);
        }

        /// <summary>
        /// Outputs an indented message to the build log if its priority is 
        /// greather than or equal to the <see cref="Threshold" /> of the 
        /// logger.
        /// </summary>
        /// <param name="e">The event to output.</param>
        /// <param name="indentationLength">TODO</param>
        private void OutputMessage(BuildEventArgs e, int indentationLength) {
            if (e.MessageLevel >= Threshold) {
                string txt = e.Message;

                // beautify the message a bit
                txt = txt.Replace("\t", " "); // replace tabs with spaces
                txt = txt.Replace("\r", ""); // get rid of carriage returns

                // split the message by lines - the separator is "\n" since we've eliminated
                // \r characters
                string[] lines = txt.Split('\n');
                string label = String.Empty;

                if (e.Task != null && !EmacsMode) {
                    label = "[" + e.Task.Name + "] ";
                    label = label.PadLeft(e.Project.IndentationSize);
                }

                if (indentationLength > 0) {
                    label = new String(' ', indentationLength) + label;
                }

                foreach (string line in lines) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(label);
                    sb.Append(line);

                    string indentedMessage = sb.ToString();

                    // output the message to the console
                    Console.Out.WriteLine(indentedMessage);

                    // if an OutputWriter was set, write the message to it
                    if (OutputWriter != null) {
                        OutputWriter.WriteLine(indentedMessage);
                    }

                    Log(indentedMessage);
                }
            }
        }

        #endregion Private Instance Methods

        #region Private Static Methods

        private static BuildEventArgs CreateBuildEvent(Level messageLevel, string message) {
            BuildEventArgs buildEvent = new BuildEventArgs();
            buildEvent.MessageLevel = messageLevel;
            buildEvent.Message = message;
            return buildEvent;
        }

        #endregion Private Static Methods

        #region Private Instance Fields

        private Level _threshold = Level.Info;
        private TextWriter _outputWriter;
        private bool _emacsMode;

        #endregion Private Instance Fields

        #region Private Static Fields

        /// <summary>
        /// Holds a stack of reports for all running builds.
        /// </summary>
        private Stack _buildReports = new Stack();

        #endregion Private Static Fields
    }

    /// <summary>
    /// Used to store information about a build, to allow better reporting to 
    /// the user.
    /// </summary>
    [Serializable()]
    public class BuildReport {
        /// <summary>
        /// Errors encountered so far.
        /// </summary>
        public int Errors;

        /// <summary>
        /// Warnings encountered so far.
        /// </summary>
        public int Warnings;

        /// <summary>
        /// The start time of the build process.
        /// </summary>
        public DateTime StartTime;

        public BuildReport(DateTime startTime) {
            StartTime = startTime;
            Errors = 0;
            Warnings = 0;
        }
    }

    /// <summary>
    /// Contains a strongly typed collection of <see cref="IBuildListener"/> 
    /// objects.
    /// </summary>
    [Serializable()]
    public class BuildListenerCollection : CollectionBase {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListenerCollection"/> 
        /// class.
        /// </summary>
        public BuildListenerCollection() {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListenerCollection"/> 
        /// class with the specified <see cref="BuildListenerCollection"/> instance.
        /// </summary>
        public BuildListenerCollection(BuildListenerCollection value) {
            AddRange(value);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListenerCollection"/> 
        /// class with the specified array of <see cref="IBuildListener"/> instances.
        /// </summary>
        public BuildListenerCollection(IBuildListener[] value) {
            AddRange(value);
        }

        #endregion Public Instance Constructors
        
        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        [System.Runtime.CompilerServices.IndexerName("Item")]
        public IBuildListener this[int index] {
            get { return ((IBuildListener)(base.List[index])); }
            set { base.List[index] = value; }
        }

        #endregion Public Instance Properties

        #region Public Instance Methods
        
        /// <summary>
        /// Adds a <see cref="IBuildListener"/> to the end of the collection.
        /// </summary>
        /// <param name="item">The <see cref="IBuildListener"/> to be added to the end of the collection.</param> 
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(IBuildListener item) {
            return base.List.Add(item);
        }

        /// <summary>
        /// Adds the elements of a <see cref="IBuildListener"/> array to the end of the collection.
        /// </summary>
        /// <param name="items">The array of <see cref="IBuildListener"/> elements to be added to the end of the collection.</param> 
        public void AddRange(IBuildListener[] items) {
            for (int i = 0; (i < items.Length); i = (i + 1)) {
                Add(items[i]);
            }
        }

        /// <summary>
        /// Adds the elements of a <see cref="BuildListenerCollection"/> to the end of the collection.
        /// </summary>
        /// <param name="items">The <see cref="BuildListenerCollection"/> to be added to the end of the collection.</param> 
        public void AddRange(BuildListenerCollection items) {
            for (int i = 0; (i < items.Count); i = (i + 1)) {
                Add(items[i]);
            }
        }
        
        /// <summary>
        /// Determines whether a <see cref="IBuildListener"/> is in the collection.
        /// </summary>
        /// <param name="item">The <see cref="IBuildListener"/> to locate in the collection.</param> 
        /// <returns>
        /// <see langword="true" /> if <paramref name="item"/> is found in the 
        /// collection; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(IBuildListener item) {
            return base.List.Contains(item);
        }
        
        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.        
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The array must have zero-based indexing.</param> 
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(IBuildListener[] array, int index) {
            base.List.CopyTo(array, index);
        }
        
        /// <summary>
        /// Retrieves the index of a specified <see cref="IBuildListener"/> object in the collection.
        /// </summary>
        /// <param name="item">The <see cref="IBuildListener"/> object for which the index is returned.</param> 
        /// <returns>
        /// The index of the specified <see cref="IBuildListener"/>. If the <see cref="IBuildListener"/> is not currently a member of the collection, it returns -1.
        /// </returns>
        public int IndexOf(IBuildListener item) {
            return base.List.IndexOf(item);
        }
        
        /// <summary>
        /// Inserts a <see cref="IBuildListener"/> into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="IBuildListener"/> to insert.</param>
        public void Insert(int index, IBuildListener item) {
            base.List.Insert(index, item);
        }
        
        /// <summary>
        /// Returns an enumerator that can iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="BuildListenerEnumerator"/> for the entire collection.
        /// </returns>
        public new BuildListenerEnumerator GetEnumerator() {
            return new BuildListenerEnumerator(this);
        }
        
        /// <summary>
        /// Removes a member from the collection.
        /// </summary>
        /// <param name="item">The <see cref="IBuildListener"/> to remove from the collection.</param>
        public void Remove(IBuildListener item) {
            base.List.Remove(item);
        }
        
        #endregion Public Instance Methods
    }

    /// <summary>
    /// Enumerates the <see cref="IBuildListener"/> elements of a <see cref="BuildListenerCollection"/>.
    /// </summary>
    public class BuildListenerEnumerator : IEnumerator {
        #region Internal Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListenerEnumerator"/> class
        /// with the specified <see cref="BuildListenerCollection"/>.
        /// </summary>
        /// <param name="arguments">The collection that should be enumerated.</param>
        internal BuildListenerEnumerator(BuildListenerCollection arguments) {
            IEnumerable temp = (IEnumerable) (arguments);
            _baseEnumerator = temp.GetEnumerator();
        }

        #endregion Internal Instance Constructors

        #region Implementation of IEnumerator
            
        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        public IBuildListener Current {
            get { return (IBuildListener) _baseEnumerator.Current; }
        }

        object IEnumerator.Current {
            get { return _baseEnumerator.Current; }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the enumerator was successfully advanced 
        /// to the next element; <see langword="false" /> if the enumerator has 
        /// passed the end of the collection.
        /// </returns>
        public bool MoveNext() {
            return _baseEnumerator.MoveNext();
        }

        bool IEnumerator.MoveNext() {
            return _baseEnumerator.MoveNext();
        }
            
        /// <summary>
        /// Sets the enumerator to its initial position, which is before the 
        /// first element in the collection.
        /// </summary>
        public void Reset() {
            _baseEnumerator.Reset();
        }
            
        void IEnumerator.Reset() {
            _baseEnumerator.Reset();
        }

        #endregion Implementation of IEnumerator

        #region Private Instance Fields
    
        private IEnumerator _baseEnumerator;

        #endregion Private Instance Fields
    }

    /// <summary>
    /// Implements a <see cref="TextWriter" /> for writing information to 
    /// the CIScript logging infrastructure.
    /// </summary>
    public class LogWriter : TextWriter {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWriter" /> class 
        /// for the specified <see cref="Task" /> with the specified output 
        /// level and format provider.
        /// </summary>
        /// <param name="task">Determines the indentation level.</param>
        /// <param name="outputLevel">The <see cref="Level" /> with which messages will be output to the build log.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider" /> object that controls formatting.</param>
        public LogWriter(Task task, Level outputLevel, IFormatProvider formatProvider) : base(formatProvider) {
            _task = task;
            _outputLevel = outputLevel;
        }

        #endregion Public Instance Constructors

        #region Override implementation of TextWriter

        /// <summary>
        /// Gets the <see cref="Encoding" /> in which the output is written.
        /// </summary>
        /// <value>
        /// The <see cref="LogWriter" /> always writes output in UTF8 
        /// encoding.
        /// </value>
        public override Encoding Encoding {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Writes a character array to the text stream.
        /// </summary>
        /// <param name="chars">The character array to write to the text stream.</param>
        public override void Write(char[] chars) {
            _message += new string(chars, 0, chars.Length -1);
        }

        public override void Write(string value) {
            _message += value;
        }

        public override void WriteLine() {
            WriteLine(string.Empty);
        }


        /// <summary>
        /// Writes a string followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is a null reference, only the line termination characters are written.</param>
        public override void WriteLine(string value) {
            _task.Log(OutputLevel, _message + value);
            _message = string.Empty;
        }

        /// <summary>
        /// Writes out a formatted string using the same semantics as 
        /// <see cref="string.Format(string, object[])" />.
        /// </summary>
        /// <param name="line">The formatting string.</param>
        /// <param name="args">The object array to write into format string.</param>
        public override void WriteLine(string line, params object[] args) {
            _task.Log(OutputLevel, _message + string.Format(
                CultureInfo.InvariantCulture, line, args));
            _message = string.Empty;
        }   

        public override void Close() {
            if (_message.Length != 0) {
                _task.Log(OutputLevel, _message);
                _message = string.Empty;
            }
            base.Close();
        }

        #endregion Override implementation of TextWriter

        #region Override implementation of MarshalByRefObject

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for 
        /// this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="ILease" /> used to control the lifetime 
        /// policy for this instance. This is the current lifetime service object 
        /// for this instance if one exists; otherwise, a new lifetime service 
        /// object initialized with a lease that will never time out.
        /// </returns>
        public override Object InitializeLifetimeService() {
            ILease lease = (ILease) base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial) {
                lease.InitialLeaseTime = TimeSpan.Zero;
            }
            return lease;
        }

        #endregion Override implementation of MarshalByRefObject

        #region Protected Instance Properties

        /// <summary>
        /// Gets the <see cref="Level" /> with which messages will be output to
        /// the build log.
        /// </summary>
        protected Level OutputLevel {
            get { return _outputLevel; }
        }

        #endregion Protected Instance Properties

        #region Private Instance Fields

        private Task _task;
        private Level _outputLevel;
        private string _message = string.Empty;

        #endregion Private Instance Fields
    }
}
