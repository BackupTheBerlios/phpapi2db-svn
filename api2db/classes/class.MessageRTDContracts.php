<?php

/*////////////////////////////
	class.rtd2db.php

	This is the

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////


class CMessageRTDContracts extends CMessageRTD
{

	public $aaContracts = Array(); // holds all data for exchanges. Key = fid_exchange_id

	public function GetSymbol($contractId) { return $this->aaContracts[$contractId]["fid_symbol"]; }
	public function GetISIN($contractId) { return $this->aaContracts[$contractId]["fid_isin"]; }
	public function GetSEDOL($contractId) { return $this->aaContracts[$contractId]["fid_local_security_number"]; }
	public function GetCountry($contractId) { return $this->aaContracts[$contractId]["fid_country"]; }
	public function GetContractType($contractId) { return $this->aaContracts[$contractId]["fid_contract_type"]; }
	public function GetExpiryMonth($contractId) { return $this->aaContracts[$contractId]["fid_expiration_month"]; }

	// holds a reference to calling class to gain access to
	// all message structures, api and database abjects
	private $oRequestRTD;

	function __construct(&$oRequestRTD)
	{
		$this->oRequestRTD =& $oRequestRTD;

		if(!isset($this->oRequestRTD->aaSettings["CONTRACT_TYPES"]))
		{
			wlog(get_class($this), "E Contract Types not found in INI file!!! Can't continue! Blowing up ungracefully...");
			unset($this->oRequestRTD);
		}
	}

	public function GetRequest()
	{
		$requests = "";
		foreach(array_keys($this->oRequestRTD->aaSettings["CONTRACT_TYPES"]) AS $contractType)
		{
			$requests .= "124|19|$contractType|359|0|15|0|105|1".chr(10);
		}
		$requests = strtr(trim($requests),"|",chr(31));
		return $requests;
	}

	public function DecodeResponse(&$sMessage)
	{
		$aMessage = explode(chr(31),$sMessage);
		$key = "fid_contract_id";

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case $key:
				case "fid_symbol":
				case "fid_isin":
				case "fid_local_security_number":
				case "fid_country":
				case "fid_contract_type":
				case "fid_expiration_month":
				case "fid_exchange_contract_id":
					$aFields[$this->aFieldTypes[$aMessage[$i]]] = $aMessage[++$i];
					break;
				default:
					$i++;
					break;
			}
		}
		// check that key exists
		if(array_key_exists($key, $aFields))
		{
			// the key exists - which it should!!
			$aFields["fid_total_volume"] = 0;
			$this->aaContracts[$aFields[$key]] = $aFields;
		}
		else
		{
			// THIS SHOULD NEVER HAPPEN!
			wlog(get_class($this), "W Contract message failed to decode. Response was: ". $sMessage);
		}
	}

}

?>
