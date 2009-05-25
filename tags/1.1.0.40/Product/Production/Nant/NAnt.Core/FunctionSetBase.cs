// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
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
//
// Ian MacLean (ian_maclean@another.com)

using System;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace NAnt.Core {
    public abstract class FunctionSetBase {
        #region Protected Instance Constructors

        protected FunctionSetBase(Project project, Location location, PropertyDictionary properties)
        {
            _project = project;
            _properties = properties;
            _location = location;
        }
        
        #endregion Protected Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the <see cref="Project" /> that this functionset will 
        /// reference.
        /// </summary>
        /// <value>
        /// The <see cref="Project" /> that this functionset will reference.
        /// </value>
        public virtual Project Project {
            get { return _project; }
            set { _project = value; }
        }

        public virtual PropertyDictionary Properties {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        public virtual Location Location
        {
            get
            {
                return _location;
            }
            set
            {
            	_location = value;
            }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private Project _project = null;
        private PropertyDictionary _properties = null;
        private Location _location = null;

        #endregion Private Instance Fields
    }
}
