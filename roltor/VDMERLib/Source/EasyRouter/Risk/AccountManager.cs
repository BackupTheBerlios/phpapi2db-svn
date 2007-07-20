using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MESSAGEFIX3Lib;

namespace VDMERLib.EasyRouter.Risk
{
    public class AccountManager : Dictionary<long,Account>
    {
        public ArrayList ComboList = new ArrayList();
        public List<Allocation> Allocations = new List<Allocation>();

        /// <summary>
        /// Decode price FIX message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public bool DecodeFIX(IFIXMessage FIXMsg)
        {
            Account theAccount;
            long lAccountID = FIXMsg.get_AsNumber(FIXTagConstants.esFIXTagESAccountID);
            if (!this.ContainsKey(lAccountID))
            {
                theAccount = new Account(lAccountID);
                this.Add(lAccountID, theAccount);
                ComboList.Add(theAccount);
            }
            else
                theAccount = this[lAccountID];
            if (FIXMsg.MsgType == FIXMsgConstants.esFIXMsgESRiskAccount)
                return theAccount.DecodeRiskAccountFIXMessage(FIXMsg);
            else
                return theAccount.DecodeFIXMessage(FIXMsg);

        }

        /// <summary>
        /// Find Account , if no account id provided then see if there is only
        /// one account , get these details and add the account ud to the outgoing fixmessage
        /// </summary>
        /// <param name="lAccountID"></param>
        /// <param name="message"></param>
        /// <returns>the account info object</returns>
        public Account FindAccount(long? lAccountID,FIXMessage message)
        {
            Account theAccount = null;

            if (lAccountID.HasValue)
            {
               if (this.ContainsKey(lAccountID.Value))
                   theAccount = this[lAccountID.Value];
            }
            else
            { 
               if (this.Count == 1)
                {
                    Dictionary<long, Account>.Enumerator it = this.GetEnumerator();
                    if (it.MoveNext())
                    {
                        theAccount = it.Current.Value;
                        message.set_AsNumber(FIXTagConstants.esFIXTagESAccountID,(int)it.Current.Key);
                    }
                }
               
            }
            return theAccount;
        }


    }
}
