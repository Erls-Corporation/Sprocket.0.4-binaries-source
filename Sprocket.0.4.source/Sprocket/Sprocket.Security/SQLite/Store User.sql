INSERT OR UPDATE INTO Users
(UserID, ClientSpaceID, Username, PasswordHash, FirstName, Surname, Email, LocalTimeOffsetHours,
 Enabled, Hidden, Locked, Deleted, Activated, ActivationReminderSent, Created)
VALUES
(@UserID, @ClientSpaceID, @Username, @PasswordHash, @FirstName, @Surname, @Email, @LocalTimeOffsetHours,
 @Enabled, @Hidden, @Locked, @Deleted, @Activated, @ActivationReminderSent, @Created);