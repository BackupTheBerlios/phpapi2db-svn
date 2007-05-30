<?php

/*////////////////////////////
	class.rtd2db.php

	This is the

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////


class CMessageRTDPrices extends CMessageRTD
{

	public $aPrice = Array(); // holds all data for exchanges. Key = fid_exchange_id

	private $aExchanges = Array();

	// holds a reference to calling class to gain access to
	// all message structures, api and database abjects
	private $oRequestRTD;

	function __construct(&$oRequestRTD)
	{
		$this->oRequestRTD =& $oRequestRTD;

		if(!isset($this->oRequestRTD->aaSettings["EXCHANGES"]))
		{
			wlog(get_class($this), "E Exchanges not found in INI file!!! Can't continue! Blowing up ungracefully...");
			unset($this->oRequestRTD);
			exit();
		}


		if(!isset($this->oRequestRTD->aaSettings["QUICKTICK"]["TABLE_PREFIX"],
			$this->oRequestRTD->aaSettings["QUICKTICK"]["DATABASE"],
			$this->oRequestRTD->aaSettings["QUICKTICK"]["SQLTYPE"]))
		{
			wlog(get_class($this), "E Prices table and database settings not found in INI file!!! Can't continue! Blowing up ungracefully...");
			unset($this->oRequestRTD);
			exit();
		}
		/*
		else
		{ // We have Quiktick info, so lets process for Quicktick
			$this->CheckQuicktick();
		}
		*/
		$this->aExchanges = $this->oRequestRTD->aaSettings["EXCHANGES"];
	}

	public function GetRequest()
	{
		$requests = "";
		foreach(array_keys($this->aExchanges) AS $exchangeId)
		{
			// rid_price_req_load_exchange_t
			$requests .= "272|47|$exchangeId|15|0|105|1".chr(10);
		}
		$requests = strtr(trim($requests),"|",chr(31));
		return $requests;
	}

	public function DecodeResponse(&$sMessage)
	{
		unset($this->aPrice);

		// message has arrived, timestamp it!!!!
		list($usec, $sec) = explode(" ", microtime());

		$this->aPrice["tickTime"] = $sec;
		$this->aPrice["tickMicroTime"] = $usec;

		$aMessage = explode(chr(31),$sMessage);

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case "fid_contract_id":
				case "fid_currency_id":
				case "fid_exchange_id":
				case "fid_total_volume":
				case "fid_phase":
				case "fid_ask_qty":
				case "fid_bid_qty":
				case "fid_best_ask":
				case "fid_best_bid":
				case "fid_last":
				case "fid_last_qty":
					$this->aPrice[$this->aFieldTypes[$aMessage[$i]]] = $aMessage[++$i];
					break;
				default:
					$i++;
					break;
			}
		}
		// message decoded
		// no key check
		// send back the contract type
		return $this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_contract_type"];
	}


	public function ProcessResponse()
	{
		if(isset($this->oRequestRTD->aaSettings["QUICKTICK"]))
			$this->ProcessQuicktick();
	}


	private function ProcessQuicktick()
	{


		if($this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_total_volume"] < $this->aPrice["fid_total_volume"])
		{
			// Update this contracts total volume
			$this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_total_volume"] = $this->aPrice["fid_total_volume"];

			// Get symbol depending on contract type
            // get the SYMBOLOGY

			switch($this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_contract_type"])
			{
				case "1":
					$symbol = $this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_symbol"];
					break;

				case "7":
					$symbol = $this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_symbol"] . " " .
					$this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_expiration_month"];
					break;

				case "8":
				case "10":
					$symbol = $this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_symbol"] . " " .
					$this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_expiration_month"].
					isset($this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_strike"]) ? " ".$this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_strike"]." " : "" .
					isset($this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_put_call"]) ? ($this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_contract_id"]]["fid_put_call"] == 1 ? " P" : " C") : "";
					break;

				default:
					wlog(get_class($this), "E Contract types not found in price message!!! FREAKING OUT!!!! Can't continue, blowing up ungracefully...");
					exit(1);
			}
			$data["symbol"] = $symbol;


			$data["tickTime"] = date("Y-m-d H:i:s", $this->aPrice["tickTime"]);
			$data["tickMicroTime"] = $this->aPrice["tickMicroTime"];
			$data["exchangeId"] = $this->aPrice["fid_exchange_id"];
			$data["currency"] = $this->oRequestRTD->oContracts->aaContracts[$this->aPrice["fid_currency_id"]]["fid_symbol"];
			$data["lastQty"] = $this->aPrice["fid_last_qty"];
			$data["lastPrice"] = $this->aPrice["fid_last"];
			$data["bidQty"] = $this->aPrice["fid_bid_qty"];
			$data["bidPrice"] = $this->aPrice["fid_best_bid"];
			$data["askQty"] = $this->aPrice["fid_ask_qty"];
			$data["askPrice"] = $this->aPrice["fid_best_ask"];
			$data["totalVolume"] = $this->aPrice["fid_total_volume"];
			$data["phaseId"] = $this->aPrice["fid_phase"];

			// select the TRADE database
			$this->oRequestRTD->oDatabase->SelectDatabase($this->oRequestRTD->aaSettings["QUICKTICK"]["DATABASE"]);
			// send trade to the database
			$this->oRequestRTD->oDatabase->InsertQuery($this->oRequestRTD->aaSettings["QUICKTICK"]["TABLE_PREFIX"]."_".date("Ym"), $data);
		}

	}

}

?>