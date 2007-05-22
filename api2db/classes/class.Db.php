<?php

/*////////////////////////////
	class.db_mssql.php

	MSSQL Database Wrapper

	Handles database connection and required database functions.

	Benn Eichhorn
	30 Jan 2007
*/////////////////////////////


/*
	db_mssql Class

	-Connects to a MS SQL database
	-Given a specified host and database from constants

*/
abstract class CDb
{

	// database connection
	protected $connection;

	// Constructor - creates db connection and selects database
	function __construct($sHost, $sUsername, $sPassword)
	{
		$this->connection = $this->connect($sHost, $sUsername, $sPassword);
	}

	// Desctuctor
	function __destruct()
	{
		$this->close();
	}


	/////// PRIVATE ////////////////////////////////////////////////////////////////


	// Connects to specified host and returns the connection
	abstract protected function connect($sHost, $sUsername, $sPassword);

	// Closes the database connection
	abstract protected function close();

	/////// PUBLIC /////////////////////////////////////////////////////////////////

	// Selects specified database
	abstract public function SelectDatabase($sDatabase);

	// Execute a select query - returns array of values
	abstract public function SelectQuery($sTable, $sData, $sOrder = NULL, $bOrder = "ASC");

	// Execute a select max query - returns single value
	abstract public function SelectMax($sTable, $sColumn);

	abstract public function SelectQueryWhere($sTable, Array $aData, Array $aWhere = NULL, Array $aOrder = NULL, $sOrder = "ASC");

	// Inserts a trade into the database
	// Requires the table name and an array of key=>value pairs to be inserted as column=>data pairs
	// If primaryKey is set, the row will be updated instead of inserted
	abstract public function InsertQuery($sTable, Array $aData);


} // class

?>
