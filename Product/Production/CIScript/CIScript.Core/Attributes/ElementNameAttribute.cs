// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Ian MacLean (ian_maclean@another.com)

using System;
using System.Reflection;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Indicates that class should be treated as a CIScript element.
    /// </summary>
    /// <remarks>
    /// Attach this attribute to a subclass of Element to have CIScript be able
    /// to recognize it.  The name should be short but must not confict
    /// with any other element already in use.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class ElementNameAttribute : Attribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cre="ElementNameAttribute" /> 
        /// with the specified name.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="name" /> is a zero-length <see cref="string" />.</exception>
        public ElementNameAttribute(string name) {
            Name = name;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name {
            get { return _name; }
            set { 
                if (value == null) {
                    throw new ArgumentNullException("name");
                }

                // Element names cannot have whitespace at the beginning, 
                // or end.
                _name = value.Trim(); 

                if (_name.Length == 0) {
                    throw new ArgumentOutOfRangeException("name", value, "A zero-length string is not an allowed value.");
                }
            }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private string _name;

        #endregion Private Instance Fields
    }
}
