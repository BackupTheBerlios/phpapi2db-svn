using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.EasyRouterClient
{
    /// <summary>
    /// Specific for FIX
    /// </summary>
    public class ERCSFIXClient : ERCSClient 
    {
        /// <summary>
        /// 
        /// </summary>
        public override void  RegisterCallbacks()
        {
 	         base.RegisterCallbacks();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UnRegisterCallBack()
        {
            base.UnRegisterCallBack();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static new public ERCSFIXClient GetInstance()
        {
            lock (theLock)
            {
                if (ERCSClient.m_objClient == null)
                    ERCSClient.m_objClient = new ERCSFIXClient();
            }
            return (ERCSFIXClient) m_objClient;
        }

    }
}
