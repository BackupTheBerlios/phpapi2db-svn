<?php

/*////////////////////////////
	class.dbMAS.php

	MAS Database Wrapper

	This is a wrapper to allow the rtd2sql class to be independent of a database

	Benn Eichhorn
	Van der Moolen Equities UK Ltd
	11 Oct 2005
*/////////////////////////////


/////// SUPPLIES ////////////////////////////////////////////////////////////////

include_once(dirname($_SERVER['PHP_SELF']) . "/inc.settings.php");


/////// CLASSES /////////////////////////////////////////////////////////////////

/*
	dbMAS Class

	-Connects to a MS SQL database
	-Given a specified host and database from constants

*/
class dbMAS
{

	// database connection
	private $connection;


	// Constructor - creates db connection and selects database
	function __construct()
	{
		$this->connection = $this->connect(RTD2SQL_db_host, RTD2SQL_db_username, RTD2SQL_db_password);
		$this->selectDb(RTD2SQL_db_database, $this->connection);
	}


	// Desctuctor
	function __destruct()
	{
		$this->close($this->connection);
	}


	/////// PRIVATE ////////////////////////////////////////////////////////////////


	// Connects to specified host and returns the connection
	private function connect($hostname, $username, $password)
	{
		$mscon = mssql_connect($hostname, $username, $password);
		if($mscon === FALSE)
		{
			$msg = "Couldn't connect to SQL Server on $mssqlServer\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			die("Couldn't connect to SQL Server on $mssqlServer");
		}
		return $mscon;
	} // function


	// Selects specified database
	private function selectDb($database, $connection)
	{
		$msdb = mssql_select_db($database, $connection);
		if($msdb === FALSE)
		{
			$msg = "Couldn't open database $mssqlDB \n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			die("Couldn't open database $mssqlDB");
		}
		return;
	} // function


	// Closes the database connection
	private function close($connection)
	{
		$msclose = mssql_close($connection);
		if($msclose === FALSE)
		{
			$msg = "Couldn't close connection $mssqlDB \n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			die("Couldn't close connection $mssqlDB");
		}
		return;
	} // function


	/////// PUBLIC /////////////////////////////////////////////////////////////////


	// Execute a query
	public function execQuery($query)
	{
		$result = mssql_query($query, $this->connection);
		if($result === FALSE)
		{
			$msg = "Couldn't execute query: $query \n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			die("Couldn't execute query");
		}
	} // function


	// Fetches the first row of the supplied T-SQL query
	public function fetchRow($query)
	{
		$result = mssql_query($query, $this->connection);
		if($result === FALSE)
		{
			$msg = "Couldn't execute query: $query \n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			die("Couldn't execute query");
		}
		$row = mssql_fetch_row($result);
		mssql_free_result($result);
		return $row;
	} // function


	// Fetches an array of the results from the supplied T-SQL query
	public function fetchArray($query)
	{
		$result = mssql_query($query, $this->connection);
		if($result === FALSE)
		{
			$msg = "Couldn't execute query: $query \n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			die("Couldn't execute query");
		}
		while($row = mssql_fetch_array($result))
		{
			$rows[] = $row;
		}
		mssql_free_result($result);
		return $rows;
	} // function


	// Inserts a trade into the database
	// Requires the table name and an array of key=>value pairs to be inserted as column=>data pairs
	// If primaryKey is set, the row will be updated instead of inserted
	public function insertTrade($table, $data, $primaryKey = NULL)
	{
		// remove null data
		foreach(array_keys($data, NULL) AS $key)
		{
			unset($data[$key]);
		}

		if(($primaryKey === NULL) OR
			( $this->fetchRow("SELECT [$primaryKey] FROM $table WHERE ([$primaryKey] = '" . $data["$primaryKey"] . "')") === FALSE))
			// catches a scenario for manual trades
		{
			$query =  "INSERT INTO $table ([";
			$query .= implode("], [", array_keys($data));
			$query .= "]) VALUES ('";
			$query .= implode("', '", array_values($data));
			$query .= "')";

			$result = mssql_query($query, $this->connection);
		}
		else
		{
			$query =  "UPDATE $table SET ";
			foreach($data AS $key => $value)
			{
				$dataConcat[] = "[$key] = '$value'";
			}
			$query .= implode(", ", $dataConcat);
			$query .= " WHERE ([";
			$query .= $primaryKey . "] = '" . $data[$primaryKey];
			$query .= "')";

			$result = mssql_query($query, $this->connection);
		}


		if($result === FALSE)
		{
			$msg = "Couldn't execute query: $query \n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			$msg = "Last message on server was: " . mssql_get_last_message() . "\n";
			file_put_contents (RTD2SQL_log_main, date("YmdHis ") . $msg , FILE_APPEND);
			// email an alert to IT
			$this->alertEmail("Query failed", $msg);
			die("Couldn't execute query");
		}
	} // function


	// sends and email to alert IT
	public function alertEmail($subject, $message)
	{
		$subject = str_replace(Array("'", '"', "`"), "", $subject);
		$message = str_replace(Array("'", '"', "`"), "", $message);
		$subject = "rtd2sql.php : $subject";
		$message = "<font face=\"Tahoma\"><p>{$subject} at " . date("YmdHis") . "</p><br><p>$message</p>";
		$toEmail = "it@uk.vandermoolen.com";
		$fromEmail = "vdm44mas1@equk.moolen.com";
		$query = "EXEC sp_send_cdohtmlmail '$fromEmail', '$toEmail', '$subject', '$message'";
		$result = mssql_query($query);

	}

} // class

?>