<?php

/*////////////////////////////
	class.rtd2db.php

	This is the

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////

class CMessageRTDAnswer extends CMessageRTD
{

	public $aaAnswer = Array(); // holds all answer data

	public function GetCount($accountId) { return $this->aaAccounts[$accountId]["fid_count"]; }
	public function GetErrorCode($accountId) { return $this->aaAccounts[$accountId]["fid_error"]; }
	public function GetContext($accountId) { return $this->aaAccounts[$accountId]["fid_context"]; }

	function __construct()
	{
    $this->aaAnswer["fid_context"] = 0;
    $this->aaAnswer["fid_count"] = 0;
	}
	
  public function GetRequest() {return "";}

	public function DecodeResponse(&$sMessage)
	{
		$aMessage = explode(chr(31),$sMessage);

		for($i=0; $i<count($aMessage); $i++)
		{
			switch($this->aFieldTypes[$aMessage[$i]])
			{
				case "fid_context":
				case "fid_error":
				case "fid_count":
					$aFields[$this->aFieldTypes[$aMessage[$i]]] = $aMessage[++$i];
					break;
				default:
					$i++;
					break;
			}
		}
	}

}

?>
