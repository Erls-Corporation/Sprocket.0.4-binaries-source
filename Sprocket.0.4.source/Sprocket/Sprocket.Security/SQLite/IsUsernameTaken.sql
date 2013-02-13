	SELECT COUNT(*)
	  FROM Users AS u
INNER JOIN ClientSpaces AS c
		ON u.ClientSpaceID = c.ClientSpaceID
	 WHERE u.Username = @Username
	   AND c.ClientSpaceID = @ClientSpaceID