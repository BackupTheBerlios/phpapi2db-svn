<?php

/*////////////////////////////
	class.rtd2db.php

	This is the

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////

class CMessageRTDAccounts extends CMessageRTD
{

	public $aaAccounts = Array(); // holds all account data

	public function GetAccountSpec1($accountId) { return $this->aaAccounts[$accountId]["fid_account_spec1"]; }
	public function GetAccountSpec2($accountId) { return $this->aaAccounts[$accountId]["fid_account_spec2"]; }
	public function GetGroupID($accountId) { return $this->aaAccounts[$accountId]["fid_group_id"]; }

	public function GetRequest()
	{
		$requests = "1|15|0|105|1";
		$requests = strtr($requests,"|",chr(31));
		return $requests;
	}

	public function DecodeResponse(&$sMessage)
	{
		$aMessage = explode(chr(31),$sMessage);
		$key = "fid_account_id";

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case $key:
				case "fid_account_spec1":
				case "fid_account_spec2":
				case "fid_group_id":
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
			$this->aaAccounts[$aFields[$key]] = $aFields;
		else
			// THIS SHOULD NEVER HAPPEN!
			wlog(get_class($this), "W Account message failed to decode. Response was: ". $sMessage);
	}



}

?>
