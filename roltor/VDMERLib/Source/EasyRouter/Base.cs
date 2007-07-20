using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter
{
    public class Base
    {
        private bool m_bDirty = false;
        public bool IsDirty
        {
            get { return m_bDirty; }
            set { m_bDirty = value; }
        }

    }
}
