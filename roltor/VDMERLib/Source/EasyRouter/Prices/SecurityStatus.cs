/*
** SecurityStatus.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Market Modes
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using MESSAGEFIX3Lib;
using EASYROUTERCOMCLIENTLib;
using System.Windows.Forms;

using VDMIFIXMessage = EASYROUTERCOMCLIENTLib.IFIXMessage;
using VDMFIXTagConstants = EASYROUTERCOMCLIENTLib.FIXTagConstants;
using VDMFIXExecutionReportConstants = MESSAGEFIX3Lib.FIXExecutionReportConstants;
using VDMFIXSecurityTradingStatusConstants = MESSAGEFIX3Lib.FIXSecurityTradingStatusConstants;

namespace VDMERLib.EasyRouter.Prices
{
    /// <summary>
    /// Class SecurityStatus
    /// </summary>
    public class SecurityStatus : PricesEventArg
    {
        /// <summary>
        /// List of market modes 
        /// </summary>
        ArrayList m_status = new ArrayList();

        /// <summary>
        /// Returns the arraylist of market modes for this instrument
        /// </summary>
        public ArrayList Status
        {
            get { { return m_status; } }
        }
        
        /// <summary>
        /// Vanilla
        /// </summary>
        public SecurityStatus()
        {
            m_OrderType = PriceDataType.MarketMode;
        }

        /// <summary>
        /// Get Market mode from FIX Message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public bool DecodeFIX(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            bool bRetVal = false;

            m_sSymbol = FIXMsg.get_AsString(VDMFIXTagConstants.esFIXTagESTickerMnemonic);
           
            EASYROUTERCOMCLIENTLib.IFIXGroup group = FIXMsg.GetGroupByTag(VDMFIXTagConstants.esFIXTagESNoTradingStatus, null);

            if (group != null)
            {
                int nCount = group.get_NumberOfGroups(null);

                for (int i = 0; i < nCount; i++)
                {
                    EASYROUTERCOMCLIENTLib.IFIXGroup singleGroup = group.GetGroupByIndex(i);
                    int nStatus = singleGroup.get_AsNumber(VDMFIXTagConstants.esFIXTagSecurityTradingStatus);
                    VDMFIXSecurityTradingStatusConstants constant = (VDMFIXSecurityTradingStatusConstants)nStatus;
                    m_status.Add(constant); 
                }
                if(m_status.Count > 0) 
                    bRetVal = true;
            }
            return bRetVal;
        }

        /// <summary>
        /// list of all market modes in use friendly format
        /// </summary>
        /// <returns></returns>
        public string ModeToString()
        {
            return ModeToString(string.Empty); 
        }

        /// <summary>
        /// list of all market modes in use friendly format
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public string ModeToString(string sData)
        {
            if (m_status.Count == 0)
                return sData + "[Unknown]";
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(sData);
                for (int i = 0; i < m_status.Count; i++)
                {
                    string sMode = GetMode((FIXSecurityTradingStatusConstants)m_status[i]);
                    if (sMode != string.Empty)
                    {
                        builder.Append("[" + sMode + "]");
                    }
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// list of all market modes in use friendly format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
             return ModeToString(m_sSymbol + " State: ");
        }

        /// <summary>
        /// Get user friendly market mode description from market mode code
        /// </summary>
        /// <param name="constant"></param>
        /// <returns></returns>
        string GetMode(FIXSecurityTradingStatusConstants constant)
        {
            switch (constant)
            {
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraVOLA: return "XetraVOLA";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraTRADE: return "XetraTRADE";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraSUSP: return "XetraSUSP";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraSTART: return "XetraSTART";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraQPREC: return "XetraQPREC";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraQCALL: return "XetraQCALL";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraPRETR: return "XetraPRETR";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraPOSTR: return "XetraPOSTR";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraOIPO: return "XetraOIPO";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraOFRZ: return "XetraOFRZ";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraOCALL: return "XetraOCALL";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraIIPO: return "XetraIIPO";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraICALL: return "XetraICALL";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraHALT: return "XetraHALT";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraENDTR: return "XetraENDTR";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraDEL: return "XetraDEL";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraCCALL: return "XetraCCALL";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusXetraADD: return "XetraADD";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAACHClosureAuction: return "MTAACHClosureAuction";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAVCHValidationOfClosure: return "MTAVCHValidationOfClosure";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAPCHPreClosing: return "MTAPCHPreClosing";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAENDEndService: return "MTAENDEndService";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTATAHTradingAfterHours: return "MTATAHTradingAfterHours";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAMGTManagementPhase: return "MTAMGTManagementPhase";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTATEREndService: return "MTATEREndService";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTACHIClosure: return "MTACHIClosure";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTACANTradeCancellation: return "MTACANTradeCancellation";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTANEGContinuousTrading: return "MTANEGContinuousTrading";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAVALValidation: return "MTAVALValidation";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAPREPreOpening: return "MTAPREPreOpening";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMTAINIStartService: return "MTAINIStartService";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusExPitProfOpen: return "ExPitProfOpen";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusExPitBlockOpen: return "ExPitBlockOpen";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusQuoteSpreadMultiplier3: return "QuoteSpreadMultiplier3";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusQuoteSpreadMultiplier2: return "QuoteSpreadMultiplier2";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusQuoteSpreadMultiplier1: return "QuoteSpreadMultiplier1";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOddLotActionDelete: return "OddLotActionDelete";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOddLotAllowed: return "OddLotAllowed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusEndOfClearingDay: return "EndOfClearingDay";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusInternalFullDepth: return "InternalFullDepth";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusExternalFullDepth: return "ExternalFullDepth";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusFillAndKillAllowed: return "FillAndKillAllowed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusFillOrKillAllowed: return "FillOrKillAllowed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOrderBookChangesAvail: return "OrderBookChangesAvail";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusEditedOrderBookChangesAvail: return "EditedOrderBookChangesAvail";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusFillKillAllowed: return "FillKillAllowed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMktOrdersAllowed: return "MktOrdersAllowed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPriceQuotationReqd: return "PriceQuotationReqd";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusEnabled: return "Enabled";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusSessionLate: return "SessionLate";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusSessionNormal: return "SessionNormal";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusSessionRestrictedOpen: return "SessionRestrictedOpen";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusCommTest: return "CommTest";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusSimulatedData: return "SimulatedData";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusReqQuote: return "ReqQuote";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPostSettleSess: return "PostSettleSess";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusLate: return "Late";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusHalted: return "Halted";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusCrossed: return "Crossed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusLocked: return "Locked";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNotFirm: return "NotFirm";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusTradingAtLastPrice: return "TradingAtLastPrice";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusInterventionAfterOpening: return "InterventionAfterOpening";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusInterventionBeforeOpening: return "InterventionBeforeOpening";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMinibatch: return "Minibatch";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusTransient: return "Transient";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusInterruption: return "Interruption";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusProhibited: return "Prohibited";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPostTrading: return "PostTrading";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusSurveillanceInterruption: return "SurveillanceInterruption";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusConsultationEnd: return "ConsultationEnd";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusTrading: return "Trading";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOpening: return "Opening";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusConsulting: return "Consulting";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPreopening: return "Preopening";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNormal: return "Normal";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusFrozen: return "Frozen";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusSuspended: return "Suspended";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusReserved: return "Reserved";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusForbidden: return "Forbidden";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusAuthorised: return "Authorised";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusLimitsEnabled: return "LimitsEnabled";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPreClose: return "PreClose";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusStartOfPreOpen: return "StartOfPreOpen";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusFast: return "Fast";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOpen: return "Open";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusClosed: return "Closed";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOpeningDelay: return "OpeningDelay";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusTradingHalt: return "TradingHalt";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusResume: return "Resume";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNoOpenNoResume: return "NoOpenNoResume";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPriceIndication: return "PriceIndication";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusTradingRangeIndication: return "TradingRangeIndication";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMarketImbalanceBuy: return "MarketImbalanceBuy";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMarketImbalanceSell: return "MarketImbalanceSell";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMarketOnCloseImbalanceBuy: return "MarketOnCloseImbalanceBuy";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusMarketOnCloseImbalanceSell: return "MarketOnCloseImbalanceSell";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNotAssigned: return "NotAssigned";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNoMarketImbalance: return "NoMarketImbalance";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNoMarketOnCloseImbalance: return "NoMarketOnCloseImbalance";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusITSPreOpening: return "ITSPreOpening";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNewPriceIndication: return "NewPriceIndication";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusTradeDisseminationTime: return "TradeDisseminationTime";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusReadyToTrade: return "ReadyToTrade";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNotAvailableForTrading: return "NotAvailableForTrading";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusNotTradedOnThisMarket: return "NotTradedOnThisMarket";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusUnknown: return "Unknown";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusPreOpen: return "PreOpen";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusOpeningRotation: return "OpeningRotation";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusFastMarket: return "FastMarket";
                case FIXSecurityTradingStatusConstants.esFIXSecurityTradingStatusUninitialised: return "Uninitialised";
                default: return string.Empty; 								
            }
        }
    }
}
