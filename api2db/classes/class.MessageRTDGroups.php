<?php

/*////////////////////////////
	class.rtd2db.php

	This is the

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////

class CMessageRTDGroups extends CMessageRTD
{

	public $aaGroups = Array(); // holds all group data

	public function GetGroupName($groupId) { return $this->aaGroups[$groupId]["fid_name"]; }

	public function GetRequest()
	{
		$requests = "79|15|0|105|1";
		$requests = strtr($requests,"|",chr(31));
		return $requests;
	}

	public function DecodeResponse(&$sMessage)
	{
		$aMessage = explode(chr(31),$sMessage);
		$key = "fid_id";

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case $key:
				case "fid_name":
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
			$this->aaGroups[$aFields[$key]] = $aFields;
		else
			// THIS SHOULD NEVER HAPPEN!
			wlog(get_class($this), "W Group message failed to decode. Response was: ". $sMessage);
	}


}

?>