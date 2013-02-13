	SELECT u.Enabled
	  FROM Users AS u
INNER JOIN ClientSpaces AS c
		ON u.ClientSpaceID = c.ClientSpaceID
	 WHERE u.Username = @Username
	   AND u.PasswordHash = @PasswordHash
	   AND u.Enabled = 1
	   AND c.Enabled = 1
	   AND c.ClientSpaceID = @ClientSpaceID