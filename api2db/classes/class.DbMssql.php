<?php

/*////////////////////////////
	class.db_mssql.php

	MSSQL Database Wrapper

	Handles database connection and required database functions.

	Benn Eichhorn
	13 May 2007
*/////////////////////////////


class CDbMssql extends CDb
{

	// Connects to specified host and returns the connection
	protected function connect($sHost, $sUsername, $sPassword)
	{
		$mscon = mssql_connect($sHost, $sUsername, $sPassword);
		if($mscon === FALSE)
		{
			wLog(get_class($this), "Couldn't connect to SQL Server on $sHost");
			wLog(get_class($this), "Last message on server was: " . mssql_get_last_message());
			die("Couldn't connect to SQL Server on $sHost");
		}
		return $mscon;
	} // function


	// Closes the database connection
	protected function close()
	{
		$msclose = mssql_close($this->connection);
		if($msclose === FALSE)
		{
			wLog(get_class($this), "Couldn't close db connection");
	      wLog(get_class($this), "Last message on server was: " . mssql_get_last_message());
			die("Couldn't close db connection");
		}
		return;
	} // function


	// Selects specified database
	public function SelectDatabase($sDatabase)
	{
		$msdb = mssql_select_db($sDatabase, $this->connection);
		if($msdb === FALSE)
		{
			wLog(get_class($this), "Couldn't open database $mssqlDB");
			wLog(get_class($this), "Last message on server was: " . mssql_get_last_message());
			die("Couldn't open database $mssqlDB");
		}
		return;
	} // function


	// Execute a select query
	public function SelectQuery($sTable, $sData, $sOrder = NULL, $bOrder = "ASC")
	{
		// remove null data
		foreach(array_keys($sData, NULL) AS $key)
		{
			unset($sData[$key]);
		}
		$sQuery =  "SELECT [";
		$sQuery .= implode("],[", array_keys($sData));
		$sQuery .= "] FROM $sTable";
		if($sOrder == NULL)
		{
			$sQuery =  " ORDER BY [";
			$sQuery .= implode("],[", array_values($sData));
			$sQuery .= "] $bOrder";
		}
		// send to the database
		$result = mssql_query($sQuery, $this->connection);
		if($result === FALSE)
		{
			wLog(get_class($this), "Couldn't execute query: $sQuery");
			wLog(get_class($this), "Last message on server was: " . mssql_get_last_message());
			die("Couldn't execute query");
		}
		// Put results into an array and return
		while($row = mssql_fetch_assoc($result))
		{
			$rows[] = $row;
		}
		mssql_free_result($result);
		return $rows;
	} // function



	public function SelectQueryWhere($sTable, Array $aData, Array $aWhere = NULL, Array $aOrder = NULL, $sOrder = "ASC")
	{
		// remove null data
		foreach(array_keys($aData, NULL) AS $key)
		{
			unset($aData[$key]);
		}
		$sQuery =  "SELECT [";
		$sQuery .= implode("],[", array_keys($aData));
		$sQuery .= "] FROM $sTable";
		if($aWhere != NULL && count($aWhere) > 0)
		{
			$sQuery =  " WHERE (";
			$sQuery .= implode(") AND (", array_values($aData));
			$sQuery .= ")";
		}
		if($sOrder != NULL)
		{
			$sQuery =  " ORDER BY [";
			$sQuery .= implode("],[", array_values($aData));
			$sQuery .= "] $sOrder";
		}
		// send to the database
		$result = mssql_query($sQuery, $this->connection);
		if($result === FALSE)
		{
			wLog(get_class($this), "Couldn't execute query: $sQuery");
			wLog(get_class($this), "Last message on server was: " . mssql_get_last_message());
			die("Couldn't execute query");
		}
		// Put results into an array and return
		while($row = mssql_fetch_assoc($result))
		{
			$rows[] = $row;
		}
		mssql_free_result($result);
		return $rows;
	} // function


	// Execute a select max query
	// expects single column
	// returns single value
	public function SelectMax($sTable, $sColumn)
	{
		$sQuery =  "SELECT MAX([{$sColumn}]) FROM $sTable";
		// send to the database
		$result = mssql_query($sQuery, $this->connection);
		if($result === FALSE)
		{
			wLog(get_class($this), "Couldn't execute query: $sQuery");
			wLog(get_class($this), "Last message on server was: " . mssql_get_last_message());
			die("Couldn't execute query");
		}
		// Put results into an array and return
		$row = mssql_fetch_assoc($result);
		return $row[$sColumn];
	} // function


	// Inserts a trade into the database
	// Requires the table name and an array of key=>value pairs to be inserted as column=>data pairs
	// If primaryKey is set, the row will be updated instead of inserted
	public function InsertQuery($sTable, Array $aData)
	{
		// remove null data
		foreach(array_keys($aData, NULL) AS $key)
		{
			unset($aData[$key]);
		}

		$sQuery =  "INSERT INTO $sTable ([";
		$sQuery .= implode("],[", array_keys($aData));
		$sQuery .= "]) VALUES ('";
		$sQuery .= implode("','", array_values($aData));
		$sQuery .= "')";

		$result = mssql_query($sQuery, $this->connection);

		if($result === FALSE)
		{
			wLog(get_class($this), "E Couldn't execute query: $sQuery");
			wLog(get_class($this), "E Last message on server was: " . mssql_get_last_message());
			// email an alert to IT
			//$this->alertEmail("Query failed", $msg);
			die("E Couldn't execute query");
		}
	} // function


	/*
	// sends and email to alert IT
	public function alertEmail($subject, $message)
	{
		$subject = str_replace(Array("'", '"', "`"), "", $subject);
		$message = str_replace(Array("'", '"', "`"), "", $message);
		$subject = "rtd2gtrade.php : $subject";
		$message = "<font face=\"Tahoma\"><p>{$subject} at " . date("YmdHis") . "</p><br><p>$message</p>";
		$toEmail = "it@uk.vandermoolen.com";
		$fromEmail = "vdm44mas1@equk.moolen.com";
		$query = "EXEC sp_send_cdohtmlmail '$fromEmail', '$toEmail', '$subject', '$message'";
		$result = mssql_query($query);

	}
	*/

} // class

?>
