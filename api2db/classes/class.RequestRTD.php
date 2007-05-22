<?php

/*////////////////////////////
	class.rtd2db.php

	This is the abstract Request class

	Benn Eichhorn
	30 Jan 2007
*/// //////////////////////////

abstract class CRequestRTD {

   public $oDatabase; // db_mssql object

   public $oApi; // rtdapi object

   protected $aRequestTypes = Array(
				33=>"rid_trade_t",
				4=>"rid_contract_t",
				2=>"rid_account_t",
				86=>"rid_usr_t",
				12=>"rid_exchange_t",
				3=>"rid_answer_t",
				80=>"rid_group_t",
				272=>"rid_price_req_load_exchange_t",
				21=>"rid_price_t"
				);

	public $aaSettings;

	abstract public function Initialise();
   abstract public function SendRequests();
   abstract protected function ParseResponse($rid, &$message);


/*
   public function GetResponse()
   {
      wlog(get_class($this), "Requests sent. Reading responses...");

      // MAIN PROGRAM LOOP
      while ($response = $this->oApi->readRequestNormal())
		{
			$rid = substr($response, 0, strpos($response, chr(31)));
			$response = substr($response, strpos($response, chr(31))+1);
			$this->ParseResponse($rid, $response);
      } // while
   }
*/
// OLD METHOD FOR PROCESSING MESSAGES

   public function GetResponse()
   {
      wlog(get_class($this), "Requests sent. Reading responses...");

      $reply = "";
      // MAIN PROGRAM LOOP
      while ($reply .= $this->oApi->readRequestBinary()) { // append to data left from previous request
         // if the last character isn't the EOT character, then this contains a part response
         //print ".";
         if ($reply[strlen($reply)-1] != chr(10))
			{
            $nlpos = strrpos($reply, chr(10));
            if ($nlpos !== false)
				{
					// has a full response plus a part response
               // process the full response sub string
               $aResponses = explode(chr(10), trim(substr($reply, 0, $nlpos + 1)));
               $reply = substr($reply, $nlpos + 1); // keep the part response
            }
            // else it is only part of another response so leave $reply set
         }
			else
			{
				// is simply a full response, so process it
            $aResponses = explode(chr(10), trim($reply));
            $reply = "";
         }

         // now parse each message one at a time
	      foreach($aResponses AS $response)
			{
				$rid = substr($response, 0, strpos($response, chr(31)));
				$response = substr($response, strpos($response, chr(31))+1);
				$this->ParseResponse($rid, $response);
			}
      } // while
   }

}
?>