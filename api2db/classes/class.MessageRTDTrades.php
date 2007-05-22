<?php

/*////////////////////////////
	class.rtd2db.php

	This class behaves a little different to the other message classes as it doesn't
	retain an array of the data. When a new trade message is decoded it overwrites
	the previos value.

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////

class CMessageRTDTrades extends CMessageRTD
{
	// structure for each trade
	private $aTrade = Array("fid_contracts_long"=>0,"fid_contracts_short"=>0);
	// stock settlements
	private $aStockSettlements;
	// exchange settlements
	private $aExchangeHolidays;

	// holds a reference to calling class to gain access to
	// all message structures, api and database abjects
	private $oRequestRTD;

	function __construct(&$oRequestRTD)
	{
		$this->oRequestRTD =& $oRequestRTD;

		if(isset($this->oRequestRTD->aaSettings["TRADE"]))
		{
			if(!isset($this->oRequestRTD->aaSettings["TRADE"]["TRADE_DB"], $this->oRequestRTD->aaSettings["TRADE"]["TRADE_TABLE"]))
			{
				wlog(get_class($this), "E Trade settings settings not found in INI file!!! Can't continue! Blowing up ungracefully...");
				unset($this->oRequestRTD);
			}
		}

		if(isset($this->oRequestRTD->aaSettings["ML"]))
		{
			if(!isset($this->oRequestRTD->aaSettings["ML"]["ML_DB"], $this->oRequestRTD->aaSettings["ML"]["ML_TABLE"]))
			{
				wlog(get_class($this), "E ML settings settings not found in INI file!!! Can't continue! Blowing up ungracefully...");
				unset($this->oRequestRTD);
			}
		}
	}


	public function LoadSettlements()
	{
		// select the ML database
		$this->oRequestRTD->oDatabase->SelectDatabase($this->oRequestRTD->aaSettings["ML"]["ML_DB"]);

		$table = "exchangeHolidays";
		$data = Array("exchange","holidayDate");
		$aResults = $this->oRequestRTD->oDatabase->SelectQuery($table, $data);
		foreach($aResults AS $row)
		{
			$this->aExchangeHolidays[$row["exchange"]][$row["holidayDate"]] = 1;
		}

		$table = "stockSettlements";
		$data = Array("exchange","isin","days");
		$this->oRequestRTD->oDatabase->SelectQuery($table, $data);
		foreach($aResults AS $row)
		{
			$this->aStockSettlements[$row["exchange"]][$row["isin"]] = $row["days"];
		}
	}

	public function GetRequest()
	{
		$requests = "32|359|1|99|".$this->oRequestRTD->iLastTradeId."|15|0|105|1";
		$requests = strtr($requests,"|",chr(31));
		return $requests;
	}

	public function DecodeResponse(&$sMessage)
	{
		unset($this->aTrade);
		$aMessage = explode(chr(31),$sMessage);
		$key = "fid_trade_id";

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case $key:
				case "fid_contract_id":
				case "fid_underlying_contract_id":
				case "fid_currency_id":
				case "fid_exchange_id":
				case "fid_account_id":
				case "fid_price":
				case "fid_booking_price":
				case "fid_value":
				case "fid_contracts_long":
				case "fid_contracts_short":
				case "fid_trade_flags":
				case "fid_date":
				case "fid_time":
				case "fid_rtd_order_id":
				case "fid_order_number":
				case "fid_trade_number":
				case "fid_trade_text":
					$this->aTrade[$this->aFieldTypes[$aMessage[$i]]] = $aMessage[++$i];
					break;
				default:
					$i++;
					break;
			}
		}
		// check that key exists
		if(!array_key_exists($key, $this->aTrade))
			// THIS SHOULD NEVER HAPPEN!
			wlog(get_class($this), "W Trade message failed to decode. Response was: ". $sMessage);
	}

	public function ProcessResponse()
	{
		$contractId = $this->aTrade["fid_contract_id"];
		$contractType = $this->oRequestRTD->oContracts->GetContractType($contractId);
		switch($contractType)
		{
			case 1:
				$this->InsertContractType1();
				break;
			default:
				wLog(get_class($this), "W Trade message with unmapped contract id. contract_type=$contractType contract_id=$contractId");
				break;
		}
	}

	private function InsertContractType1()
	{
		// if manual trade then ignore it
		if ($this->aTrade["fid_trade_flags"] == 128 || $this->aTrade["fid_trade_flags"] == 129 ||	$this->aTrade["fid_trade_flags"] == 130)
		{
			wlog(get_class($this), "I Manual Trade Found : " . $trade["fid_trade_id"] . " (Ignoring)");
			break;
		}

		if(isset($this->oRequestRTD->aaSettings["ML"]))
		{
			/* PROCESS FOR ML */
			// default - new trade
			$data["action"] = "N";
			// default
			$data["transactionType"] = "SC";
			// will display trade text stored in the RTD user profile
			$data["bookCode"] = $this->aTrade["fid_trade_text"];
			$data["bookCodeDescription"] = $this->oRequestRTD->oUsers->GetUserName($this->aTrade["fid_user_id"]);
			$data["tradeReference"] = $this->aTrade["fid_trade_id"];
			$data["securityCodePrefix"] = "SEDL";
			$data["securityCode"] = $this->oRequestRTD->oContracts->GetSEDOL($this->aTrade["fid_contract_id"]);
			$data["securityDescription"] = $this->oRequestRTD->oContracts->GetSymbol($this->aTrade["fid_contract_id"]);
			$data["currency"] = $this->oRequestRTD->oContracts->GetSymbol($this->aTrade["fid_currency_id"]);
			$data["brokerCode"] = $this->oRequestRTD->aaSettings["TRADE"]["BROKERCODE"];
			$data["operation"] = (($this->aTrade["fid_contracts_long"] - $this->aTrade["fid_contracts_long"]) > 0 ? "B" : "S"); // Buy or sell?;
			$data["quantity"] = abs($this->aTrade["fid_contracts_long"] - $this->aTrade["fid_contracts_long"]);
			$data["price"] = (float) $this->aTrade["fid_price"];
			$data["netAmount"] = (float) abs( (float) $this->aTrade["fid_value"]);
			$data["tradeDate"] =	date("Ymd", strtotime($this->aTrade["fid_date"]));
			$data["tradeTime"] =	date("H:i", strtotime($this->aTrade["fid_date"] . " " . $this->aTrade["fid_time"]));
			// get settlement date
			$data["settlementDate"] = date("Ymd", strtotime(
				$this->SettlementCheckStock(
					$this->aTrade["fid_date"],
					$this->oRequestRTD->oContracts->GetISIN($this->aTrade["fid_contract_id"]),
					$this->oRequestRTD->oExchanges->GetExchangeSymbol($this->aTrade["fid_exchange_id"]),
					$this->oRequestRTD->aaSettings["ML"]["SETTLEMENT_DEFAULT"])
					)
				);
			$data["agentIndicator"] = (($this->aTrade["fid_contracts_long"] - $this->aTrade["fid_contracts_long"]) > 0 ? "P" : "S");
			$data["orderReference"] = $this->aTrade["fid_order_number"];
			$data["executionReference"] = $this->aTrade["fid_trade_number"];
			$data["marketClientIndicatorField"] = "M";
			$data["transactionReportMarker"] = "R";
			$data["transactionReportBIC"] = $this->oRequestRTD->oAccounts->GetAccountSpec2($this->aTrade["fid_member_id"]);
			$data["marketOfExecution"] = "LN";

			//print_r($data);
			//return;
			// select the ML database
			//$this->oRequestRTD->oDatabase->SelectDatabase($this->oRequestRTD->aaSettings["ML"]["ML_DB"]);
			// send to the database
			//$this->oRequestRTD->oDatabase->InsertQuery($this->oRequestRTD->aaSettings["ML"]["ML_TABLE"], $data);
			unset($data);
		}

		if(isset($this->oRequestRTD->aaSettings["TRADE"]))
		{
			/* PROCESS FOR P&L */
			$data["tradeDateTime"] = date("c", strtotime($this->aTrade["fid_date"] . " " . $this->aTrade["fid_time"]));
			$data["exchange"] = $this->oRequestRTD->oExchanges->GetExchangeSymbol($this->aTrade["fid_exchange_id"]);
			$data["isin"] = $this->oRequestRTD->oContracts->GetISIN($this->aTrade["fid_contract_id"]);
			$data["symbol"] = $this->oRequestRTD->oContracts->GetSymbol($this->aTrade["fid_contract_id"]);
			$data["currency"] = $this->oRequestRTD->oContracts->GetSymbol($this->aTrade["fid_currency_id"]);
			$data["quantity"] = $this->aTrade["fid_contracts_long"] - $this->aTrade["fid_contracts_long"];
			$data["price"] = (float) $this->aTrade["fid_price"];
			$data["account"] = $this->oRequestRTD->oAccounts->GetAccountSpec1($this->trade["fid_account_id"])." ".$this->oRequestRTD->oAccounts->GetAccountSpec2($this->trade["fid_account_id"]);
			$data["aggressor"] = ($this->aTrade["fid_trade_flags"] == 4096 ? "Y": "N");
			$data["exchangeOrderId"] = $this->aTrade["fid_order_number"];
			$data["exchangeTradeId"] = $this->aTrade["fid_trade_number"];
			$data["internalOrderId"] = $this->aTrade["fid_rtd_order_id"];
			$data["internalTradeId"] = $this->aTrade["fid_trade_id"];
			$data["internalSource"] = $this->oRequestRTD->aaSettings["TRADE"]["SOURCENAME"];
			// select the TRADE database
			//$this->oRequestRTD->oDatabase->SelectDatabase($this->oRequestRTD->aaSettings["ML"]["TRADE_DB"]);
			// send trade to the database
			//$this->oRequestRTD->oDatabase->InsertQuery($this->oRequestRTD->aaSettings["TRADE"]["TRADE_TABLE"], $data);


			$aUrl[] = "t=" . urlencode($data["tradeDateTime"]);
			$aUrl[] = "e=" . urlencode($data["exchange"]);
			$aUrl[] = "i=" . urlencode($data["isin"]);
			$aUrl[] = "s=" . urlencode($data["symbol"]);
			$aUrl[] = "c=" . urlencode($data["currency"]);
			$aUrl[] = "q=" . urlencode($data["quantity"]);
			$aUrl[] = "p=" . urlencode($data["price"]);
			$aUrl[] = "acc=" . urlencode($data["account"]);
			$aUrl[] = "agg=" . urlencode($data["aggressor"]);
			$aUrl[] = "eoi=" . urlencode($data["exchangeOrderId"]);
			$aUrl[] = "eti=" . urlencode($data["exchangeTradeId"]);
			$aUrl[] = "ioi=" . urlencode($data["internalOrderId"]);
			$aUrl[] = "iti=" . urlencode($data["internalTradeId"]);
			$aUrl[] = "is=" . urlencode($data["internalSource"]);

			$url = implode("&",$aUrl);

			print "?" . $url . "\n";

			unset($data);
			unset($aUrl);
		}
	}


   // skim through days to calculate the settlement date
   // check to see if a specific stock has a special settlement date
   // else use the default amount set in a constant
   private function SettlementCheckStock($date, $isin, $exchange, $daysSettlement = 3)
   {
	   // set to default 3 if all other settings don't exist!
		if(isset($this->aStockSettlements[$exchange][$isin]))
			$daysSettlement = $this->aStockSettlements[$exchange][$isin];

      // progress through days until you reach the settlement date
      $i = 0;
      while($i < $daysSettlement) {
         $date = date("Ymd", strtotime("+1 days", strtotime($date)));
         $i += $this->SettlementCheckDate($date, $exchange);
      }
      return date("Y/m/d", strtotime($date));
   }

   // Checks the settlement date against a table of exchange holidays
   private function SettlementCheckDate($settlement, $exchange)
   {
      $day = date("D", strtotime($settlement));
      if(($day == "Sat")
            or ($day == "Sun")
            or (isset($this->aExchangeHolidays[$exchange][$settlement])))
         return 0;
      else
			return 1;
   }


}

?>