// NAnt - A .NET build tool
// Copyright (C) 2001-2002 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

// Ian MacLean (imaclean@gmail.com)

using System;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;

using NAnt.Core.Attributes;

namespace NAnt.Core {
    public class DataTypeBaseBuilder {
        #region Public Instance Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="DataTypeBaseBuilder" /> class
        /// for the specified Element class in the assembly specified.
        /// </summary>
        /// <param name="className">The class representing the Element.</param>
        /// <param name="assemblyFileName">The assembly containing the Element.</param>/// 
        public DataTypeBaseBuilder(Type dataType) {
            _Type = dataType;
            _className = dataType.FullName;
            _assemblyFileName = dataType.Assembly.Location;

            Assembly assembly = GetAssembly();
            // get Element name from attribute
            ElementNameAttribute ElementNameAttribute = (ElementNameAttribute) 
                Attribute.GetCustomAttribute(assembly.GetType(ClassName), typeof(ElementNameAttribute));

            _elementName = ElementNameAttribute.Name;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        public Type Type{
            get
            {
                return _Type;
            }
        }

        public string ClassName {
            get { return _className; }
        }

        public string AssemblyFileName {
            get { return _assemblyFileName; }
        }

        public string DataTypeName {
            get { return _elementName; }
        }

        #endregion Public Instance Properties

        #region Public Instance Methods

        [ReflectionPermission(SecurityAction.Demand, Flags=ReflectionPermissionFlag.NoFlags)]
        public DataTypeBase CreateDataTypeBase() {
            Assembly assembly = GetAssembly();
            return (DataTypeBase) assembly.CreateInstance(
                ClassName, 
                true, 
                BindingFlags.Public | BindingFlags.Instance,
                null,
                null,
                CultureInfo.InvariantCulture,
                null);
        }

        #endregion Public Instance Methods

        #region Private Instance Methods

        private Assembly GetAssembly() {
            Assembly assembly = null;

            if (AssemblyFileName == null) {
                assembly = Assembly.GetExecutingAssembly();
            } else {
                //check to see if it is loaded already
                Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < ass.Length; i++) {
                    try {
                        string assemblyLocation = ass[i].Location;

                        if (assemblyLocation != null && assemblyLocation == AssemblyFileName) {
                            assembly = ass[i];
                            break;
                        }
                    } catch (NotSupportedException) {
                        // dynamically loaded assemblies do not not have a location
                    }
                }
                //load if not loaded
                if (assembly == null) {
                    assembly = Assembly.LoadFrom(AssemblyFileName);
                }                
            }
            return assembly;
        }

        #endregion Private Instance Methods

        #region Private Instance Fields

        string _className;
        string _assemblyFileName;
        private Type _Type;
        string _elementName;

        #endregion Private Instance Fields
    }
}
