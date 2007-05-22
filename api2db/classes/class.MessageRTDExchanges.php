<?php

/*////////////////////////////
	class.rtd2db.php

	This is the

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////

class CMessageRTDExchanges extends CMessageRTD
{

	public $aaExchanges = Array(); // holds all exchange data

	public function GetExchangeCountry($exchangeId) { return $this->aaExchanges[$exchangeId]["fid_exchange_country"]; }
	public function GetExchangeText($exchangeId) { return $this->aaExchanges[$exchangeId]["fid_text"]; }
	public function GetExchangeSymbol($exchangeId){ return $this->aaExchanges[$exchangeId]["fid_exchange_symbol"]; }

	public function GetRequest()
	{
		$requests = "10|15|0|105|1";
		$requests = strtr($requests,"|",chr(31));
		return $requests;
	}

	public function DecodeResponse(&$sMessage)
	{
		$aMessage = explode(chr(31),$sMessage);
		$key = "fid_exchange_id";

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case $key:
				case "fid_exchange_symbol":
				case "fid_exchange_country":
				case "fid_text":
					$aFields[$this->aFieldTypes[$aMessage[$i]]] = $aMessage[++$i];
					break;
				default:
					$i++;
					break;
			}
		}
		// check that key exists
		if(array_key_exists($key, $aFields))
			// It exists
			$this->aaExchanges[$aFields[$key]] = $aFields;
		else
			// THIS SHOULD NEVER HAPPEN!
			wlog(get_class($this), "W Exchange message failed to decode. Response was: ". $sMessage);
	}



}

?>