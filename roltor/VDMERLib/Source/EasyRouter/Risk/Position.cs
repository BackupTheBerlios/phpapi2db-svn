using System;
using System.Collections.Generic;
using System.Text;
using MESSAGEFIX3Lib;

namespace VDMERLib.EasyRouter.Risk
{
    public class Position : RiskEventArg 
    {
        private double? m_dPAL;

        /// <summary>
        /// PAL
        /// </summary>
        public double? PAL { get { return m_dPAL; } }

        private double? m_dNetPos;

        private int? m_iRoundTrips;
        
        /// <summary>
        /// Round Trips
        /// </summary>
        public int? RoundTrips
        {
            get { return m_iRoundTrips; }
        }

        private int? m_iNoFills;
        /// <summary>
        /// NumberFills
        /// </summary>
        public int? NumberFills
        {
            get { return m_iNoFills; }
        }

        /// <summary>
        /// Net Position
        /// </summary>
        public double? NetPosition { get { return m_dNetPos; } }

        /// <summary>
        /// ES Ticker Mnemonic
        /// </summary>
        private string m_sSymbol = string.Empty;

        /// <summary>
        /// ES Ticker Mnemonic
        /// </summary>
        public string Symbol { get { return m_sSymbol; } set { m_sSymbol = value; } }


        protected long m_iAccountID;
        /// <summary>
        /// AccountID
        /// </summary>
        public long AccountID { get { return m_iAccountID; } }

        public Position(long iAccountID)
        {
            m_iAccountID = iAccountID;
        }
         /// <summary>
        /// Decode position FIX message
        /// </summary>
        /// <param name="FIXGroup"></param>
        /// <returns></returns>
        public bool DecodeFIXGroup(IFIXGroup fixRptGroup)
        {
            //-----------Security Level Positions-----------------------------------------
            IFIXGroup fixPosCountGroup = fixRptGroup.GetGroupByTag(FIXTagConstants.esFIXTagNoPositions,null);
            if (fixPosCountGroup!=null)
            {
                for (int i = 0; i < fixPosCountGroup.get_NumberOfGroups(null); i++)
                {
                    IFIXGroup fixPosRptGroup = fixPosCountGroup.GetGroupByIndex(i);

                    switch (fixPosRptGroup.get_AsChar(FIXTagConstants.esFIXTagPosType))
                    {
                        case FIXPosTypeConstants.esFIXPositionTypeFilled:
                            SetPosition (fixPosRptGroup,  out m_dNetPos);
                            break;
                        case FIXPosTypeConstants.esFIXPositionTypeRoundTrips:
                        {
                            double dblLongPos, dblShortPos, dblTotalPos;
                            if (fixPosRptGroup.GetDouble(out dblLongPos, FIXTagConstants.esFIXTagLongQty) && fixPosRptGroup.GetDouble(out dblShortPos, FIXTagConstants.esFIXTagShortQty))
                            {
                                m_iRoundTrips = Int32.Parse(Math.Min(dblLongPos, dblShortPos).ToString());
                                dblTotalPos = dblLongPos + dblShortPos;
                                m_iNoFills = Int32.Parse(dblTotalPos.ToString());
                            }
                            else
                                m_iRoundTrips = null;
                            break;
                        }
                    }
                }
            }

            IFIXGroup fixPosAmountGroup = fixRptGroup.GetGroupByTag(FIXTagConstants.esFIXTagNoPosAmt, null);
            if (fixPosAmountGroup!=null)
            {
                for (int i = 0; i < fixPosAmountGroup.get_NumberOfGroups(null); i++)
                {
                    IFIXGroup fixAmtRptGroup = fixPosAmountGroup.GetGroupByIndex(i);

                    switch (fixAmtRptGroup.get_AsChar(FIXTagConstants.esFIXTagPosAmtType))
                    {
                        case FIXPosAmtTypeConstants.esFIXPosAmtTypePAL:
                            SetPrice (fixAmtRptGroup,  out m_dPAL);
                            break;

                        
                            
                            
                    }
                }
            }
            return true;
        }

        public void SetPosition (IFIXGroup FIXGroup, out double? dblPosition)
        {
            double dblValue;
            if (FIXGroup.GetDouble(out dblValue, FIXTagConstants.esFIXTagPosQty))
                dblPosition = dblValue;
            else
                dblPosition = null;
            IsDirty = true;
        }

        public void SetPrice(IFIXGroup FIXGroup, out double? dblPrice)
        {
            double dblValue;
            if (FIXGroup.GetDouble(out dblValue, FIXTagConstants.esFIXTagPosAmt))
                dblPrice = dblValue;
            else
                dblPrice = null;
            IsDirty = true;
        }
    }
}
